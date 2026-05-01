<a id="team-5"></a>
## Team 5 규교굥

| 항목 | 내용 |
|------|------|
| 프로젝트명 | On-device 로컬 LLM 기술을 사용하여, 플레이어 맞춤형 힌트를 실시간으로 제공하는 1인칭 3D 공포 방탈출 게임 |
| 서비스명(브랜드) | Do Not Open This Box |
| 트랙 | 산학 |
| 팀명 | 규교굥 |
| 팀구성 | 정혜교, 윤민주, 박남규 |
| 팀지도교수 | 윤명국 교수님 |
| 무엇을 만들고자 하는가 | 6개의 방을 순차적으로 탈출하는 3D 1인칭 공포 방탈출 게임. 각 씬마다 고유한 실내 공간에서 단서를 수집하고 추리하여 탈출 아이템을 찾아 상자에 넣으면 탈출구가 열린다.막혔을 때는 무전기를 통해 On-device LLM(Ollama)에 힌트를 요청할 수 있으며, LLM은 플레이어의 물음을 토대로 맞춤형 힌트를 제공한다(씬당 2회 사용 가능). |
| 고객 (누구를 위해) | PC로 공포 장르 게임을 즐기는 플레이어, 방탈출·호러 게임 관련 콘텐츠를 소비하는 영상 플랫폼 이용자 or 방탈출·퍼즐 게임을 즐기지만 난이도에 막혀 중도 포기한 경험이 있는 PC 게이머, 호러 게임 콘텐츠를 즐기는 영상 플랫폼 이용자 |
| Pain Point (해결할 문제) | 방탈출 게임을 플레이하다 너무 어려워 포기하는 유저가 많다. 특히 스토리 중심 게임에서 특정 단계에 막히면 외부 공략을 찾게 되어 몰입이 깨진다. 기존 게임은 플레이어의 현재 상황을 반영한 맞춤형 힌트를 제공하지 못한다. 본 프로젝트는 On-device LLM을 통해 플레이어의 물음을 토대로 맞춤형 힌트를 게임 내에서 바로 제공함으로써, 이탈 없이 자연스럽게 플레이를 이어갈 수 있도록 한다. |
| 사용 기술 | 유니티 3D, Ollama |
| 개발환경 | 1. Client 디바이스: PC (Windows, Mac)<br>2. FE: Unity, Blender, Clip studio<br>3. BE: 초기에는 Standalone 형태로 개발 후, 필요 시 FastAPI 기반 서버 연동 예정<br>4. DB: 초기에는 로컬 데이터 저장 방식을 고려하고 있으며, 필요 시 MySQL 도입 예정<br>5. 특별한 라이브러리: Unity URP/2D Light, TextMeshPro, JsonUtility, Custom Render Feature, Shader Graph, Post Processing, Cinemachine<br>6. API Call 서비스:  Ollama 4B 기반 로컬 LLM
| 사용하는 소프트웨어 URL | unity.com / blender.org / ollama.com
| 기대 효과 | 플레이어는 공포 분위기 속 추리·탐색의 긴장감을 즐기는 동시에, On-device LLM 기반 맞춤형 힌트로 게임 흐름을 끊지 않고 자연스럽게 진행할 수 있다. 외부 서버 없이 로컬에서 실행되어 API 비용이 없으므로 플레이어가 비용을 부담할 필요가 없으며, 또한 플레이어마다 다른 맥락 기반 힌트 경험을 제공한다. |
| GitHub Repo | [https://github.com/muffinhead03/Start2026_1](https://github.com/muffinhead03/Start2026_1) |
| Team Ground Rule | https://github.com/muffinhead03/Start2026_1/blob/main/Team_Ground_Rule.md |
| 최종수정일 | 2026.05.01 |
