# Unity 프로젝트 구조 분석 보고서

## 1. 요약

이 프로젝트는 Unity 6000.3.11f1 기반의 1인칭 싱글플레이어 공포·방탈출 게임이다. 플레이어는 여러 씬의 퍼즐을 풀고 탈출하며, 로컬 LLM을 사용하는 힌트 시스템이 플레이 진행 상태를 분석해 단계별 힌트를 제공한다.

현재 구조는 다음 네 부분으로 나뉜다.

1. `LoadingScene`에서 LLM 런타임을 준비한 뒤 `StartingScene`으로 이동하는 부트스트랩 흐름
2. 씬별 퍼즐, 플레이어 이동, 상호작용, 인벤토리, UI가 결합된 게임플레이 코드
3. `SaveSystem`, `GameSetting`, `InputManager`, `LLMSingleton` 같은 전역·공용 상태
4. `Assets/Scripts/AI_test/Core`의 힌트 판단 로직과 `Runtime`의 Unity/LLM 연결 계층

기능의 큰 흐름은 읽을 수 있지만, 진행 저장이 씬 `buildIndex`에 의존하고, 입력 에셋이 두 개 공존하며, 문서와 런타임 설정이 일부 불일치한다. 장기 유지보수와 씬 순서 변경을 고려하면 먼저 진행 식별자, 입력 단일화, 힌트 사용 정책을 정리하는 것이 우선이다.

## 2. 확인 범위와 미확인 범위

### 확인한 범위

- 루트 문서: `README.md`, `Self_demo.md`, `ProjectDescription.md`, `Team_Ground_Rule.md`, `AI_TRANSPARENCY.md`, `CodeMap.md`
- AI 문서: `Assets/Scripts/AI_test/Core/README.md`, `Assets/Scripts/AI_test/docs/known-issues.md`
- Unity 버전·패키지: `ProjectSettings/ProjectVersion.txt`, `Packages/manifest.json`, `Packages/packages-lock.json`
- 프로젝트 설정: `ProjectSettings/ProjectSettings.asset`, `GraphicsSettings.asset`, `QualitySettings.asset`, `InputManager.asset`, `EditorBuildSettings.asset`
- `Assets` 하위 추적 폴더, 씬 목록, 입력 에셋
- 씬 시작·전환, 저장, 플레이어, UI, 인벤토리, AI 힌트 관련 주요 C# 스크립트

### 미확인 또는 이번 조사에서 확정하지 않은 범위

- Unity Editor에서 실제로 각 씬 오브젝트의 Inspector 필드가 어떤 프리팹·컴포넌트에 연결되는지
- 모든 씬의 실제 런타임 오브젝트 구성과 씬별 실행 결과
- Windows/macOS 실제 빌드 성공 여부와 LLM 런타임의 플랫폼별 실행 여부
- 자동화된 테스트의 통과 여부
- `SceneContextData` 에셋이 실제로 Inspector에 연결된 씬이 있는지

이번 작업은 읽기 전용 구조 조사다. Unity Editor 실행, 빌드, 테스트 실행, 서버 시작·재시작, 코드 수정은 수행하지 않았다. Inspector 연결이나 실행 결과를 직접 확인할 수 없는 경우 아래에서 `미확인`으로 표시한다.

## 3. Unity 버전, 패키지, 프로젝트 설정

### Unity와 기본 설정

- Unity 버전: `ProjectSettings/ProjectVersion.txt`의 `m_EditorVersion: 6000.3.11f1`
- revision: `3000ef702840`
- 프로젝트 이름: `ProjectSettings/ProjectSettings.asset`의 `productName: Start2026_1`
- 회사명: `DefaultCompany`
- 기본 해상도: `1024x768`
- 색 공간: Linear
- 렌더링: Universal Render Pipeline 기반
- 품질 프로파일: `Mobile`, `PC`; Standalone 기본 품질은 `PC`

README의 실행 안내도 Unity 6000.3.11f1을 요구하므로 문서와 `ProjectVersion.txt`는 일치한다.

### 주요 패키지

