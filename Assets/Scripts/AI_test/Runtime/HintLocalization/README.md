# HintLocalization — 힌트 시스템 번역

힌트 패널에 나오는 모든 고정 문장(한국어·영어·일본어)이 여기 있음.
**힌트 문장을 추가하거나 고칠 땐 이 폴더만 보면 됨.**

| 폴더 | 내용 |
|---|---|
| `Scripts/Localization/` (팀 공용) | 아이템 이름 등 팀 번역 — 힌트 쪽은 안 건드림 |
| `AI_test/Core/HintLocalization/` | 인터페이스·키 이름·언어 프로필 (문장 없음, 새 언어 추가 때만 수정) |
| `AI_test/Runtime/HintLocalization/` (여기) | 실제 번역 문장 + 구현 |

## 파일

| 파일 | 내용 |
|---|---|
| `HintLanguageDataKey.cs` | 번역 키 상수 (`Hint{영역}_{대상}`) |
| `HintLanguageData.cs` | 조회(`Get` / `GetCandidates`) + 영역 파일 합치기 |
| `HintLanguageData.UI.cs` | 패널 고정 문구 (치지직... / 도움이 필요해? 등) |
| `HintLanguageData.Observation.cs` | 관찰 문장 틀 ("{0}에 있구나" 등) |
| `HintLanguageData.Actions.cs` | 최근 행동 라벨 (와인·오르간·인형) |
| `HintLanguageData.WineScene.cs` | 와인씬 구역·랜드마크 이름 |
| `HintLocalizationChecker.cs` | 번역 검사 로직 |
| 나머지 (`HintPhraseTable`, `LocalizedHintUIText`, `GameHintLanguageSource`) | Core 인터페이스 구현 — 보통 안 건드림 |

## 문장 고치기 / 추가하기

1. **문장만 고칠 때**: 해당 `HintLanguageData.*.cs`에서 그 키의 Korean / English / Japanese 배열만 수정
2. **새 문장 추가**: `HintLanguageDataKey.cs`에 키 추가 → 해당 영역 파일에 한·영·일 추가
3. **새 씬 영역 파일 추가** (예: `HintLanguageData.OrganScene.cs`): 파일을 만들고 `HintLanguageData.cs`의 `parts` 배열에 한 줄 추가
   — 씬마다 파일을 나눠서, 담당자끼리 같은 파일을 동시에 고쳐 머지 충돌이 나는 일을 막음

## 규칙

- 값은 **후보 배열** — 같은 상황에서 다시 물어도 같은 말을 안 하게 랜덤으로 하나 고름 (후보 개수는 언어끼리 맞추기)
- `{0}`, `{1}`은 자리표시자 — 번역에서 빼먹으면 안 됨 (언어마다 순서는 바뀌어도 됨)
- 행동 라벨에는 쉼표(`,` `、`)를 넣지 않음 — 관찰 문장 틀 안에 들어가서 문장이 어색해짐
- 영어는 문장 중간에도 들어가므로 소문자로 시작 (문장 맨 앞이면 코드가 대문자로 바꿈)
- 말투: 한국어 반말 / 영어 캐주얼 / 일본어 タメ口 (무전기 너머 친구)
- 번역 없는 언어는 게임에서 한국어로 나옴, 키 자체가 없으면 키 이름이 그대로 보임

## 검사

번역을 넣거나 고친 뒤(특히 LLM으로 초안을 만든 뒤) Unity 메뉴 **HintSystem → Validate Translations** 실행.
빠진 번역, 자리표시자 불일치, 후보 개수 차이, 행동 라벨 쉼표를 콘솔에 보여줌.
