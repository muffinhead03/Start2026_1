# Ollama 힌트 시스템 실험

## 실험 개요

- **모델**: gemma3:1b → gemma3:4b
- **환경**: Ollama 로컬 (Mac, localhost:11434)
- **목표**: 플레이어의 진행 상황(씬, 퍼즐 단계, 실패 횟수, 체류 시간 등)을 기반으로 LLM이 맥락에 맞는 한국어 힌트 1~2문장을 생성하는지 검증

---

## 시스템 구조 요약

힌트 요청은 아래 파이프라인으로 처리된다.

```
PlayerState (게임 상태)
    ↓
HintEngine.Calculate()   ← 점수 기반으로 hintLevel(1~5), playerStatus 결정
    ↓
HintResult
    ↓
PromptBuilder.Build()    ← 씬 컨텍스트 + 플레이어 상태 → 프롬프트 조립
    ↓
OllamaClient             ← HTTP POST → localhost:11434/api/generate
    ↓
응답 파싱 (response 필드 추출 + 영어 괄호 제거)
    ↓
Unity UI에 한국어 힌트 출력
```

### HintEngine 점수 가중치

| 항목 | 가중치 | 설명 |
|------|--------|------|
| 감정 점수 (체류 시간 + 힌트 횟수 + 실패 횟수) | 35% | 플레이어 답답함 추정 |
| 요청 방식 (direct / indirect) | 25% | 직접 힌트 선택 시 강한 힌트 |
| 정체 점수 (failCount + staySeconds) | 20% | 같은 자리에서 막힌 정도 |
| 진행률 | 15% | 남은 퍼즐 단계 비율 |
| 반복 조사 횟수 | 5% | 같은 오브젝트 3회 이상 조사 |

점수 합산 결과로 `hintLevel 1~5` 결정 → 레벨이 높을수록 직접적인 힌트 제공.

---

## 프롬프트 구조

### 시스템 프롬프트 (고정)

```
You are a hint guide AI in a Korean horror escape room game.
CRITICAL: You MUST respond in Korean language ONLY.
You must write exactly 1 to 2 sentences, never more.
Never reveal the answer directly.
Always maintain a creepy and atmospheric tone.
```

### 유저 프롬프트 (요청마다 동적 조립)

```
[Scene context]
{씬별 배경 정보 — 오브젝트, 퍼즐 구조, 탈출 조건}

[Player state]
Hint level: {1~5} ({레벨 설명})
Hint style: {direct / indirect and atmospheric}
Player status: {단서 미발견 / 반복 실패 / 포기 직전 ...}
Next step goal: {다음 달성해야 할 단계 목표}
Hint direction: {힌트 방향 설명}

[Player question type]
{puzzle / exit / entity 중 해당 설명}

Korean hint (Korean language only, no English):
```

현재 지원하는 씬: `wine_glass_room`, `organ_room`

---

## 모델 비교: gemma3:1b vs gemma3:4b

| 항목 | gemma3:1b | gemma3:4b |
|------|-----------|-----------|
| 한국어 유지 | 간헐적으로 영어 혼입 | 한국어 전용 유지 |
| 문장 수 제한 | 초과 발생 | 1~2문장 준수 |
| hintLevel 반영 | 레벨 차이 거의 없음 | 레벨에 따라 강도 차별화 |
| playerStatus 반영 | 반영 미흡 | 상태에 맞게 자연스럽게 반영 |
| 씬 컨텍스트 활용 | 일반적 힌트에 그침 | 씬 오브젝트/구조 기반 힌트 생성 |
| 코드블록 삽입 | 발생 | 발생 (Unity `ParseResponse()`로 처리) |
| 영어 괄호 번역 삽입 | 발생 | 발생 (`bracketStart` 로직으로 제거) |

→ **gemma3:4b**를 최종 채택. `OllamaClient.cs`에서 모델명 고정.

---

## Unity 응답 파싱 처리

Ollama 응답에서 힌트 텍스트를 추출하기 위해 `OllamaClient.ParseResponse()`에 다음 처리를 적용:

1. `"response":"..."` 필드 직접 추출 (JsonUtility 미사용 — 파싱 오류 방지)
2. `\n`, `\"` 등 이스케이프 문자 복원
3. 모델이 한국어 뒤에 영어 번역을 `(...)` 형태로 붙이는 경우 괄호 이전까지만 사용
4. 앞뒤 따옴표 및 공백 제거

---

## Ollama API 요청 파라미터

```json
{
  "model": "gemma3:4b",
  "system": "{시스템 프롬프트}",
  "prompt": "{유저 프롬프트}",
  "stream": false,
  "options": {
    "temperature": 0.7,
    "num_predict": 80
  }
}
```

`num_predict: 80` — 1~2문장 제한에 맞게 토큰 수 제한.

---

## 폴더 구조

```
experiments/
└── ollama-hint-system/
    ├── prompts/   # 시스템 프롬프트 버전별 기록
    └── results/   # 씬/케이스별 실험 결과 로그
```
