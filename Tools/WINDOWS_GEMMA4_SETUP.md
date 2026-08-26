# Windows에서 Gemma 4 힌트 시스템 테스트하기

`exp/gemma4-window` 브랜치 전용 가이드입니다. Windows에서 이 프로젝트를 처음 열고
Gemma 4 힌트가 정상 동작하도록 만드는 전체 과정을 다룹니다.

## 왜 이 작업이 필요한가

LLMUnity 패키지(`ai.undream.llm`)는 프로젝트를 처음 열 때 `Assets/StreamingAssets/`
아래에 LlamaLib 네이티브 바이너리를 **자동으로 다운로드**합니다. 이때 받아오는 건
[undreamai/LlamaLib](https://github.com/undreamai/LlamaLib) 공식 저장소의
`v2.0.5` 릴리즈인데, 이 공식 Windows 빌드에는 Gemma 4를 크래시시키는 버그가
최소 2개 있습니다 (자세한 원인은 이 문서 맨 아래 참고, 전체 디버깅 과정은
[`GEMMA4_WINDOWS_DEBUG_LOG.md`](./GEMMA4_WINDOWS_DEBUG_LOG.md) 참고).

이 브랜치에는 그 버그들을 고친 **커스텀 Windows 빌드**(`Tools/LlamaLib-win-custom-build/`)가
같이 들어있습니다. 아래 스크립트 한 번으로 공식 빌드를 이 커스텀 빌드로 덮어씁니다.
macOS/Linux/Android/iOS 바이너리는 전혀 건드리지 않으므로, 다른 플랫폼에는 영향이
없습니다.

## 이미 이 프로젝트로 작업 중이던 사람이라면

Unity를 새로 설치할 필요도, 프로젝트를 처음부터 다시 열 필요도 없습니다.
아래 "설정 순서"의 **1번(폴더 위치)과 2번(Unity 첫 오픈)은 이미 다 되어있는
상태**라 사실상 해당 없고, **3번(스크립트 실행)과 4번(모델 파일 받기)만
하면 됩니다.** 단, 3번 실행 전에 **Unity 에디터를 완전히 종료**하세요 (아래
3번 안내 참고 — 그냥 Play 모드만 껐다 켜는 걸로는 부족합니다).

## 사전 준비물

- Unity **6000.3.11f1** (Unity Hub로 설치)
- Git for Windows ([git-scm.com](https://git-scm.com/download/win)) — **시스템 PATH에
  추가되는 옵션으로 설치할 것** (기본 설치 옵션이면 자동으로 됩니다)
- 이 저장소가 `exp/gemma4-window` 브랜치로 체크아웃되어 있을 것

## 설정 순서

### 1. 프로젝트 폴더 위치 확인 (중요)

**`Documents`, `Desktop`, `Pictures` 등 Windows Defender가 보호하는 폴더 안에
프로젝트를 두지 마세요.** Windows Defender의 "제어된 폴더 액세스"가 기본적으로 켜져
있으면 Unity가 그 안에서 파일을 쓰지 못해 `"Project folder or disk is read only"`
에러가 납니다.

- 프로젝트를 `C:\Dev\...`, `C:\Users\<나>\UnityProjects\...` 같은 보호되지 않는
  경로에 두거나,
- 이미 Documents 안에 있다면 Windows 보안 → 바이러스 및 위협 방지 → 랜섬웨어 방지 →
  제어된 폴더 액세스를 끄거나 Unity.exe/Unity Hub.exe를 허용 목록에 추가하세요.

### 2. Unity로 프로젝트 열기

Unity Hub에서 이 저장소 폴더를 Add 하고 엽니다. **최초 1회는 반드시 이 단계를
먼저 완료**해야 합니다 — LLMUnity가 이때 `Assets/StreamingAssets/LlamaLib-v2.0.5/`
폴더 구조를 자동으로 만들어주기 때문에, 이 폴더가 없으면 3번 스크립트가 실패합니다.

패키지 resolve 중 `"No 'git' executable was found"` 에러가 나면:
1. Git이 설치돼 있는지, 시스템 PATH에 있는지 확인 (새 터미널에서 `git --version`)
2. Unity Hub를 **완전히 종료**(트레이 아이콘까지 전부) 후 재시작 — Hub가 실행
   중이던 상태에서 PATH를 바꾸면 이미 떠 있는 프로세스는 예전 PATH를 계속 씀

첫 임포트는 프로젝트 규모상 시간이 꽤 걸립니다 (수 분~수십 분).

### 3. 커스텀 LlamaLib 빌드 적용

**먼저 Unity 에디터를 완전히 종료하세요 (Play 모드 정지가 아니라 프로젝트
자체를 닫으세요).** 오늘 반영된 수정사항 중 하나가 "네이티브 DLL을 한 번
로드하면 프로세스가 끝날 때까지 언로드하지 않는다"는 것이라, 힌트 기능을
한 번이라도 써본(Play 모드에서 LLM이 로드된) 상태로 Unity를 켜놓고 있으면
그 DLL이 계속 파일 잠금 상태로 붙잡혀서 아래 스크립트가 실패하거나
(`Access denied` 계열 에러), 덮어쓰기가 되더라도 이미 메모리에 있는 구버전이
계속 쓰입니다.

Unity를 닫은 다음 (`Assets/StreamingAssets/LlamaLib-v2.0.5/win-x64/` 폴더가
있는지 확인 — 처음 여는 사람만 해당, 기존 작업자는 이미 있음), PowerShell에서:

```powershell
cd <저장소 경로>
.\Tools\setup-custom-llamalib-win.ps1
```

`Replaced: ...` 4줄이 뜨면 성공입니다. 그다음 Unity를 새로 여세요.

### 4. Gemma 4 모델 파일 받기

모델 파일은 3GB가 넘어서 저장소에 포함되어 있지 않습니다. 직접 받아서 넣어주세요:

- 다운로드: https://huggingface.co/unsloth/gemma-4-E2B-it-GGUF/resolve/main/gemma-4-E2B-it-Q4_K_M.gguf
- 저장 위치: `Assets/StreamingAssets/gemma-4-E2B-it-Q4_K_M.gguf` (파일명 그대로)

`mmproj-*.gguf` 파일은 받을 필요 없습니다 (텍스트 전용 사용).

### 5. 테스트

1. Unity에서 `Assets/Scenes/HyeGyo_Scene/Scene2_Again.unity` (또는
   `Scene2_Inventory.unity`) 열기
2. Play
3. 게임 안에서 무전기 조작 → 힌트 요청
4. Console에 `[힌트 결과] 모델: gemma-4-E2B-it-Q4_K_M.gguf ...` 로그가 뜨고
   실제 한국어 힌트 텍스트가 나오면 성공

## 문제가 생기면

| 증상 | 원인 / 조치 |
|---|---|
| `Could not find Unity Package Manager local server application` | 이전 Unity 설치가 손상됨. `C:\Program Files\Unity\Hub\Editor\<버전>` 폴더를 지우고 Unity Hub에서 재설치 (관리자 권한 없이 설치하려면 Unity Hub 설정에서 설치 경로를 사용자 폴더로 변경) |
| `Library\Bee\artifacts\...` 관련 `FileNotFoundException` | 첫 임포트 중 빌드 캐시 레이스 컨디션, 대개 일시적 — 프로젝트 재시작으로 재시도. 반복되면 `Library` 폴더를 통째로 지우고 재임포트 (캐시라서 안전, 다만 다시 시간 걸림) |
| `LLMService construction returned null pointer` 또는 `LlamaLib error -1` | 3번 스크립트를 안 돌렸거나 실패한 상태로 공식(미수정) 빌드가 그대로 쓰이고 있을 가능성. 스크립트 재실행 후 Unity 재시작 |
| 힌트 요청이 매우 느림 (20초+) | CPU 추론이라 정상입니다 (`_numGPULayers: 0`). GPU가 있는 머신이면 `LLM_Manager` 프리팹의 `_numGPULayers`를 올려서 테스트해볼 수 있습니다 |

## 참고: 공식 빌드가 왜 깨져있는가 (요약)

1. **빌드 실패**: `common/jinja/utils.h`의 UTF-8 리터럴이 비-UTF8 활성 코드페이지
   Windows(한국어 등)에서 MSVC에 의해 깨져서 `C2001` 컴파일 에러 발생
2. **런타임 크래시 (댕글링 레퍼런스)**: jinja 매크로 실행 클로저가 컨텍스트를
   참조로 캡처해서, 매크로가 스코프 밖에서 호출되면 해제된 스택 메모리를 읽음
   (macOS/Linux에서는 우연히 안 터짐, Windows/MSVC 스택 레이아웃에서는 재현됨)
3. **런타임 크래시 (OpenMP DLL 언로드)**: Windows는 ggml 연산에 시스템 OpenMP
   런타임(`vcomp140.dll`)을 쓰는데, 이 런타임의 영구 워커 스레드 풀을 정지시킬
   방법이 없어서, LlamaLib DLL을 언로드하면 아직 실행 중이던 스레드가 언맵된
   메모리를 실행하려다 크래시함

전체 과정과 각 항목의 상세 근거는 [`GEMMA4_WINDOWS_DEBUG_LOG.md`](./GEMMA4_WINDOWS_DEBUG_LOG.md)에 정리되어 있습니다.
