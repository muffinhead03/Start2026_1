# Windows에서 Gemma 4 힌트 시스템 디버깅 — 전체 기록

`exp/gemma4-window` 브랜치. Claude Code(Sonnet 5)와 함께 진행한 디버깅 세션 전체
기록입니다. 목표: LlamaLib(포크: `png401/LlamaLib`, 브랜치:
`claude/llamalib-windows-gemma4-debug-z8e50i`)를 llama.cpp b8855(Gemma 4 지원)로
올린 상태에서, macOS에서는 이미 성공했던 Gemma 4 로드/추론을 Windows에서도
되게 만들고, 최종적으로 실제 Unity 게임(이 저장소)에서 검증하는 것.

목차:
1. [빌드 실패 원인 규명 및 수정](#1-빌드-실패-원인-규명-및-수정)
2. [llama.cpp CLI 레벨 검증](#2-llamacpp-cli-레벨-검증)
3. [LlamaLib DLL 직접 검증](#3-llamalib-dll-직접-검증)
4. [C# P/Invoke 레이어 버그 2건 발견 및 수정](#4-c-pinvoke-레이어-버그-2건-발견-및-수정)
5. [커밋](#5-커밋)
6. [Unity 설치 및 프로젝트 오픈](#6-unity-설치-및-프로젝트-오픈)
7. [실제 게임에서 검증 성공](#7-실제-게임에서-검증-성공)
8. [배포 전략](#8-배포-전략)

---

## 1. 빌드 실패 원인 규명 및 수정

### 1.1 환경 세팅

- `png401/LlamaLib`를 `C:\Users\mycom\LlamaLib`에 서브모듈 포함 클론
- `claude/llamalib-windows-gemma4-debug-z8e50i` 브랜치가 원격에 없어서 `main`에서
  새로 생성
- **발견**: `cmake/LLAMALIBConfig.cmake` ↔ `LlamaLibConfig.cmake` (대소문자만 다른
  두 파일)가 저장소에 둘 다 추적되어 있음. macOS/Linux(대소문자 구분 파일시스템)에서는
  문제없지만 Windows NTFS(대소문자 미구분)에서는 두 파일이 물리적으로 충돌함 →
  체크아웃할 때마다 `git status`가 불안정하게 dirty해짐. **이번엔 손대지 않고 보류**
  (다른 작업에 지장 없어서 후순위로 미룸).

### 1.2 첫 번째 원인: git이 시스템 PATH에 없음

CMake로 `win-x64_avx2` configure 시도 → `third_party/CMakeLists.txt`의
`APPLY_PATCH` 함수가 `execute_process(COMMAND git apply ...)`로
`patches/llama.cpp.patch`를 서브모듈에 적용하는데 실패. **CMake가 실제 git
에러 메시지를 감추고 "Failed to apply" 로만 보고**해서 원인 파악이 어려웠음.

`git apply --check`로 직접 실행해보니 패치 자체는 문제없음(exit 0) — 그런데
PowerShell 세션에 `git`이 PATH에 없었음(`Get-Command git` 실패). Git Bash에는
있지만 시스템 PATH엔 없어서, CMake를 실행한 프로세스(PowerShell)에서
`execute_process(COMMAND git ...)`가 그냥 `git`을 못 찾아서 실패한 것.

**수정**: PowerShell 세션에 `$env:Path = "C:\Program Files\Git\cmd;" + $env:Path`
로 git 추가 후 재시도 → 패치 정상 적용, configure 성공.

이 발견은 이후 Unity Hub가 패키지 resolve 중 겪은 것과 **완전히 동일한
증상**(`"No 'git' executable was found"`)의 원형이었음 — 6장 참고.

### 1.3 두 번째 원인 (진짜 빌드 실패): `/utf-8` 플래그가 LlamaLib 자체 타겟에
전파되지 않음

git PATH 문제를 고치고 나니 avx2 빌드가 실제 컴파일 단계까지 진행됐고, 다음
에러로 실패:

```
common/jinja/utils.h(37,38): error C2001: 상수에 줄 바꿈 문자가 있습니다.
common/jinja/utils.h(38,5): error C2146: 구문 오류: ')'이(가) 'output' 식별자 앞에 없습니다.
```

`common/jinja/utils.h:37`을 보니:

```cpp
string_replace_all(substr, "\n", "↵");
```

`"↵"`(U+21B5)는 UTF-8 멀티바이트 리터럴. 이 머신의 활성 Windows 코드페이지는
**949(한국어)**였고 (`chcp` 확인), MSVC는 소스 파일에 UTF-8 BOM이 없으면
narrow 리터럴을 **활성 코드페이지 기준으로 해석**함 — CP949로 이 바이트열을
잘못 해석하면서 문자열 리터럴이 깨져 컴파일 에러가 남.

원인 조사 결과, `third_party/llama.cpp/CMakeLists.txt`에 이미 `/utf-8` 옵션이
있었음:

```cmake
if (MSVC)
    add_compile_options("$<$<COMPILE_LANGUAGE:C>:/utf-8>")
    add_compile_options("$<$<COMPILE_LANGUAGE:CXX>:/utf-8>")
endif()
```

**그런데 이건 `third_party/llama.cpp`의 CMake 디렉토리 스코프에만 적용됨.**
LlamaLib 자신의 타겟(`src/CMakeLists.txt`의 `${LLAMALIB_ARCHITECTURE_LIBRARY}`,
즉 `LLM_service.cpp` 등을 컴파일하는 타겟)은 루트 `CMakeLists.txt`에서 별도로
`add_subdirectory("src")`로 추가되는 **형제 디렉토리**라, `add_compile_options`가
전파되지 않음. `common/jinja/utils.h`는 llama.cpp가 아니라 **LlamaLib가 직접
include하는 헤더**라서 이 문제를 직접 맞음.

**수정**: 루트 `CMakeLists.txt`에도 동일한 `/utf-8` 옵션 추가 (`third_party`
서브디렉토리 추가 전, WIN32 분기 근처).

```cmake
if (MSVC)
    # third_party/llama.cpp sets this for its own subtree, but LlamaLib's own
    # targets (src/, tools/, tests/) are added via a sibling add_subdirectory
    # and don't inherit it.
    add_compile_options("$<$<COMPILE_LANGUAGE:C>:/utf-8>")
    add_compile_options("$<$<COMPILE_LANGUAGE:CXX>:/utf-8>")
endif()
```

이후 3개 아키텍처(`win-x64_avx2`, `win-x64_avx`, `win-x64_noavx`) 모두 정상
빌드 성공 (vcpkg로 OpenSSL을 `x64-windows-static-md` triplet으로 설치해서
링크).

---

## 2. llama.cpp CLI 레벨 검증

빌드된 라이브러리와 별개로, llama.cpp 자체(`llama-completion` CLI, 순정
빌드)로 Gemma 3 vs Gemma 4를 비교 테스트.

| | Gemma 3 (1B) | Gemma 4 (E2B) |
|---|---|---|
| 모델 로드 | 성공 | 성공 |
| `-no-cnv` (템플릿 미적용) | 성공 | 성공 |
| 기본 설정 (conversation 모드, `--jinja` 미지정) | 성공 | **크래시** (exit `-1073740791` = `0xC0000409`) |
| `--jinja` 명시 | (불필요) | **성공** |

**원인 분석**: `--jinja`가 꺼진(기본값) 상태에서는 레거시(non-jinja) 채팅
템플릿 경로(`common_chat_templates_apply_legacy`, `llama_chat_apply_template`)를
탐. `src/llama-chat.cpp`의 `llm_chat_detect_template()`는 `<start_of_turn>`
문자열 유무로 "gemma" 계열을 감지하는데, Gemma 4 템플릿은 `<turn|>`/`<|turn>`
토큰을 써서 이 패턴에 안 걸림 → `LLM_CHAT_TEMPLATE_UNKNOWN` → 음수 반환 →
`throw std::runtime_error("this custom template is not supported, try using
--jinja")`. **`tools/completion/completion.cpp`의 `main()`엔 `try`/`catch`가
전혀 없어서** 이 예외가 처리되지 않은 채 전파 → `std::terminate()` → CRT
`abort()` → Windows에서 `STATUS_STACK_BUFFER_OVERRUN`(0xC0000409) fastfail로
보고됨 (콘솔에 진단 메시지 없이 조용히 죽는 이유).

**결론**: 이건 실재하는 llama.cpp 레벨 버그지만(업스트림에 보고할 가치 있음),
`src/LLM_service.cpp:235`에 `params->use_jinja = true;`가 하드코딩돼 있어서
**LlamaLib 자신은 이 레거시 경로를 절대 타지 않음** → 이 크래시는 LlamaLib
자체와 무관하다고 판단하고 조사 방향 전환.

---

## 3. LlamaLib DLL 직접 검증

llama.cpp CLI가 아니라 **LlamaLib가 실제로 쓰는 경로**(정확히는 Unity가
쓰는 것과 가장 가까운 런타임 디스패치 경로)로 검증.

`build/avx2/libs/Release/llamalib_tests_runtime.exe`를 저장소 루트 바로 밑
임시 폴더(`_test_cwd/`)에서 실행 (`tests/cpp/llamalib_tests.cpp`가
`../tests/model.gguf` 상대경로를 기대하므로).

| | Gemma 3 | Gemma 4 |
|---|---|---|
| CPU 감지 → `llamalib_win-x64_avx2.dll` 동적 로드 | 성공 | 성공 |
| 모델 로드, 서비스 시작 | 성공 | 성공 |
| Context overflow 시나리오(Truncate/Summarize) | 성공 | 성공 |
| 최종 종료 코드 | 127 | 127 (동일 원인) |

두 모델 다 정확히 같은 지점에서 `Assertion failed: tokens[0] == 151644`로
종료. `tests/download_test_models.sh`를 보면 이 테스트의 원래 대상 모델은
**Qwen3-0.6B**이고 `151644`는 Qwen의 `<|im_start|>` 토큰 ID — Gemma 계열
모델을 억지로 넣었으니 이 특정 assertion은 당연히 실패. **모델/LlamaLib
버그가 아니라 테스트 픽스처가 다른 모델을 기대하는 것**이며, 두 모델이
정확히 같은 이유로 "실패"한다는 사실 자체가 Gemma 4가 Gemma 3와 동등하게
정상 작동함을 보여줌.

---

## 4. C# P/Invoke 레이어 버그 2건 발견 및 수정

### 4.1 테스트 인프라 구축

- .NET 8 SDK 설치 (winget)
- `csharp/`에서 `dotnet pack`으로 로컬 nuget 패키지 빌드 (`tests/csharp/NuGet.Config`가
  이미 로컬 패키지 소스를 nuget.org보다 우선하도록 설정돼 있음 — CI가 쓰는
  방식과 동일)
- **삽질**: 로컬 빌드 네이티브 바이너리를 NuGet 캐시(`~/.nuget/packages/LlamaLib/LlamaLib-v2.0.5/`)에
  덮어씌우려다가, 그 폴더가 실제 nuget 패키지 설치 위치(`~/.nuget/packages/llamalib/2.0.5/`,
  Windows는 대소문자 구분 안 함)와 충돌해서 패키지 캐시가 꼬임. `dotnet clean`으로
  초기화 후 재구성하면서 해결 (이 과정에서 실수로 **공식 미수정 v2.0.5**로
  한 번 테스트하게 됐는데, 오히려 "아무 수정 없는 공식 버전"의 깨끗한 재현
  사례가 되어 유용했음).

### 4.2 버그 #1 — `LLMService.FromCommand()` 초기화 순서 버그

`tests/csharp/Tests.cs`의 `Tests_LLMClient()`가 `LLMService.FromCommand("-m " +
modelPath)`를 호출하면 (다른 LlamaLib API를 먼저 호출한 적 없는 "차가운"
프로세스에서) 즉시 `System.NullReferenceException`.

원인 (`csharp/LLMService.cs`):

```csharp
llamaLibInstance = new LlamaLib(LlamaLib.Has_GPU_Layers(paramsString ?? string.Empty));
```

`Has_GPU_Layers`는 static 델리게이트인데, **`LlamaLib`의 생성자 안에서만**
(`LoadRuntimeLibrary()` 호출을 통해) 로드됨. 그런데 위 코드는 그 생성자의
**인자**로 아직 로드되지 않은 `Has_GPU_Layers`를 호출함 — 생성자가 실행되기도
전에 그 생성자가 채워줄 값을 쓰려는 순서 버그. 플랫폼/모델과 무관하게, 이게
프로세스의 첫 LlamaLib 호출이면 무조건 재현됨.

**수정** (`csharp/LlamaLib.cs`, `csharp/LLMService.cs`):
- `LoadRuntimeLibrary()`를 `internal static`으로 변경 (인스턴스 없이도 호출
  가능하게)
- `FindLibrary`의 실제 탐색 로직을 `FindLibraryStatic`이라는 정적 헬퍼로 분리
  (가상 디스패치가 필요 없는 부트스트랩 경로에서 재사용하기 위해; `FindLibrary`
  자체는 여전히 `virtual`이라 하위 클래스 오버라이드 가능성 유지)
- `LLMService.FromCommand()`에서 `Has_GPU_Layers` 호출 전에
  `LlamaLib.LoadRuntimeLibrary();`를 명시적으로 먼저 호출

### 4.3 버그 #2 — DLL 언로드 시 살아있는 OpenMP 스레드 크래시

버그 #1을 고친 뒤 `Tests_LLMService()`(Gemma 4 로드 → Dispose → 임베딩 모델
로드)를 돌리면 **다른 크래시**가 반복 재현됨: 아무 콘솔 출력도 없이 테스트
호스트 프로세스가 즉시 죽음.

Windows 이벤트 뷰어(`Get-WinEvent`)로 실제 크래시 덤프 확인:

```
오류 있는 모듈 이름: VCOMP140.DLL_unloaded
예외 코드: 0xc0000005 (access violation)
오류 오프셋: 0x000000000000455e   ← 매번 정확히 동일
```

`VCOMP140.DLL_unloaded`(이미 **언로드된** 상태의 모듈 이름)에서, 매번 동일한
오프셋으로 access violation — 랜덤 메모리 손상이 아니라 **"이미 unload된
DLL의 코드를 실행하려는 스레드가 있다"**는 결정론적 신호.

`csharp/LlamaLib.cs`의 `Dispose()`와 `TryNextLibrary()`가 아키텍처별 네이티브
DLL(`llamalib_win-x64_avxN.dll`)에 대해 `FreeLibrary`를 호출하는데, **그 DLL이
띄운 스레드가 완전히 멈췄는지 전혀 확인하지 않음**.

**왜 Windows에서만 문제가 되는가** (macOS에선 안 그랬던 이유): ggml의 CPU
백엔드는 `find_package(OpenMP)`가 성공하면 `GGML_USE_OPENMP`를 켜고 OpenMP로
병렬화함 (`ggml/src/ggml-cpu/CMakeLists.txt`). MSVC에서는 이게 항상 성공해서
시스템 공유 DLL `vcomp140.dll`의 **영구적이고 명시적으로 종료 안 되는**
워커 스레드 풀을 씀 (MSVC의 OpenMP 런타임은 옛날 2.0 스펙만 지원해서, 스레드
풀을 정지시키는 `omp_pause_resource_all` 같은 최신 API 자체가 없음). 반면
macOS 기본 Xcode Clang은 별도 libomp 설치 없인 `-fopenmp`를 지원 안 해서
`find_package(OpenMP)`가 실패하고, ggml은 **자기 자신이 명시적으로 만들고
파괴하는 pthread 기반 스레드풀**로 자동 폴백함 (`ggml-cpu.c`의
`#ifndef GGML_USE_OPENMP` 분기) — 이 스레드들은 `LLM_Delete` 등이 리턴하는
시점에 이미 다 join되어 있어서, 그 뒤에 dylib을 언로드해도 안전함.

**수정**: `Dispose()`와 `TryNextLibrary()`에서 아키텍처별 네이티브 DLL에 대한
`FreeLibrary` 호출을 제거 (핸들 값은 여전히 `IntPtr.Zero`로 리셋해서 C# 쪽
상태 추적은 유지). 프로세스 수명 동안 DLL이 메모리에 계속 남는 대가(~12MB)로,
결정 불가능한 크래시를 원천 차단. 이 fix는 native 재빌드가 필요 없는 순수
C# 변경이라 빠르게 적용 가능했음.

### 4.4 검증

두 수정 반영 후 전체 테스트 스위트 재실행 — **4개 테스트 전부 통과**:

```
통과 Tests_LLMService [13 s]
통과 Tests_LLMClient [12 s]
통과 Tests_LLMRemoteClient [18 s]
통과 Tests_LLMAgent [14 s]
```

Gemma 4로 LLM_Start, Tokenize, Detokenize, Completion(스트리밍/비스트리밍),
Slot 저장/복원, Cancel, Lora 목록, 임베딩, 원격 클라이언트, 히스토리
관리/채팅까지 전부 정상.

---

## 5. 커밋

`C:\Users\mycom\LlamaLib` (브랜치 `claude/llamalib-windows-gemma4-debug-z8e50i`)에
4개 파일 변경사항 커밋:

- `CMakeLists.txt` — `/utf-8` 전파 수정
- `patches/llama.cpp.patch` — jinja 댕글링 레퍼런스 수정 diff **추가**
  (서브모듈은 커밋 시점에 직접 수정하지 않고, `third_party/CMakeLists.txt`의
  `APPLY_PATCH`가 빌드 때마다 이 패치 파일을 서브모듈에 자동 적용하는 방식이라,
  패치 파일 쪽에 반영하는 게 이 저장소의 기존 관례와 맞음). 패치가 pristine
  서브모듈 체크아웃에 깨끗이 적용되는지 `git apply --check`로 검증 완료.
- `csharp/LLMService.cs`, `csharp/LlamaLib.cs` — 4.2, 4.3의 C# 수정

주의: 서브모듈 자체(`third_party/llama.cpp`)의 "modified content"는 커밋
대상이 아님 — 원래부터 빌드 시점에 패치가 적용되는 설계라 서브모듈 워킹
디렉토리가 항상 dirty한 게 정상.

이후 `git push origin claude/llamalib-windows-gemma4-debug-z8e50i` — **머지나
PR 없이 브랜치만 원격(`png401/LlamaLib`)에 푸시**. `main`은 그대로 두고
(macOS는 계속 `main` 기준), 이 브랜치가 Windows용 수정사항을 담는 구조.

---

## 6. Unity 설치 및 프로젝트 오픈

여기서부터는 LlamaLib 자체가 아니라, **실제 게임(이 저장소)에서 검증**하기
위한 환경 구축 과정. 이 머신엔 Unity가 전혀 설치돼 있지 않았음.

### 6.1 Unity Hub / Editor 설치

- `winget install Unity.UnityHub` (MSIX 패키지로 설치됨,
  `C:\Program Files\WindowsApps\...\Unity Hub.exe`)
- `Unity Hub.exe -- --headless install --version 6000.3.11f1 --changeset
  3000ef702840` (프로젝트의 `ProjectSettings/ProjectVersion.txt` 기준)
- Wi-Fi 환경이라 다운로드(~3.9GB)가 매우 느렸음 (여러 시간 소요, 중간에 중복
  다운로드가 생기는 등 Hub CLI 자체의 버그도 있었음)
- **실수**: 다운로드 완료 후 설치(압축 해제) 프로세스가 아직 진행 중인
  상태에서, 메모리 확보를 위해 Unity Hub 프로세스를 강제 종료하면서 **설치
  프로그램(`UnitySetup64-6000.3.11f1.exe`)까지 같이 죽여버림** → 불완전한
  설치 (PackageManager 서버 exe 등 핵심 파일 누락, `Could not find Unity
  Package Manager local server application` 에러로 나중에 드러남)
- 원인 조사 중 **이 세션이 관리자 권한이 아님**을 확인 → `Program Files`
  아래 쓰기가 애초에 실패했을 가능성도 있었음
- **해결**: 캐시된 설치 파일(`UnitySetup64-6000.3.11f1.exe`)을 직접
  `/S /D=C:\Users\mycom\UnityEditors\6000.3.11f1` (관리자 권한 불필요한
  사용자 폴더)로 silent 재설치, 이번엔 **프로세스를 절대 건드리지 않고
  자연 종료까지 대기** (파일 개수/용량을 주기적으로 폴링하며 확인, 총
  49,332개 파일까지 완료 확인)
- Unity Hub CLI의 `editors --add`가 로그상 명령은 받았지만 실제 등록에
  실패하는 버그 발견 (`0 located editors found in storage`) → 사용자가
  Hub GUI에서 직접 "Locate a version"으로 등록

### 6.2 프로젝트 열기 중 발생한 에러들

**① `No 'git' executable was found`** — 패키지 resolve 중
`ai.undream.llm`(git URL 의존성)이 git을 요구하는데 시스템 PATH에 없음
(1.2절과 같은 근본 원인, 다른 프로세스에서 재발). `[Environment]::
SetEnvironmentVariable("Path", ..., "User")`로 `C:\Program Files\Git\cmd`를
**영구** 추가. 단, **이미 실행 중이던 Unity Hub 프로세스는 예전 환경변수를
그대로 물고 있어서**, Hub와 그 자식 프로세스(Unity Editor)를 전부 강제
종료하고 Hub를 새로 띄워야 실제로 적용됨.

**② `Library\Bee\artifacts\...\*.mvfrm` FileNotFoundException** — 첫
임포트 중 병렬 빌드 프로세스 간 타이밍 레이스로 추정. 재시도로 자연
해결됨.

**③ `Project folder or disk is read only`** — 프로젝트가
`Documents\2026_Start_Project\Start2026_1`에 있었는데, **Windows Defender의
"제어된 폴더 액세스"**(기본 활성화된 랜섬웨어 방지 기능)가 `Documents`
폴더에 대한 쓰기를 비인가 프로세스로부터 차단하고 있었음. `Get-MpPreference`로
`EnableControlledFolderAccess = 1` 확인, 이 세션엔 관리자 권한이 없어서
`Add-MpPreference`도 실패 → 사용자가 Windows 보안에서 직접 껐음.

### 6.3 프로젝트 폴더 이동 (사고 및 복구)

Documents 밖(`C:\Users\mycom\UnityProjects\Start2026_1`)으로 옮기기로 결정.
`Move-Item`이 GitKraken이 파일 핸들을 잡고 있어서 중간에 실패 → **`.git`
폴더가 소스/목적지에 걸쳐 반쪽씩 나뉘는 사고 발생** (`objects`/`logs`는
소스에 남고 나머지 메타데이터만 목적지로 이동됨, `git status`가
`"fatal: not a git repository"`로 깨짐).

**복구**: `robocopy <소스> <목적지> /E /MOVE /R:2 /W:1`로 나머지 전체를
병합 이동 (38,165개 파일, 실패 0건). robocopy는 디렉토리 트리를 제대로
병합하므로 `.git/objects`, `.git/logs`도 목적지의 기존 골격과 정상
합쳐짐. 이후 `git status`/`git log`로 커밋 히스토리가 전부 살아있음을
확인. 이 과정에서 생긴 이중 중첩 폴더(`UnityProjects\Start2026_1\Start2026_1\`)는
한 단계 평탄화해서 정리.

### 6.4 프로젝트 안에 들어있던 `LlamaLib/` 폴더 정리

프로젝트 루트에 (제가 작업하던 것과는 별개로) `LlamaLib/`라는 자체 git
저장소가 통째로 들어있었음 — `png401/LlamaLib`의 **수정 안 된 `main`
브랜치** 클론. `.gitignore`에 이 경로가 전혀 안 걸려있어서, `git status`에
"Untracked: LlamaLib/"로 뜨는 상태였고, `git add .`나 GitKraken "Stage All"을
누르면 통째로 게임 저장소에 커밋될 뻔한 위험이 있었음 (사용자가 직접
포착한 문제).

이미 `C:\Users\mycom\LlamaLib`에 수정+빌드까지 끝난 진짜 작업 사본이
있었으므로, 프로젝트 안의 중복 클론은 **그냥 삭제**하기로 결정. (`.gitignore`에
추가하는 방안도 잠깐 적용했다가, 폴더 자체를 지우면서 불필요해져서 되돌림.)

---

## 7. 실제 게임에서 검증 성공

프로젝트가 정상적으로 열린 뒤:

1. `Assets/StreamingAssets/LlamaLib-v2.0.5/win-x64/native/`의 4개 DLL
   (`llamalib_win-x64_avx2/avx/noavx/runtime.dll`)을 `C:\Users\mycom\LlamaLib\build\`의
   수정된 빌드로 교체 (공식 버전은 LLMUnity가 첫 임포트 때 자동 다운로드한
   것)
2. `gemma-4-E2B-it-Q4_K_M.gguf`를 `Assets/StreamingAssets/`에 배치
3. `Assets/Prefabs/AI_canvas/LLM_Manager.prefab`의 `_model` 필드를
   `gemma-3-4b-it-Q4_K_M.gguf` → `gemma-4-E2B-it-Q4_K_M.gguf`로 변경
   (`_contextSize: 8192`, `_numGPULayers: 0`, `maxContextLength: 131072`는
   기존 값 그대로 — Gemma 4 네이티브 컨텍스트와 이미 맞아떨어짐)

`Scene2_Again.unity`에서 Play → 게임 내 무전기로 힌트 요청 → 실제 파이프라인
(`HintManager` → `LLMUnity.LLMAgent.Chat` → `UndreamAI.LlamaLib.LLMAgent.ChatAsync`)을
그대로 통과해서:

```
[힌트 결과] 모델: gemma-4-E2B-it-Q4_K_M.gguf / 레벨: 1 / 상태: 단서 미이해 /
응답시간: 23134ms / 응답: 낡은 악기의 뼈대 주변을 유심히 들여다보시오.
그 속에 숨겨진 진실이 당신을 기다리고 있을 것입니다.
```

예외 없이, 실제 맥락 있는 한국어 힌트 생성 확인. **Windows에서 Gemma 4가
실제 게임 플레이 경로로 정상 작동함을 최종 확인.**

---

## 8. 배포 전략

다른 Windows 팀원, 그리고 향후 CI/배포에 이 수정사항을 어떻게 반영할지
논의.

### 8.1 관련 저장소 관계 확인

- `undreamai/LlamaLib` (네이티브 라이브러리 원본) ← `png401/LlamaLib`
  (사용자의 포크, `fork: true` / `parent: undreamai/LlamaLib` GitHub API로
  확인) — 오늘의 모든 수정사항이 있는 곳
- `undreamai/LLMUnity` (Unity 패키지 래퍼, `ai.undream.llm`) — **포크 안 됨**,
  이 프로젝트의 `Packages/manifest.json`이 공식 저장소를 `v3.0.3` 태그로
  직접 참조 중

### 8.2 결정: 브랜치 푸시만, 머지/PR 없음

`png401/LlamaLib`에 `claude/llamalib-windows-gemma4-debug-z8e50i` 브랜치를
푸시만 하고 `main`엔 머지하지 않기로 결정. 의도: **macOS는 계속 `main`
기준으로, Windows는 이 브랜치 기준으로** 운용.

### 8.3 발견: LLMUnity 다운로드 URL이 `undreamai` 조직으로 하드코딩됨

```csharp
// Library/PackageCache/ai.undream.llm@.../Runtime/LLMUnitySetup.cs
public static string LlamaLibVersion = "v2.0.5";
LlamaLibReleaseURL = "https://github.com/undreamai/LlamaLib/releases/download/{LlamaLibVersion}"
```

즉 `png401/LlamaLib`에 릴리즈를 새로 만들어도 LLMUnity는 그걸 절대 참조하지
않음 — 항상 `undreamai/LlamaLib`의 릴리즈만 봄. **"우리 포크에 릴리즈 만들기"만으론
자동 배포가 안 됨.**

진짜 자동화하려면:
1. `undreamai/LLMUnity`도 별도로 포크(예: `png401/LLMUnity`)해서 이 URL을
   패치하고, `Packages/manifest.json`도 그 포크를 보게 바꾸기 (관리해야 할
   포크가 LlamaLib + LLMUnity 2개로 늘어남), 또는
2. `undreamai/LlamaLib`(진짜 원본)에 정식 PR을 올려서 메인테이너가 새 공식
   릴리즈를 내주길 기다리기 (가장 "제대로 된" 방법, 외부 의존적이라 느림)

### 8.4 현재 채택한 방식: 수동 적용 스크립트 (`Tools/`)

당장 팀원들이 쓸 수 있도록:
- `Tools/LlamaLib-win-custom-build/win-x64/` — 수정된 빌드 4개 DLL을 그대로
  저장소에 커밋 (총 ~41MB, 팀 공유 목적으로는 충분히 작음 — 매번 각자
  Visual Studio + vcpkg로 처음부터 빌드하게 하는 것보다 훨씬 마찰이 적음)
- `Tools/setup-custom-llamalib-win.ps1` — 위 DLL들을
  `Assets/StreamingAssets/LlamaLib-v2.0.5/win-x64/native/`로 복사하는
  원클릭 스크립트
- `Tools/WINDOWS_GEMMA4_SETUP.md` — 팀원용 단계별 가이드 (이 문서보다
  간결하고 실행 중심)

계획: 이 방식으로 팀원들이 모두 정상 동작 확인하면, 그다음 8.3의 1번(LLMUnity
포크) 또는 2번(업스트림 PR)으로 자동화 진행.
