# Third Party Notices

이 프로젝트는 아래의 서드파티 소프트웨어/모델을 사용한다. 각 항목의 라이선스 조건을 반드시 확인할 것.
빌드에 같이 나가는 고지 파일은 프로젝트 루트의 `Licenses/` 폴더에 있고, `Assets/Editor/CopyLicensesPostBuild.cs`가 빌드할 때마다 빌드 폴더 옆으로 복사한다.

---

## LLM for Unity (LLMUnity) v3.0.2

- **제공처**: [undreamai/LLMUnity](https://github.com/undreamai/LLMUnity)
- **라이선스**: Apache License 2.0
- **사용 방식**: 로컬 LLM(Gemma) 추론을 Unity 안에서 직접 구동하기 위한 통합 레이어
- **요구사항**: 라이선스 사본 + 저작권 고지 포함 (`Licenses/LICENSE-Apache-2.0.txt`, `Licenses/NOTICE.txt`)
- **비고**: 수정한 포크(`png401/LLMUnity`)를 프로덕션 빌드에 쓰게 되면, 수정한 파일에 수정 사실을 표시해야 함(Apache 2.0 4(b)). 현재 `Packages/manifest.json`은 원본 `v3.0.2`라 해당 없음

---

## LlamaLib v2.0.4

- **제공처**: [undreamai/LlamaLib](https://github.com/undreamai/LlamaLib)
- **라이선스**: Apache License 2.0
- **사용 방식**: LLMUnity의 네이티브 백엔드 (`Assets/StreamingAssets/LlamaLib-v2.0.4/`)
- **요구사항**: 라이선스 사본 + 저작권 고지 포함

---

## llama.cpp (LlamaLib 안에서 사용)

- **제공처**: [ggml-org/llama.cpp](https://github.com/ggml-org/llama.cpp)
- **라이선스**: MIT License (Copyright (c) 2023-2026 The ggml authors)
- **사용 방식**: LlamaLib이 내부적으로 쓰는 추론 엔진
- **요구사항**: 저작권 고지 + MIT 전문 포함 (`Licenses/LICENSE-llama.cpp-MIT.txt`)

---

## Vulkan Loader (`vulkan-1.dll`)

- **제공처**: [KhronosGroup/Vulkan-Loader](https://github.com/KhronosGroup/Vulkan-Loader)
- **라이선스**: Apache License 2.0
- **사용 방식**: LlamaLib Vulkan 백엔드와 같이 배포됨

---

## Gemma 3 (프로덕션 모델)

- **제공처**: Google (양자화 파일: [lmstudio-community/gemma-3-4b-it-GGUF](https://huggingface.co/lmstudio-community/gemma-3-4b-it-GGUF), `gemma-3-4b-it-Q4_K_M.gguf`)
- **라이선스**: [Gemma Terms of Use](https://ai.google.dev/gemma/terms) (2026-04-01 개정판 기준, Google 자체 약관)
- **사용 방식**: 게임 내 힌트 생성용 온디바이스 LLM, `includeInBuild: true`로 빌드에 포함되어 배포됨
- **요구사항 (약관 3.1·3.2·4.2항)**:
  - NOTICE 텍스트 파일에 다음 문장을 그대로 포함: "Gemma is provided under and subject to the Gemma Terms of Use found at ai.google.dev/gemma/terms" → `Licenses/NOTICE.txt`
  - 받는 사람에게 약관 사본 제공 → `Licenses/Gemma_Terms_of_Use.txt` (출시 전 약관 원문 붙여넣기)
  - [Gemma Prohibited Use Policy](https://ai.google.dev/gemma/prohibited_use_policy)를 사용 조건에 포함하고 사용자에게 고지 → `Licenses/THIRD_PARTY_TERMS.txt` + 게임 내 라이선스 화면
  - 모델을 수정했으면 수정 사실 표시 → 파인튜닝 없이 원본 GGUF 사용이라 해당 없음
  - Google 상표·로고 사용 권리 없음, 보증 관계로 오해하게 하면 안 됨 → **로고 사용 금지**, 글자 크레딧만
- **정정**: 예전 버전 문서의 "Built with Gemma 표기 필수"는 현재 약관에 없는 조항이라 삭제함 (필수 표기는 NOTICE 문장 하나)

---

## Gemma 4 (실험용 모델)

- **제공처**: Google
- **사용 방식**: 개인 실험 브랜치(`png401/LlamaLib`, `exp/gemma4-*`)에서 벤치마크 목적으로만 사용, 프로덕션 미반영
- **주의**: `Assets/StreamingAssets/LLMManager.json`에 `gemma-4-E2B-it-Q4_K_M.gguf`가 `includeInBuild: true`로 남아 있음 → 출시 빌드 전에 `false`로 바꾸거나 항목 제거 (안 쓰는 모델이 빌드에 들어감)

---

## Steam 배포 시 확인할 것

- [ ] `Licenses/` 폴더가 빌드 폴더에 복사됐는지 (빌드 로그 `[CopyLicensesPostBuild]`)
- [ ] `Gemma_Terms_of_Use.txt`에 약관 원문을 붙여넣었는지
- [ ] 게임 내 크레딧/설정에 라이선스 문구 (한·영·일)
- [ ] Steamworks AI 콘텐츠 공개: Live-Generated(힌트) + 가드레일 설명, Pre-Generated(Tripo AI 3D 에셋)
- [ ] 모델 파일(GGUF)은 리포지토리에 커밋하지 않음
