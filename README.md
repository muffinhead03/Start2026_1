<div align="center">
  
<br/>

![게임 로고](images/game_logo.png)

# 상점 광내기: Polished Madness

### 생성형 AI NPC와 흥정하는 골동품 가게 운영 시뮬레이션 게임

![Simulation](https://img.shields.io/badge/시뮬레이션-3498DB?style=for-the-badge)
![1st Person](https://img.shields.io/badge/1인칭-3498DB?style=for-the-badge)
![Single Player](https://img.shields.io/badge/싱글플레이어-3498DB?style=for-the-badge)
![Horror](https://img.shields.io/badge/공포-3498DB?style=for-the-badge)
![Management](https://img.shields.io/badge/경영-3498DB?style=for-the-badge)
![Antique](https://img.shields.io/badge/골동품-3498DB?style=for-the-badge)
![Anomaly](https://img.shields.io/badge/이상현상-3498DB?style=for-the-badge)

<br/>

2026-1학기 캡스톤 디자인과 창업 프로젝트 | **팀 규교굥 (Team 5)**

<br/>

</div>

---

# 게임 소개

## 💵 상점을 운영해 빚을 갚으세요.

당신은 매일 상점을 운영해 얻은 수익으로 빚을 갚아야 합니다.

수익을 얻는 방법은 간단합니다.

**물건을 싸게 사서 비싸게 팔기만 하면 됩니다.**

그러기 위해선 다음과 같은 일을 해야 합니다.

- 매일 아침 이메일을 통해 거래 제안을 확인하세요.
- 마을을 돌아다니며 팔만한 물건들을 수집하세요.
- 상점을 청소하세요.
- 물건을 깨끗하게 하거나 빈티지스럽게 만들어 더 높은 가격에 팔 수 있습니다.
- 물건을 진열하세요.
- 손님에게 물건을 파세요.
- AI NPC인 손님과 흥정하여 최대한 낮은 가격에 물건을 사세요.
- 가게 안 컴퓨터로 물건을 주문하고 가게를 홍보하세요.

**당신도 이제 사장님이 되어 당신만의 가게를 운영하고 확장할 수 있습니다!**

<br/>

## 📦 이상현상을 찾아 상자에 넣으세요.

상점에는 아주 오래된 상자가 있는데 그 상자에는 비밀이 있습니다. **상자를 여는 것은 당신의 자유입니다!**

<br/>

---

# 주요 기능

- **AI NPC 실시간 흥정 시스템:** Ollama를 활용한 지능형 손님 구현.
    - 플레이어의 대화에 따라 동적으로 변하는 물건 가격.
- **골동품 관리 및 상호작용:** 1인칭 시점의 정교한 물건 조작.
    - 도구를 활용한 아이템 닦기, 수리 등 상점 운영의 디테일 구현.
- **상점 성장 및 확장:** 게임 내 수익을 통해 인벤토리 확충 및 매장 리모델링.
- **미스터리 추적:** 특정 사건과 연관된 '특수 아이템'의 이상현상 감별하고 상자 안에 수집하는 퀘스트.

<br/>

---

# AI 생성 콘텐츠 사용 공개

- 이 게임에 사용된 일부 그래픽 에셋은 AI를 사용해 만들어졌습니다.
- 게임 플레이 중 AI는 플레이어의 입력에 기반하여 일부 NPC의 대사를 생성합니다.

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

# 레포지토리 구조

```text
📂 Start2026_1/
├── 📂 Assets/                            # 유니티 프로젝트 에셋
│   ├── 📂 Animation/                     # 애니메이션 클립 및 컨트롤러
│   ├── 📂 BlenderAssetTest/              # 블렌더 에셋 연동 테스트 폴더
│   ├── 📂 Material/                      # 3D 모델 머티리얼 및 텍스처
│   ├── 📂 MobileDependencyResolver/      # 모바일/외부 패키지 종속성 관리
│   ├── 📂 Prefabs/                       # 게임 내 재사용 가능한 프리팹 오브젝트
│   ├── 📂 Resources/                     # 런타임 동적 로드용 리소스
│   ├── 📂 Scenes/                        # 게임 씬
│   ├── 📂 Scripts/                       # C# 스크립트
│   ├── 📂 Settings/                      # 유니티 환경 및 렌더링 설정 (URP 등)
│   └── 📂 TutorialInfo/                  # 유니티 튜토리얼 관련 데이터
├── 📂 BlenderPractice/                   # 블렌더 3D 모델링 작업 및 연습 파일
├── 📂 Packages/                          # 유니티 패키지
├── 📂 ProjectSettings/                   # 유니티 프로젝트 설정
├── 📂 experiments/
│   └── 📂 ollama-negotiation/            # Ollama AI 기반 흥정 시스템 테스트 및 실험 데이터
├── 📂 images/                            # README에 사용되는 이미지 파일
├── Ideation.MD                            # 게임 기획 및 아이디어 정리 문서
├── ProjectDescription.md                  # 프로젝트 상세 설명 문서
├── README.md                              # 레포지토리 메인 소개 문서
├── Start2026_1.slnx                       # 프로젝트 솔루션 파일
├── Team_Ground_Rule.md                    # 팀 협업 규칙 및 그라운드 룰 문서
└── index.html                             # 웹 호스팅용 메인 인덱스 페이지
