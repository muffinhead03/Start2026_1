# Known Issues

## Metal GPU 백엔드 + Unity Mono ThreadPool = 결정론적 크래시 (macOS/Apple Silicon)

**증상**: Metal GPU 백엔드로 빌드한 llama.cpp를 Unity 안에서 실행하면 `ggml_metal_op_mul_mat`(+560) 지점에서 SIGSEGV 발생. 타이밍에 따라 가끔 나는 게 아니라, 재현 조건이 갖춰지면 매번 같은 지점에서 크래시.

**환경**: Mac17,2 (Apple M5, MTLGPUFamilyMetal4), Unity 6000.3.11f1, LlamaLib v2.0.5 fork.

**원인 (추정)**: Unity가 이미 보유한 Metal 컨텍스트와, llama.cpp가 Unity의 Mono ThreadPool 위에서 별도로 초기화하는 자신만의 Metal 컨텍스트가 상태 충돌을 일으키는 것으로 보임. 레이스 컨디션이 아니라 구조적 충돌.

**우회 방법**: CPU-only로 빌드된 dylib 사용 (`n_gpu_layers = 0`). 자세한 배경은 `adr/0003-cpu-only-over-metal.md` 참고.

**해결 여부**: 미해결. llama.cpp/Metal 백엔드 업스트림 변경으로 향후 해결될 가능성 있음. Windows 환경에서 동일 이슈가 재현되는지는 미확인.

---

## `failCount`가 증가하지 않아 Level 4 힌트가 수학적으로 도달 불가능했던 문제 (수정됨)

**증상**: `HintEngine`의 점수 계산식상 최대 도달 가능 점수가 약 5.65점이었는데, Level 4 임계값은 6.0점이라 아무리 오래 플레이해도 Level 4 힌트를 볼 수 없었음.

**원인**: `failCount`를 증가시키는 지점(엔티티 사망, 퍼즐 실패 등)이 실제 게임 로직에 연결되지 않아 항상 0으로 유지됐음.

**해결**: `RegisterFail()` 메서드를 `HintManager`, 퍼즐 관련 스크립트(`WineBookPutOn`, `Room3Keypad`, `Object_Pwd`)에 2초 쿨다운과 함께 추가.

> 참고: 이후 힌트 레벨 체계가 4단계 → 5단계로 재정렬됨(v0.2.0). `failCount` 연동 수정 덕분에 최대 도달 점수가 충분히 올라가 있어, 5단계 임계값(5점)도 정상적으로 도달 가능함을 확인함.

---

## Ollama 실험용 대안 구현체 잔존

`OllamaClient.cs`는 초기 프로토타입 단계에서 만든 실험적 구현체로, 프로덕션에서는 사용하지 않는다(`adr/0002-llmunity-over-ollama.md` 참고). 코드베이스 정리 시 제거하거나 `Samples~`로 이동할 예정.

---

## 향후 확인이 필요한 항목

- `OnSceneChanged`가 실제 씬 전환 시 호출되지 않아, 퍼즐 ID/힌트 카운트가 씬을 넘어가도 리셋되지 않는 문제 (진행 중)
- `Scene2.unity`(오르간룸)가 아직 싱글톤 이전 방식의 LLMClient 연결을 사용 중이고, `currentPuzzleId` 기본값도 `wine_glass_room`으로 잘못 설정되어 있음 (진행 중)

## 해결됨 (재확인 완료, 0901)

- `OnSceneChanged` 미호출 문제: `HintManager`가 `DontDestroyOnLoad` 없이 씬마다 `Start()`로 완전히 재초기화되는 구조로 바뀌어 구조적으로 해소됨
- `currentPuzzleId` 오설정 문제: `HintManager.prefab` 기본값이 `organ_room`으로 수정됐고, 와인잔 방 씬은 `wine_glass_room`으로 명시적 override돼 있음을 씬 파일에서 직접 확인함