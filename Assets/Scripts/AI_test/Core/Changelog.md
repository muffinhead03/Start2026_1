## [0.1.0] - 2026-08-26

### Added
- `HintEngine`: 플레이어 상태 기반 힌트 레벨(1~4) 판단 로직 (가중치: 감정상태 35%, 요청강도 25%, 정체시간 20%, 퍼즐진행 15%, 반복조사 5%)
- `PromptBuilder`: 판단 결과를 LLM 프롬프트로 조립
- `IHintLLMClient` 인터페이스 — LLM 백엔드 추상화 (기존 `LLMClient`가 이를 구현하도록 리팩터링)
- `IPuzzleDataProvider` 인터페이스 — 퍼즐 데이터 조회 추상화 (기존 `PuzzleConfigData`가 이를 구현하도록 리팩터링)
- `ISceneContextProvider` 인터페이스 + `SceneContextData` ScriptableObject — 씬 배경 텍스트를 코드가 아닌 에셋으로 관리 가능
- `HintSystemConfig` — 언어/톤/최대 문장수를 설정 가능하게 분리 (다국어 대응 준비)
- `HintSystem.Core` 어셈블리 분리 (`Core`/`Runtime`/`Editor` 폴더 구조)
- `package.json` 추가 (UPM 패키지화 준비)
- `HintEngine` 유닛 테스트 6종 (`Tests/HintEngineTests.cs`) — 레벨 판단 경계값, `failCount` 반영 여부 등 검증
- 최소 통합 예제 씬 (`Samples/MinimalExample`) — 와인방/오르간방 등 게임 콘텐츠 없이 Core 패키지만으로 힌트 판단 로직을 확인 가능
- 모델 다운로드 스크립트 (`scripts/download_model.py`) — Gemma 3 4B(Q4_K_M) 모델을 SHA256 검증과 함께 자동 다운로드

### Changed
- `HintManager`의 `llmClient` 필드 타입을 구체 클래스(`LLMClient`)에서 인터페이스(`IHintLLMClient`)로 변경
- `PromptBuilder.SystemPrompt`의 언어("Korean")/톤 하드코딩을 `HintSystemConfig` 기반으로 변경

### Fixed
- `WineBookPutOn`에서 틀린 책을 놓으려 시도할 때 `failCount`가 증가하지 않던 문제 수정 (`RegisterFail()` 추가, 2초 쿨다운 적용)
