# Changelog

이 프로젝트의 모든 주요 변경사항을 이 파일에 기록한다.
포맷은 [Keep a Changelog](https://keepachangelog.com/ko/1.1.0/)를 따르고,
버전은 [Semantic Versioning](https://semver.org/lang/ko/)을 따른다.

## [0.1.0] - 2026-08-24

### Added
- `HintEngine`: 플레이어 상태 기반 힌트 레벨(1~4) 판단 로직 (가중치: 감정상태 35%, 요청강도 25%, 정체시간 20%, 퍼즐진행 15%, 반복조사 5%)
- `PromptBuilder`: 판단 결과를 LLM 프롬프트로 조립
- `IHintLLMClient` 인터페이스 — LLM 백엔드 추상화 (기존 `LLMClient`가 이를 구현하도록 리팩터링)
- `IPuzzleDataProvider` 인터페이스 — 퍼즐 데이터 조회 추상화 (기존 `PuzzleConfigData`가 이를 구현하도록 리팩터링)
- `ISceneContextProvider` 인터페이스 + `SceneContextData` ScriptableObject — 씬 배경 텍스트를 코드가 아닌 에셋으로 관리 가능
- `HintSystemConfig` — 언어/톤/최대 문장수를 설정 가능하게 분리 (다국어 대응 준비)
- `HintSystem.Core` 어셈블리 분리 (`Core`/`Runtime`/`Editor` 폴더 구조)
- `package.json` 추가 (UPM 패키지화 준비)

### Changed
- `HintManager`의 `llmClient` 필드 타입을 구체 클래스(`LLMClient`)에서 인터페이스(`IHintLLMClient`)로 변경
- `PromptBuilder.SystemPrompt`의 언어("Korean")/톤 하드코딩을 `HintSystemConfig` 기반으로 변경

### Fixed
- (해당 시점까지 있었던 기존 버그 수정 사항은 이후 릴리즈에서 별도 기록 예정 — `failCount` 미증가 이슈 등)