# Third Party Notices

이 프로젝트는 아래의 서드파티 소프트웨어/모델을 사용한다. 각 항목의 라이선스 조건을 반드시 확인할 것.

---

## LLM for Unity (LLMUnity)

- **제공처**: [undreamai/LLMUnity](https://github.com/undreamai/LLMUnity)
- **라이선스**: Apache License 2.0
- **사용 방식**: 로컬 LLM(Gemma) 추론을 Unity 안에서 직접 구동하기 위한 통합 레이어로 사용
- **요구사항**: 저작권 고지 및 라이선스 사본 포함, 변경 사항 명시(수정한 경우)

---

## llama.cpp (LlamaLib을 통해 사용)

- **제공처**: [ggml-org/llama.cpp](https://github.com/ggml-org/llama.cpp)
- **라이선스**: MIT License
- **사용 방식**: LLMUnity의 백엔드(LlamaLib, C++/C# 래퍼)가 내부적으로 사용하는 추론 엔진
- **요구사항**: 저작권 고지 및 라이선스 사본 포함

```
MIT License

Copyright (c) 2023-2026 The ggml authors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

## Gemma 3 (프로덕션 모델)

- **제공처**: Google
- **라이선스**: [Gemma Terms of Use](https://ai.google.dev/gemma/terms) (표준 오픈소스 라이선스 아님, Google 자체 약관)
- **사용 방식**: 게임 내 힌트 생성용 온디바이스 LLM (Gemma 3 4B Q4_K_M GGUF)
- **요구사항 (본 프로젝트가 준수해야 하는 것)**:
  - ✅ 배포 시 Gemma 사용약관 사본 또는 링크 제공 (본 문서 및 게임 내 고지에서 제공)
  - ✅ **"Built with Gemma" 표기** 포함 (게임 크레딧/설정 화면에 명시 필요 — 아직 미반영, TODO)
  - ✅ [Gemma Prohibited Use Policy](https://ai.google.dev/gemma/prohibited_use_policy) 준수
  - ✅ 모델을 수정한 경우, 수정 사실을 명확히 표시 (본 프로젝트는 파인튜닝 없이 원본 GGUF 양자화 모델만 사용하므로 해당 없음)
  - ⚠️ Google이 Gemma를 업데이트할 경우, 합리적인 범위 내에서 최신 버전을 사용하도록 노력해야 함(약관 4.1항)

---

## Gemma 4 (실험용 모델)

- **제공처**: Google
- **라이선스**: Apache License 2.0
- **사용 방식**: 개인 실험 브랜치(`nancypark/LlamaLib`, `exp/gemma4-test`)에서 벤치마크 목적으로만 사용, 프로덕션 미반영
- **비고**: Gemma 4부터 표준 Apache 2.0으로 전환되어 Gemma 3 대비 배포 요건이 단순함(별도 "Built with Gemma" 표기 의무 없음). 추후 프로덕션에 Gemma 4를 채택할 경우 이 섹션의 요구사항이 훨씬 가벼워짐.

---

## Steam 배포 시 추가로 확인할 것

- 게임 설정/크레딧 화면에 "Built with Gemma" 문구와 Gemma 사용약관 링크를 노출하는 UI를 추가해야 함 (현재 미구현, 제작설계서/개발보고서 작성 전 반영 필요)
- 모델 파일(GGUF) 자체는 이 리포지토리에 커밋하지 않으며, 별도 다운로드 스크립트를 통해 배포됨