`Packages/manifest.json`과 `Packages/packages-lock.json`에서 확인한 주요 패키지는 다음과 같다.

- LLMUnity: `ai.undream.llm`, Git tag `v3.0.2`
- Universal Render Pipeline: `com.unity.render-pipelines.universal` `17.3.0`
- Input System: `com.unity.inputsystem` `1.19.0`
- AI Navigation: `2.0.11`
- ProBuilder: `6.0.9`
- Timeline: `1.8.11`
- uGUI: `2.0.0`
- Unity Test Framework: `1.6.0`
- Unified Universal Blur: Git 패키지

`packages-lock.json`에서 Unified Universal Blur가 URP `14.0.9`를 dependency로 요구하지만, 프로젝트 직접 의존성은 URP `17.3.0`이다. 실제 호환성은 Unity Editor에서 패키지 임포트와 빌드로 확인해야 하지만, 현재 lock 정보만으로도 버전 정합성 검토가 필요한 상태다.

### 프로젝트 설정상의 관찰

- `ProjectSettings/InputManager.asset`에 구형 입력 축이 남아 있다.
- 동시에 Input System 패키지와 `.inputactions` 에셋을 사용한다.
- `ProjectSettings/EditorBuildSettings.asset`의 `m_configObjects`는 GUID `8ba933f998d30b24d800d07e4f184dee` 입력 에셋을 가리킨다.
- `GameSetting.cs`는 런타임 코드 파일에서 `using UnityEditor;`를 직접 가져온다. `EditorApplication.isPlaying` 사용은 `#if UNITY_EDITOR`로 감싸져 있지만, 런타임 스크립트와 Editor 전용 API가 같은 파일에 있는 구조는 빌드 플랫폼 검토가 필요하다.
- `GameSetting.cs`의 언어 배열에 `"�ѱ���"`처럼 깨진 문자열이 보인다.

## 4. Assets 폴더 구조

주요 추적 폴더와 역할은 다음과 같다.

| 경로 | 역할 |
|---|---|
| `Assets/Scenes` | 게임 씬, 프로토타입, 샘플, 씬별 작업본 |
| `Assets/Scripts` | 플레이어, 씬 로직, UI, 저장, AI 힌트, 입력 코드 |
| `Assets/Scripts/AI_test` | 힌트 시스템 Core/Runtime, 퍼즐 데이터, bridge, 실험 문서 |
| `Assets/Prefabs` | 씬에 배치하는 재사용 오브젝트 |
| `Assets/Resources` | 런타임 이름 기반 로드 후보 |
| `Assets/Settings` | 렌더링·프로젝트 관련 설정 에셋 후보 |
| `Assets/Animation` | 애니메이션 및 컨트롤러 |
| `Assets/Material`, `Assets/Meshes` | 재질과 메시 |
| `Assets/ArtAssetsFromUnityAsset`, `Assets/ArtSource`, `Assets/BlenderAssetTest` | 외부·제작 원본 및 테스트 아트 |
| `Assets/Music`, `Assets/Sounds` | 음악과 효과음 |
| `Assets/TextMesh Pro`, `Assets/UI Toolkit` | UI 관련 리소스 |
| `Assets/TimeLIne` | Timeline 리소스 후보. 폴더명이 `Timeline`과 다르므로 명명 규칙 점검 대상 |
| `Assets/ShelfObjects` | 선반 오브젝트와 샘플 씬 |
| `Assets/StreamingAssets` | 빌드에 포함할 원본 스트리밍 파일 |
| `Assets/MobileDependencyResolver` | 모바일 의존성 관련 외부 구성 |
| `Assets/FontCollector` | 폰트 수집·관리 리소스 |
| `Assets/_Recovery` | 복구·임시 성격으로 보이는 리소스. 소유와 제거 기준 미확인 |

`Assets` 아래에 제작 원본, 외부 에셋, 프로토타입, 실제 런타임 코드가 함께 있어 프로젝트가 커질수록 소유 경계가 흐려질 가능성이 있다. 특히 `AI_test`라는 이름은 현재 핵심 힌트 기능까지 포함하고 있어 테스트 전용 코드와 제품 런타임 코드의 구분을 확인할 필요가 있다.

