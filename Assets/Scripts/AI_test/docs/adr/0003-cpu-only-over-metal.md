# 0003. Apple Silicon에서 Metal 대신 CPU-only 백엔드를 사용한다

## Context

Gemma 4 실험(개인 포크 `nancypark/LlamaLib`, 브랜치 `exp/gemma4-test`) 과정에서, Metal GPU 백엔드로 빌드한 llama.cpp를 Unity(Mac17,2, M5, MTLGPUFamilyMetal4) 안에서 실행하면 `ggml_metal_op_mul_mat`(+560) 지점에서 **결정론적으로 SIGSEGV**가 발생했다. "가끔 발생하는 타이밍 이슈"가 아니라 매번 같은 지점에서 재현되는 크래시였다.

원인을 분석한 결과, Unity가 자체적으로 이미 Metal 컨텍스트를 갖고 있는 상태에서 llama.cpp가 Unity의 Mono ThreadPool 위에서 별도로 자신만의 Metal 컨텍스트를 초기화하려고 하면서, **두 Metal 컨텍스트 간 상태 충돌**이 발생하는 것으로 보인다. 이는 레이스 컨디션이 아니라 구조적인 문제라, 재시도나 락(lock) 추가로는 해결되지 않는다.

## Decision

**Apple Silicon(macOS) 환경에서는 CPU-only로 빌드된 llama.cpp dylib를 사용한다.** GPU 레이어 오프로딩(`n_gpu_layers`)을 사용하지 않는다.

## Consequences

**장점**
- Unity 안에서 안정적으로 동작함 (SIGSEGV 재현되지 않음)
- "고성능 GPU 없이도 동작한다"는 프로젝트의 핵심 차별점(가격 경쟁력)과도 방향이 일치함

**트레이드오프**
- GPU 가속을 못 쓰므로 추론 속도가 느려짐 — 벤치마크: Gemma 3 4B Q4_K_M ~43.6 tok/s, Gemma 4 E2B Q4_K_M ~65.4 tok/s (둘 다 CPU 기준)
- Windows 환경(팀원들의 개발 환경)에서는 이 문제가 재현되는지 별도 확인 필요 — 지금까지는 macOS(Apple Silicon)에서만 확인된 이슈
- 향후 llama.cpp/Metal 백엔드 쪽 업스트림 수정으로 해결될 가능성이 있어, 주기적으로 재검토가 필요함

## 관련 known-issue

자세한 재현 조건과 스택트레이스는 `known-issues.md` 참고.
