<div align="center">
  
<br/>

![게임 로고](images/game_logo.png)

# Do not open this box.

### 상자를 열면 기이한 현상이 일어나는 골동품 상점에서 탈출하는 생존형 공포 게임.

![Horror](https://img.shields.io/badge/공포-3498DB?style=for-the-badge)
![1st Person](https://img.shields.io/badge/1인칭-3498DB?style=for-the-badge)
![Single Player](https://img.shields.io/badge/싱글플레이어-3498DB?style=for-the-badge)
![Escape](https://img.shields.io/badge/방탈출-3498DB?style=for-the-badge)
![Survival](https://img.shields.io/badge/생존-3498DB?style=for-the-badge)
![Anomaly](https://img.shields.io/badge/이상현상-3498DB?style=for-the-badge)

<br/>

2026-1학기 캡스톤 디자인과 창업 프로젝트 | **팀 규교굥 (Team 5)**  

지도교수 : 윤명국 교수님 | 이화여자대학교 컴퓨터공학과

<br/>

</div>

---

# 게임 소개

## 📦"금지된 상자, 당신이라면 상자를 여시겠습니까?"
당신에겐 할머니께서 남기신 골동품 상점이 있습니다.<br/>
그곳엔 한 오래된 신비로운 상자가 있습니다.<br/>
그리고 유일한 규칙.<br/>
**"무슨 일 있어도 그 상자를 열어보면 안 돼."**<br/>
할머니께서는 마지막까지도 그 사실을 당부하셨습니다.<br/>
당신은 이 상자를 여시겠습니까?


## 🔑"상자 안에 남은 마지막 희망, 탈출구를 찾으세요."
금지된 상자가 열린 순간, 상점 안의 모든 규칙과 상식은 산산조각 났습니다.<br/>
기괴한 환영, 스스로 움직이는 골동품, 그리고 당신을 위협하는 존재들.<br/>
판도라의 상자에서 빠져나온 재앙들이 당신을 향해 손을 뻗습니다.<br/>
이제 당신은 이 끔찍한 공간에서 살아남아야 합니다.<br/>
숨겨진 퍼즐을 풀고, 곳곳에 깔린 함정을 피해 탈출구를 찾아내세요.<br/>
가장 절망적인 상황에서 탈출한 끝에, 당신은 희망을 맞이할 것입니다.

<br/>

---

# 주요 기능

- **플레이어별 맞춤형 AI 힌트 제공:** 플레이어의 스테이지 진행 상황에 따라 Ollama를 활용한 텍스트 형식의 힌트 생성.
- **6가지 탈출 맵과 기믹:** 총 6가지 스테이지가 있으며 각 스테이지마다 다른 기믹과 상호작용을 구현.
<br/>

---
# 스크린샷

| 게임 플레이 | AI 힌트 UI | AI 힌트 생성 중 |
|:-----------:|:----------:|:--------------:|
| ![gameplay](images/game_play.png) | ![hint_ui](images/game_hint_ui.png) | ![hint_ai](images/hint_ai_announcing.png) |

---

# 데모 영상

[데모 영상 보기](https://www.youtube.com/watch?v=JZrWJRSucn8)

---

# AI 생성 콘텐츠 사용 공개

- 이 게임에 사용된 일부 그래픽 에셋은 AI를 사용해 만들어졌습니다.
- 게임 플레이 중 AI는 플레이어의 진행 기록을 바탕으로 일부 텍스트를 생성합니다.

<br/>

---
# 설치 및 실행 방법
실행 환경

Unity 6000.3.11f1
OS: Windows 10 이상 / macOS 12 이상

게임 실행 방법 및 빌드파일 위치

빌드 파일은 레포지토리 최상단의 `BuildFiles/Start2026_1` 폴더에 있습니다.
실행하려면 GitHub에서 `BuildFiles` → `Start2026_1` 순서로 들어간 뒤, 폴더 안의 실행 파일을 실행하면 됩니다.

이 레포지토리의 Builds/ 폴더로 이동
운영체제에 맞는 파일 실행:

- Windows: Start2026_1.exe
- macOS: Start2026_1.app
단, Unity 빌드 특성상 `Start2026_1_Data` 폴더와 실행 파일이 같은 위치에 있어야 하므로 폴더 구조를 변경하지 않는 것을 권장합니다.

처음 따라해보는 분은 Self_demo.md를 참고하세요.

<br/>


---

# 기술 소개

| Category | Tech Stack |
| :--- | :--- |
| **Engine** | ![Unity](https://img.shields.io/badge/Unity_3D-100000?style=for-the-badge&logo=unity&logoColor=white) |
| **Language** | ![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white) |
| **AI** | ![Ollama](https://img.shields.io/badge/Ollama_Gemma3-000000?style=for-the-badge&logo=ollama&logoColor=white) |
| **Platform** | ![Windows](https://img.shields.io/badge/Windows-0078D4?style=for-the-badge&logo=windows&logoColor=white) ![MacOS](https://img.shields.io/badge/MacOS-000000?style=for-the-badge&logo=apple&logoColor=white) |
| **Rendering** | ![URP](https://img.shields.io/badge/Unity_URP-5E5E5E?style=for-the-badge&logo=unity&logoColor=white) |

<br/>

---

# 시스템 구조

![시스템 구조](images/system_architecture.png)

플레이어가 무전기로 힌트를 요청하면 Unity가 로컬 Ollama 서버에 HTTP 요청을 보냅니다.
플레이어의 진행 상황(체류 시간, 발견 단서, 실패 횟수)을 분석해 1~5단계 맞춤형 힌트를 생성합니다.

# 레포지토리 구조

```text
📂 Start2026_1/
├── 📂 Assets/                            # 유니티 프로젝트 에셋
│   ├── 📂 Animation/                     # 애니메이션 클립 및 컨트롤러
│   ├── 📂 ArtSource/                     # 아트 원본 소스 파일
│   ├── 📂 BlenderAssetTest/              # 블렌더 에셋 연동 테스트 폴더
│   ├── 📂 FontCollector/                 # 폰트 리소스 모음
│   ├── 📂 Material/                      # 3D 모델 머티리얼 및 텍스처
│   ├── 📂 Meshes/                        # 3D 메시 파일
│   ├── 📂 MobileDependencyResolver/      # 모바일/외부 패키지 종속성 관리
│   ├── 📂 Music/                         # 배경음악 및 오디오 파일
│   ├── 📂 Prefabs/                       # 게임 내 재사용 가능한 프리팹 오브젝트
│   ├── 📂 Resources/                     # 런타임 동적 로드용 리소스
│   ├── 📂 Scenes/                        # 게임 씬
│   ├── 📂 Scripts/                       # C# 스크립트
│   ├── 📂 Settings/                      # 유니티 환경 및 렌더링 설정 (URP 등)
│   ├── 📂 Sounds/                        # 효과음 파일
│   ├── 📂 TextMesh Pro/                  # TextMesh Pro 폰트 및 설정
│   ├── 📂 TimeLine/                      # 타임라인 애니메이션 데이터
│   └── 📂 _Recovery/                     # 복구용 백업 파일
├── 📂 BlenderPractice/                   # 블렌더 3D 모델링 작업 및 연습 파일
├── 📂 doc/                               # 프로젝트 관련 문서 및 발표 자료
├── 📂 experiments/
│   └── 📂 ollama-experiments/            # Ollama AI 힌트 시스템 실험 및 결과 데이터
├── 📂 images/                            # README에 사용되는 이미지 파일
├── 📂 Packages/                          # 유니티 패키지
├── 📂 ProjectSettings/                   # 유니티 프로젝트 설정
├── INDUSTRY_TRACK.md                     # 경쟁 게임 분석 및 차별점 정리
├── Ideation.MD                           # 게임 기획 및 아이디어 정리 문서
├── ProjectDescription.md                 # 프로젝트 상세 설명 문서
├── README.md                             # 레포지토리 메인 소개 문서
├── Self_demo.md                          # 게임 시연 가이드
├── Start2026_1.slnx                      # 프로젝트 솔루션 파일
├── index.html                            # 웹 호스팅용 메인 인덱스 페이지
└── Team_Ground_Rule.md                   # 팀 협업 규칙 및 그라운드 룰 문서
```

<br/>


# 팀원 소개
[![Contributors](https://contrib.rocks/image?repo=muffinhead03/Start2026_1)](https://github.com/muffinhead03/Start2026_1/graphs/contributors)

| 이름 | 역할 |
|------|------|
| 정혜교 | PM, Unity 씬 개발, Blender 3D 에셋 제작 |
| 윤민주 | 기획, Unity 씬 개발, 레벨 디자인 |
| 박남규 | AI 힌트 시스템 개발, Unity 씬 개발 |
<br/>

---

# AI 투명성 리포트

| AI 도구 | 활용 내역 | 인간 검수 여부 |
|---------|----------|--------------|
| Claude | 힌트 시스템 스크립트 초안 작성 (HintManager, PromptBuilder 등) | ✅ 팀원이 직접 검토 후 수정 |
| ChatGPT | 기획 보조, 문서 초안 작성 | ✅ 팀원이 직접 검토 후 수정 |
| Ollama (gemma3:4b) | 게임 내 플레이어 맞춤형 힌트 실시간 생성 | ✅ 프롬프트 설계 및 결과 검증 |
| Blender AI 기능 | 일부 3D 에셋 생성 보조 | ✅ 팀원이 직접 수정 |

> AI 출력물은 모두 팀원이 직접 검토하고 수정했습니다.
> 핵심 게임 로직, 퍼즐 설계, 레벨 디자인은 팀원이 직접 구현했습니다.