## 5. 씬 목록과 빌드 설정

### 추적된 씬

현재 저장소에서 확인한 씬은 다음과 같다.

- `Assets/Scenes/BeforeGame/StartingScene.unity`
- `Assets/Scenes/Demo.unity`
- `Assets/Scenes/EndingScene.unity`
- `Assets/Scenes/HyeGyo_Scene/Scene2_Again.unity`
- `Assets/Scenes/HyeGyo_Scene/Scene2_Inventory.unity`
- `Assets/Scenes/HyeGyo_Scene/Scene2_UPdateforINventory.unity`
- `Assets/Scenes/NamScene/LoadingScene.unity`
- `Assets/Scenes/NamScene/NamScene 1.unity`
- `Assets/Scenes/NamScene/NamScene.unity`
- `Assets/Scenes/ObjectScene.unity`
- `Assets/Scenes/Prototype/Prototype.unity`
- `Assets/Scenes/SampleScene.unity`
- `Assets/Scenes/Scene2/Scene2.unity`
- `Assets/Scenes/Scene_Tutorial/DollSceneUpdate.unity`
- `Assets/Scenes/Tutorial.unity`
- `Assets/ShelfObjects/ShelfSampleScene.unity`

### EditorBuildSettings의 직렬화 순서

`ProjectSettings/EditorBuildSettings.asset`에는 다음 순서가 저장되어 있다.

| 직렬화 위치 | 활성화 | 씬 |
|---:|:---:|---|
| 0 | 아니오 | `Assets/Scenes/SampleScene.unity` |
| 1 | 아니오 | `Assets/Scenes/Tutorial.unity` |
| 2 | 예 | `Assets/Scenes/NamScene/LoadingScene.unity` |
| 3 | 예 | `Assets/Scenes/BeforeGame/StartingScene.unity` |
| 4 | 예 | `Assets/Scenes/Scene_Tutorial/DollSceneUpdate.unity` |
| 5 | 예 | `Assets/Scenes/NamScene/NamScene 1.unity` |
| 6 | 예 | `Assets/Scenes/Scene2/Scene2.unity` |
| 7 | 예 | `Assets/Scenes/EndingScene.unity` |

`SampleScene`과 `Tutorial`은 파일은 존재하지만 Build Settings에서는 비활성화되어 있다. 반면 `StartingScene.cs`의 기본 `tutorialSceneName`은 `Tutorial`이다. 따라서 새 게임 버튼이 비활성화된 `Tutorial`을 이름으로 로드하려는 구조인지, 실제 씬이 다른 씬으로 교체되었는지는 Editor 실행으로 확인해야 한다. 이 부분은 현재 가장 먼저 검증할 런타임 흐름 중 하나다.

### 시작 및 진행 흐름

1. `Assets/Scripts/LoadingScene/LoadingSceneController.cs`의 `LoadingSceneController.Start()`가 `LLMSingleton.Instance`와 `LLMClient`를 찾는다.
2. LLM이 준비되었거나 실패하면 `GoToNextScene()`으로 이동하고, 기본 `nextSceneName`은 `StartingScene`이다.
3. 워밍업 중에는 최대 90초 대기하며, 설정에 따라 일정 시간 후 사용자가 건너뛸 수 있다.
4. `Assets/Scripts/AI_test/Runtime/LLMSingleton.cs`의 `Awake()`는 싱글톤과 `DontDestroyOnLoad`를 사용한다.
5. `Assets/Scripts/StartingScene/StartingScene.cs`의 `StartGame()`은 인트로 코루틴 뒤 `tutorialSceneName`을 이름으로 로드한다.
6. `StartingScene.ContinueGame()`은 `GameSetting.data.current_stage`를 `SceneManager.LoadScene(int)`에 전달한다.
7. `Assets/Scripts/GameSetting.cs`의 `Awake()`는 `StartingScene`이 아닌 활성 씬의 `buildIndex`를 `current_stage`에 기록한다.
8. 씬별 퍼즐 전환은 `Event_LoadScene`, `Doll_SceneChangeTrigger` 등 여러 스크립트에서 `SceneManager.LoadScene`을 직접 호출한다.
9. `EndingManager.GoToStartScene()`도 문자열 `StartingScene`을 직접 로드한다.

