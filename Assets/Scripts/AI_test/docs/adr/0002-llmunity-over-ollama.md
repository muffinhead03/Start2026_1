# 0002. LLM 백엔드로 Ollama 대신 LLMUnity(로컬 GGUF)를 채택한다

## Context

로컬 LLM을 게임에 내재화하는 방법은 여러 가지가 있었다. 초기 프로토타입은 `OllamaClient`(별도로 실행 중인 Ollama 서버에 HTTP 요청을 보내는 방식)로 만들었다.

Ollama 방식의 문제:
- **별도 프로세스 의존**: 플레이어가 게임을 실행하기 전에 Ollama 서버를 따로 켜둬야 함. Steam 배포 시 최종 사용자에게 이런 사전 설치/실행을 요구하는 건 비현실적.
- **배포 복잡도**: 게임 하나 설치하면 끝이 아니라, Ollama 설치 + 모델 다운로드 + 서버 실행까지 사용자가 직접 해야 함.
- **스트리밍 UX**: 힌트가 한 번에 완성되어 나오기보다, 무전기 UI 특성상 점진적으로("치지직... 교신 중...") 나오는 게 자연스러운데 이 구현체는 완료 콜백만 있었음.

## Decision

**[LLMUnity](https://github.com/undreamai/LLMUnity) 패키지(`undream.llmunity`)를 통해 llama.cpp 기반 GGUF 모델을 Unity 프로세스 안에서 직접 구동한다.**

`LLMClient`가 `LLMCharacter`(LLMUnity 제공)를 감싸서, 게임 실행과 동시에 모델이 로드되고(`Warmup`), 별도 서버·프로세스 없이 인게임에서 바로 추론이 이루어진다. 스트리밍(`onChunk`/`onComplete`)도 기본 지원되어 무전기 UI의 점진적 텍스트 출력과 자연스럽게 맞아떨어진다.

## Consequences

**장점**
- 최종 사용자는 게임만 설치하면 됨(Ollama 등 별도 설치 불필요) → Steam 배포에 적합
- 스트리밍 UX 확보
- 인터넷 연결 없이도 완전히 동작 (성과공유회 현장처럼 네트워크가 불안정한 환경에도 안정적)

**트레이드오프**
- 모델 파일(GGUF, 수 GB)을 게임 빌드에 포함하거나 별도 다운로드시켜야 함(리포지토리에 직접 커밋하지 않음 — Git LFS 또는 별도 배포)
- Unity 프로세스 안에서 추론이 돌아가므로, Unity의 스레딩 모델(Mono ThreadPool)과 llama.cpp 네이티브 라이브러리 간 충돌 가능성이 있음 (→ 0003 참고)
- `OllamaClient.cs`는 실험적 대안 구현으로 코드베이스에 남아 있으나 프로덕션에서는 사용하지 않음
