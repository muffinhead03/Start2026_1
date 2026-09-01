# Adaptive Hint System

방탈출/퍼즐 게임을 위한, **코드 기반 판단 + 로컬 LLM 기반 자연어 표현을 분리한** 적응형 힌트 시스템.

> "판단은 게임 로직이, 표현은 생성형 AI가 담당한다."

## 왜 필요한가

방탈출 게임의 힌트 시스템은 보통 둘 중 하나다:

- **고정 텍스트 힌트**: 모든 플레이어에게 동일 → 너무 이르면 스포일러, 너무 늦으면 도움이 안 됨
- **LLM에게 판단까지 위임**: 할루시네이션으로 스포일러가 새거나, 아예 틀린 방향을 안내할 위험

이 시스템은 **"언제, 얼마나 많은 정보를 줄지"는 결정론적 코드(HintEngine)가 판단**하고, **LLM(PromptBuilder + 실제 LLM 클라이언트)은 그 판단을 자연어로 표현만** 하도록 역할을 분리한다. 그리고 클라우드 API가 아니라 **로컬 sLM(Gemma 등)** 을 전제로 설계되어, API 비용이나 고성능 GPU 없이도 게임에 내재화할 수 있다.

## 아키텍처

```
플레이어 행동/상태 (PlayerState)
        │
        ▼
  HintEngine.Calculate()   ← 순수 로직, 5개 가중치 요소로 힌트 레벨(1~5) 판단
        │
        ▼
  PromptBuilder.Build()    ← 판단 결과를 프롬프트 문자열로 조립
        │
        ▼
  IHintLLMClient           ← 실제 LLM 백엔드 (구현체는 이 패키지 밖, 게임 프로젝트에 위치)
        │
        ▼
      게임 UI
```

이 패키지(`Core`)에는 **HintEngine, PromptBuilder, 데이터 타입, 인터페이스**만 있다. 실제 LLM 호출·씬 콘텐츠·UI는 이 패키지를 사용하는 프로젝트(예: `Runtime` 폴더) 쪽 책임이다.

## Quick Start

```csharp
// 1. 퍼즐 설정 준비
var config = new PuzzleConfig
{
    puzzleId = "my_room",
    totalSteps = 3,
    steps = new List<PuzzleStep> { /* ... */ }
};

// 2. 현재 플레이어 상태 준비 (게임에서 매 프레임/액션마다 갱신)
var state = new PlayerState { staySeconds = 180, hintCount = 1, failCount = 2 };

// 3. 판단
HintResult result = HintEngine.Calculate(state, config);

// 4. 표현 (LLM에 보낼 프롬프트 조립)
PromptBuilder.SceneContextProvider = myProvider;   // ISceneContextProvider 구현체 필요
string systemPrompt = PromptBuilder.SystemPrompt;
string userPrompt   = PromptBuilder.Build(result);

// 5. 실제 LLM 호출은 IHintLLMClient 구현체(예: LLMUnity 래퍼)에게 위임
myLlmClient.RequestHintStream(systemPrompt, userPrompt, onChunk, onComplete, hintDirection);
```

## 핵심 타입

| 타입 | 역할 |
|---|---|
| `HintEngine` | `PlayerState` + `PuzzleConfig` → `HintResult` 판단 (순수 함수) |
| `PromptBuilder` | `HintResult` → LLM 프롬프트 문자열 조립 |
| `IHintLLMClient` | 실제 LLM 호출을 담당할 백엔드가 구현해야 하는 인터페이스 |
| `IPuzzleDataProvider` | 퍼즐 ID로 `PuzzleConfig`를 조회하는 제공자 인터페이스 |
| `ISceneContextProvider` | 퍼즐 ID+레벨로 씬 배경 텍스트를 조회하는 제공자 인터페이스 |
| `HintSystemConfig` | 언어/톤/문장수 설정 (다국어 대응 가능) |

## 사용 시 준비할 것 (통합 가이드)

이 패키지를 새 프로젝트에 통합하려면:

1. `PlayerState`에 채울 값들을 게임 로직에서 갱신하기
2. `IPuzzleDataProvider` 구현체 만들기 (퍼즐 설정을 어디서 가져올지 — ScriptableObject, JSON 등 자유)
3. `ISceneContextProvider` 구현체 만들기 (씬 배경 텍스트를 어디서 가져올지)
4. `IHintLLMClient` 구현체 만들기 (실제 LLM 백엔드 — 로컬 LLM, 클라우드 API 등 자유)
5. `PromptBuilder.SceneContextProvider`, `PromptBuilder.Config`를 원하는 구현체/설정으로 세팅

## 알려진 이슈 / 제약

`../docs/known-issues.md` 참고 (Metal SIGSEGV 이슈 등).

## 라이선스

(추후 결정 — LICENSE 파일 참고)