## 6. 주요 스크립트와 의존 관계

### 공용 시작·설정·저장 계층

- `Assets/Scripts/LoadingScene/LoadingSceneController.cs`
  - LLM 준비 상태를 확인하고 진행률·타임아웃·건너뛰기 UI를 관리한다.
  - `LLMSingleton`, `LLMClient`, `SceneManager`에 의존한다.
- `Assets/Scripts/AI_test/Runtime/LLMSingleton.cs`
  - LLM 클라이언트를 전역으로 보관하고 씬 전환 뒤에도 유지한다.
- `Assets/Scripts/StartingScene/StartingScene.cs`
  - 새 게임, 이어하기, 설정 열기, 종료 버튼을 처리한다.
- `Assets/Scripts/GameSetting.cs`
  - 저장된 설정을 읽고, 설정 UI·입력·포인터·오디오 믹서를 제어한다.
  - `GameSetting.data` static 상태를 사용한다.
- `Assets/Scripts/SaveSystem.cs`
  - `PlayerData`를 `Application.persistentDataPath/savefile.json`에 JSON으로 저장한다.
  - 저장 필드는 `hintLevel`, `mouseSensitivity`, `soundEffect`, `language`, `current_stage`다.

### 플레이어와 상호작용

- `Assets/Scripts/Scene2/Player_Move.cs`
  - `CharacterController` 기반 이동, 시점, 걷기·달리기, 숙이기, 점프, 중력, 발소리를 처리한다.
  - `InputSystem.actions`에서 `Look`, `Move`, `Run`, `Hide`, `Jump`를 직접 찾는다.
  - `GameSetting.data.mouseSensitivity`와 `CameraPivot`, `Play_Audio`에 의존한다.
- `Assets/Scripts/Scene2/Player_Interaction.cs`
  - 화면 중앙 raycast로 `Event_On_Ray` 대상을 찾고 `Interact`, `Interact While Inspecting`을 처리한다.
  - `Player_Grab`, `Scene_UI_Manager`, `InventoryUIManager`와 연결된다.
- `Assets/Scripts/InputManager.cs`
  - `PC_Player`와 `PC_UI` action map을 켜고 끈다.
- `Assets/Scripts/Scene2/Scene_UI_Manager.cs`
  - 패널, 버튼, 슬라이더, TextMeshPro 텍스트, 포인터 잠금과 커서를 관리한다.

### 인벤토리

- `Assets/Scripts/HyeGyo_Part/InventoryPart/InventoryData.cs`
  - 8개 슬롯, 원본 오브젝트, 선택 및 장착 상태를 관리하는 현재 인벤토리 계층이다.
- `Assets/Scripts/HyeGyo_Part/InventoryPart/InventoryInputBridge.cs`
  - `InputActionReference`로 인벤토리 열기·닫기·확인·스크롤을 연결한다.
  - 인벤토리가 열리면 월드 `Interact` 액션을 비활성화하고 닫히면 다시 동기화한다.
- `Assets/Scripts/Scene2/Player_Inventory.cs`
  - 별도의 static `List<string>` 기반 legacy 인벤토리다.

현재 `InventoryData` 계층과 `Player_Inventory` 계층이 함께 존재한다. 어떤 씬과 시스템이 어느 계층을 정식 기준으로 사용하는지 문서화되지 않으면 아이템 보유 상태와 힌트 입력이 서로 다른 데이터를 읽을 수 있다.

## 7. 입력, UI, 플레이어, 게임 진행의 연결

### 입력 에셋

