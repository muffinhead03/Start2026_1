# Ollama 네고 시스템 실험

## 실험 개요
- 모델: gemma3:1b → gemma3:4b
- 환경: Ollama 로컬 (Mac, localhost:11434)
- 목표: 손님 NPC가 성향/상황에 따라 가격 흥정 JSON 응답을 생성하는지 검증

## 응답 형식
{"intent":"buy|counter|reject|hesitate","price":숫자,"dialogue":"손님 대사 한 줄"}

## 1b vs 4b 비교
| 항목 | gemma3:1b | gemma3:4b |
|------|-----------|-----------|
| intent 단일값 | 실패 | 성공 |
| 가격 논리 | 오류 | 정상 |
| 역할 유지 | 판매자 시점 혼입 | 손님 시점 유지 |
| 성향 반영 | 거의 안 됨 | 자연스럽게 반영됨 |
| 코드블록 | 발생 | 발생 (Unity Regex로 처리) |

## 폴더 구조
- prompts/ - 시스템 프롬프트 버전별 기록
- results/ - 케이스별 실험 결과 JSON
