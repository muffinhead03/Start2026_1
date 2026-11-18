# Changelog

이 프로젝트의 모든 주요 변경사항을 이 파일에 기록한다.
포맷은 [Keep a Changelog](https://keepachangelog.com/ko/1.1.0/)를 따르고,
버전은 [Semantic Versioning](https://semver.org/lang/ko/)을 따른다.

## [Unreleased]

### Added
- `HintLanguageProfile` — 언어별 프롬프트 언어 이름 + 친구 말투 규칙(한국어 반말 / 영어 캐주얼 / 일본어 タメ口) 묶음. 새 언어는 프로필 추가만으로 대응
- `IHintLanguageSource` 인터페이스 — 현재 힌트 언어를 알려주는 대상 추상화 (게임 언어 설정과의 결합은 구현체가 담당)
- `IHintUITextProvider` 인터페이스 + `HintUITextId` — 힌트 패널 고정 문구(잡음, 인사말, 횟수 소진 등)를 언어별로 받는 추상화
- `IHintUsageStore` 인터페이스 — 힌트 사용 횟수 저장 추상화 (게임을 껐다 켜도 같은 구간이면 횟수 유지)
- `IHintPhraseTable` 인터페이스 + `ObservationPhraseKeys` — 관찰 문장 틀(위치·거리·이어붙이기)을 언어별 번역 테이블에서 받는 추상화. 테이블이 없으면 한국어 기본값으로 동작

### Changed
- `HintSystemConfig.language`를 `languageProfile`로 교체 (`language`는 읽기 전용 속성으로 유지), `casualKorean` → `casualSpeech`로 일반화
- `PromptBuilder`의 말투 규칙을 한국어 하드코딩에서 `HintLanguageProfile` 기반으로 변경
- `PromptBuilder.LimitSentences`가 일본어 문장 끝(`。！？`, 닫는 괄호 포함)도 문장으로 셈
- `HintEngine` 관찰 문장의 한국어 하드코딩을 `HintEngine.Phrases`(IHintPhraseTable) 기반으로 변경 — 구역·랜드마크 이름도 테이블에 키가 있으면 번역해서 표시, 영어처럼 대소문자가 있는 언어는 문장 첫 글자를 대문자로

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