`Assets/EX_CharacterController_InputSystem.inputactions`는 metadata GUID `8ba933f998d30b24d800d07e4f184dee`이며 `PC_Player`, `PC_UI` map을 가진다. `Move`, `Look`, `Interact`, `Run`, `Hide`, `Jump`, `InventoryOpen`, `SettingOpen`, `HintOpen`, `InventoryClose`, `SettingClose`, `HintClose`, `Click` 등이 정의되어 있고 WASD, 마우스, E, Shift, Ctrl, Space, I, Q, F, Esc 등의 바인딩이 있다.

별도로 `Assets/InputSystem_Actions.inputactions`도 존재한다. 이 파일은 `Player`, `UI` map과 다른 action 이름 체계를 사용한다. `EditorBuildSettings.asset`이 직접 가리키는 것은 첫 번째 에셋이지만, 코드와 씬의 모든 `InputActionReference`가 어느 파일을 사용하는지는 텍스트만으로 완전히 확정할 수 없다. 두 파일의 공존은 유지보수 시 잘못된 에셋을 수정하거나 런타임 에셋과 문서가 어긋날 위험이 있다.

`ProjectSettings/InputManager.asset`에 구형 입력 축까지 남아 있으므로 입력 시스템은 다음 세 흔적이 동시에 존재한다.

- 현재 전역 Input System 액션 에셋
- 별도 `InputSystem_Actions.inputactions` 에셋
- 구형 `InputManager` 축 설정

현재 코드가 실제로 읽는 액션 이름은 `InputSystem.actions.FindAction("Look")` 같은 전역 액션 조회와 `InputActionReference` 연결이 혼재한다. `InputManager.cs`는 action map 활성화 상태를 제어하고, `HintManager`, `GameSetting`, `Player_Move`는 전역 액션을 직접 찾는다.

### 플레이어 입력 및 UI 상태

`Player_Move`는 `CharacterController`를 이동시킨 뒤 입력 상태에 따라 시점·이동·달리기·숙이기·점프를 적용한다. UI가 열리면 `GameSetting`, `Scene_UI_Manager`, `InventoryInputBridge`, `HintManager`가 각각 포인터와 이동 잠금, player/UI action map 상태를 바꾼다.

이 구조는 기능별로는 이해 가능하지만, UI가 여러 개일 때 누가 최종적으로 입력과 커서를 소유하는지 전역 규칙이 코드에 통합되어 있지 않다. 예를 들어 인벤토리와 힌트·설정 패널이 동시에 열리는 경우의 우선순위는 Inspector 연결과 실행 상태 확인이 필요하다.

## 8. AI 힌트 시스템

### Core 판단 계층

`Assets/Scripts/AI_test/Core/HintSystem.Core.asmdef`의 assembly 이름은 `HintSystem.Core`다. 핵심 구성은 다음과 같다.

- `HintEngine.Calculate()` (`HintEngine.cs`): 상태와 퍼즐 설정에서 힌트 레벨, 상태, 다음 스텝을 결정
- `PromptBuilder.Build()` (`PromptBuilder.cs`): 판단 결과와 씬 컨텍스트를 LLM 프롬프트로 변환
- `PlayerState.cs`: 체류 시간, 힌트 요청 횟수, 실패 횟수, 완료 스텝, 조사 기록 등 보관
- `PuzzleConfig.cs`: `PuzzleStep`, 퍼즐 ID, 단계별 관련 오브젝트 정의
- `IHintLLMClient`, `IPuzzleDataProvider`, `ISceneContextProvider`, `IPossessionProvider`, `ILocationAwareProvider`, `IStepProgressProvider`: Unity 구현과 판단 계층 사이의 인터페이스

`HintEngine.Calculate()`는 다음 가중치로 점수를 계산한다.

- 힌트 이력 및 체류: 35%
- 요청 방식: 25%
- 정체·실패: 20%
- 퍼즐 진행률: 15%
- 반복 조사: 5%

다음 안내 스텝은 손에 든 오브젝트, 인벤토리 오브젝트, 가장 낮은 미완료 체크리스트 순서로 선택한다. 손이나 인벤토리 정보가 있으면 체크리스트 순서를 override할 수 있다. 위치 제공자가 있으면 오브젝트와의 거리·구역 체류를 판단하고, 진행도 제공자가 있으면 씬별 세부 진행 문장을 추가한다.

### Runtime 연결 계층

- `Assets/Scripts/AI_test/Runtime/HintManager.cs`
  - 씬에서 `PlayerState`를 새로 생성한다.
  - `LLMSingleton.Instance.LlmClient`를 `IHintLLMClient`로 주입한다.
  - `HintOpen`/`HintClose` 액션을 등록한다.
  - 기본 `currentPuzzleId`는 `wine_glass_room`이다.
  - Inspector 기본 `maxHints`는 4다.
  - LLM 응답 실패·타임아웃 시 고정 fallback 문구를 사용한다.
  - `DefaultSceneContextProvider`, `DefaultActionLabelProvider`를 기본 provider로 사용하고, `sceneContexts`가 연결되었을 때만 ScriptableObject 기반 provider로 교체한다.
- `Assets/Scripts/AI_test/Runtime/LLMClient.cs`
  - LLMUnity `LLMCharacter`를 워밍업하고 스트리밍 응답을 받는다.
  - 첫 chunk timeout은 30초, chunk idle timeout은 10초로 기록되어 있으며 실패 시 fallback 경로가 있다.
- `Assets/Scripts/AI_test/Runtime/OllamaClient.cs`
  - `http://localhost:11434/api/generate`를 호출하는 초기 실험 구현이다.
  - known issues 문서와 ADR 기준으로 production 주 사용 경로가 아닌 prototype으로 기록되어 있다.
- `Assets/Scripts/AI_test/Runtime/PuzzleConfigData.cs`
  - `wine_glass_room`, `organ_room`, `doll_room` 세 퍼즐 ID를 지원한다.
- `WineGlassRoomData.cs`, `OrganRoomData.cs`, `DollRoomData.cs`
  - 각각 7, 17, 8개 퍼즐 단계를 제공한다.
- `WineHintBridge.cs`, `OrganHintBridge.cs`, `DollHintBridge.cs`
  - 씬별 진행·소지품·위치 상태를 provider로 연결한다.
  - Organ bridge는 Inspector 배열 인덱스에, Doll bridge는 hard-coded hierarchy path에 의존한다.
- `WineLocationProvider.cs`
  - Inspector의 `relatedObjectName`과 Transform 매핑을 사용한다.
  - zone tracking은 현재 null/0으로 처리되는 구조다.

### 문서와 코드의 힌트 정책 불일치

`README.md`, `Self_demo.md`, `ProjectDescription.md`는 씬당 힌트 2회라고 설명한다. 그러나 `HintManager.cs`의 `[SerializeField] int maxHints = 4`와 실제 제한 로직은 기본 4회다. 제품 정책을 2회로 할지 4회로 할지 결정한 뒤 문서, Inspector 기본값, 안내 UI를 함께 맞춰야 한다.

`known-issues.md`는 `OnSceneChanged`와 `currentPuzzleId` 문제를 진행 중과 해결됨 항목에 동시에 기록한다. 현재 코드의 `HintManager`가 씬마다 `Start()`에서 `PlayerState`를 새로 생성하는 점은 해결 설명과 맞지만, 문서의 과거 이슈와 해결 상태가 한 문서에 섞여 있어 최신 상태를 판별하기 어렵다.

## 9. 위험 요소와 개선 우선순위

### 높음: 진행 저장이 build index에 의존

근거: `SaveSystem.PlayerData.current_stage`, `GameSetting.Awake()`의 `SceneManager.GetActiveScene().buildIndex`, `StartingScene.ContinueGame()`의 `SceneManager.LoadScene(savedSceneNumber)`.

현재 이어하기는 씬 이름이나 별도 진행 ID가 아니라 빌드 인덱스를 저장한다. `EditorBuildSettings.asset`에는 비활성 씬도 인덱스 0, 1로 포함되어 있고, 씬을 삽입·삭제·활성화하면 기존 저장 데이터가 다른 씬을 열 수 있다.

권고 우선순위 1: `tutorial`, `doll_room`, `organ_room`처럼 안정적인 progression key를 저장하고, key와 씬 이름을 별도 매핑한다. 기존 저장 파일 호환을 위해 저장 포맷 버전과 기존 index 마이그레이션 규칙도 함께 둔다.

### 높음: 힌트 사용 횟수 정책 불일치

근거: README 계열 문서의 씬당 2회 설명과 `HintManager.maxHints = 4`, `currentPlayerState.hintCount >= maxHints` 제한 로직.

제품에서 실제 사용자에게 노출되는 횟수와 문서·데모 설명이 다르다. 어떤 값이 의도인지 결정한 뒤 `HintManager` 기본값, 씬 Inspector 값, usage UI, README 및 시연 문서를 한 변경 단위로 맞춰야 한다.

### 높음: 입력 에셋 두 개 공존

근거: `Assets/EX_CharacterController_InputSystem.inputactions`는 Project Settings GUID `8ba933...`로 연결되어 있고, `Assets/InputSystem_Actions.inputactions`도 별도로 존재한다. 코드에서는 전역 `InputSystem.actions` 조회와 `InputActionReference`가 혼재한다.

권고 우선순위 2: 실제 사용 에셋을 하나로 결정하고 나머지는 제거 또는 명확히 prototype 영역으로 격리한다. action map과 action 이름을 표로 고정하고, 씬의 `InputActionReference` 연결을 Editor에서 검증한다. 구형 `InputManager` 설정도 더 이상 사용하지 않는다면 제거 여부를 결정한다.

### 중간: URP dependency 버전 정합성

근거: `manifest.json`의 URP `17.3.0`과 `packages-lock.json`의 Unified Universal Blur dependency URP `14.0.9`.

패키지 임포트, 셰이더 컴파일, 빌드에서 문제가 발생할 수 있다. Blur 패키지가 Unity 6/URP 17을 지원하는지 확인하고, 지원 버전으로 교체하거나 호환되는 커밋을 고정해야 한다. 이 판단은 Editor 패키지 resolve와 실제 빌드로 검증한다.

### 중간: 씬 계층과 Inspector 배열에 대한 결합

근거: `FindFirstObjectByType`, `GameObject.Find`, Doll bridge의 hard-coded hierarchy path, Organ bridge의 Inspector 배열 인덱스, `WineLocationProvider`의 이름-Transform 매핑.

씬 계층이나 오브젝트 이름을 변경하면 컴파일 오류 없이 provider가 잘못된 상태를 반환할 수 있다. 장기적으로는 ScriptableObject 기반 scene binding 또는 명시적인 컴포넌트 참조를 사용하고, 씬 로드 시 필수 참조를 검증하는 진단 로그를 둔다.

### 중간: 현재 인벤토리와 legacy 인벤토리 공존

근거: `InventoryData`의 8슬롯 구조와 `Player_Inventory`의 static `List<string>` 구조.

상호작용·UI·힌트 bridge가 서로 다른 인벤토리를 읽으면 보유 아이템 기준 힌트가 실제 UI와 달라질 수 있다. 정식 인벤토리 source of truth를 `InventoryData` 또는 다른 한 계층으로 결정하고, legacy 사용처를 씬 단위로 제거·마이그레이션한다.

### 중간: 전역 mutable state 집중

근거: `LLMSingleton.Instance`, `GameSetting.data`, `HintEngine.Tuning`, `PromptBuilder.SceneContextProvider` 및 static provider 설정.

씬 테스트 순서나 재진입 순서에 따라 상태가 남을 수 있고, 동시에 여러 씬·UI를 테스트하기 어렵다. 우선 초기화 시점과 소유자를 문서화하고, 이후에는 게임 세션 컨텍스트를 명시적으로 전달하는 방향으로 줄이는 것이 좋다.

### 중간: 로컬 AI 런타임 및 플랫폼 의존

근거: `LoadingSceneController`의 최대 90초 워밍업, `LLMClient`의 LLMUnity 의존, `OllamaClient`의 `localhost:11434`, `README.md`의 Ollama/gemma3:4b 요구사항, `known-issues.md`의 macOS Apple Silicon Metal SIGSEGV 미해결 기록.

fallback 경로는 있지만 시작 지연과 플랫폼별 모델 로딩 실패 가능성이 있다. 빌드 실행 전에 AI 사용 가능 여부를 명시적으로 표시하고, CPU-only 우회와 fallback 동작을 Windows/macOS 빌드에서 검증해야 한다.

### 낮음: 런타임 코드와 정리 대상

- `GameSetting.cs`의 Editor 네임스페이스 import와 깨진 언어 문자열
- `ProjectSettings.asset`의 `DefaultCompany` 및 template application identifier
- `using NUnit.Framework`가 runtime 스크립트 일부에 남아 있으나 실제 테스트 파일은 확인되지 않음
- `Assets/Scripts/AI_test/docs/known-issues.md`의 과거 이슈와 해결 항목이 충돌하는 서술
- `SceneContexts` 폴더·에셋은 추적 파일 목록에서 확인되지 않았고, 현재 runtime 기본값은 `DefaultSceneContextProvider`인 것으로 보임

## 10. 개선 권장 순서

1. **진행 저장 안정화**: build index 저장을 progression key/씬 이름 기반으로 바꾸고 기존 save 호환 규칙을 정의한다.
2. **입력 source of truth 확정**: Project Settings에 연결된 입력 에셋과 별도 에셋의 역할을 정리하고, 전역 조회·reference 연결·구형 InputManager의 사용 경계를 문서화한다.
3. **힌트 정책 정합성 회복**: 2회와 4회 중 제품 정책을 확정하고 코드, Inspector, UI, README, 시연 문서를 동시에 수정한다.
4. **패키지 호환성 검증**: Unity 6000.3.11f1과 URP 17.3.0에서 Unified Universal Blur를 resolve·import·build한다.
5. **인벤토리 단일화**: `InventoryData`와 `Player_Inventory` 중 하나를 기준으로 정하고 bridge와 힌트 provider가 같은 상태를 읽도록 정리한다.
6. **씬 참조 진단 강화**: hard-coded path와 배열 인덱스 연결에 로드 시 검증을 추가하고, 가능하면 명시적 참조 또는 ScriptableObject binding으로 이동한다.
7. **AI와 전역 상태 테스트**: LLM 미사용, warmup timeout, fallback, 씬 재진입, 저장 불러오기, 입력/UI 전환을 자동화 또는 최소 수동 테스트 시나리오로 정리한다.
8. **문서와 릴리스 설정 정리**: known issues를 현재 상태 기준으로 갱신하고 회사명, application identifier, 언어 문자열, prototype 폴더의 배포 경계를 정리한다.

## 11. 결론

프로젝트는 `LoadingScene` → `StartingScene` → 튜토리얼·퍼즐 씬 → 엔딩이라는 실행 골격과, `HintEngine` 중심의 분리된 힌트 판단 계층을 갖추고 있다. 특히 힌트 레벨을 코드가 결정하고 LLM은 문장 표현을 담당하도록 나눈 점, 씬별 bridge와 provider 인터페이스를 둔 점은 확장 방향이 명확하다.

반면 게임 진행·입력·인벤토리·씬 참조가 여러 구현 방식으로 공존하고 있어 변경 시 런타임에서만 드러나는 오류 위험이 있다. 가장 먼저 build index 기반 저장, 입력 에셋 중복, 힌트 사용 횟수 불일치를 해결한 뒤 패키지 호환성과 씬 참조 안정화를 진행하는 순서가 적절하다.

이번 보고서에서 Unity Editor Inspector 연결, 실제 씬 실행, Windows/macOS 빌드 결과는 확인하지 않았다. 따라서 위 항목의 최종 판정은 Unity 6000.3.11f1에서 Editor로 각 활성 빌드 씬을 실행하고, 패키지 resolve 및 테스트/빌드를 수행한 뒤 확정해야 한다.
