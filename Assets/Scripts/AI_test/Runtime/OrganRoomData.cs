using System.Collections.Generic;

/// <summary>
/// 오르간씬 퍼즐 설정 (scene02 민주 힌트 트리 기준, 23스텝).
///
/// id = 실제 진행 순서(1~23). HintEngine은 "가장 낮은 id의 미완료 스텝"을 고르므로
/// OrganHintBridge가 트리 리프를 id로 바꾼 뒤 그보다 작은 id를 전부 완료 처리한다.
/// 트리 리프 번호 → id 매핑은 OrganHintBridge.LeafToStep에 있음 (각 스텝 주석에도 리프 번호 표기).
///
/// relatedObjectName은 OrganHintBridge.ResolveTarget()의 위치 키 (거리 문장용).
/// 손/인벤토리 override는 쓰지 않음 — 트리가 진행 판정을 전부 하기 때문.
///
/// hintByLevel 문장(한·영·일)은 HintLanguageData.OrganScene.cs에 있음 — 여기는 퍼즐 구조와 "어느 키를 쓸지"만.
/// GetConfig()는 힌트를 요청할 때마다 새로 불리므로, 그 순간의 언어 설정으로 문장이 채워짐.
/// </summary>
public static class OrganRoomData
{
    // 레벨 1~5 키 → 현재 언어 문장 (없으면 한국어) — DollRoomData와 같은 방식
    static string[] Levels(params string[] keys)
    {
        var lines = new string[keys.Length];
        for (int i = 0; i < keys.Length; i++)
            lines[i] = HintLanguageData.Get(keys[i]);
        return lines;
    }

    public static PuzzleConfig GetConfig()
    {
        return new PuzzleConfig
        {
            puzzleId = "organ_room",
            totalSteps = 23, // Phase 1(12) + Phase 2(9) + Phase 3(2)
            requiredClues = new List<string>
            {
                "clue_organ_broken",
                "clue_music_box_melody",
                "clue_lp_painting_hint",
                "clue_torn_sheet_pieces",
                "clue_completed_sheet",
            },
            steps = new List<PuzzleStep>
            {
                // ── Phase 1 : 파이프 4개 찾아 교체하기 ──────────────────
                new PuzzleStep
                { // 트리 리프 0 (GetCurrentStep() == 1)
                    id = 1, goal = "오르간 설명서 책을 확인한다",
                    relatedObjectName = "Organ", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step1_Level1,
                        HintLanguageDataKey.HintOrgan_Step1_Level2,
                        HintLanguageDataKey.HintOrgan_Step1_Level3,
                        HintLanguageDataKey.HintOrgan_Step1_Level4,
                        HintLanguageDataKey.HintOrgan_Step1_Level5)
                },
                new PuzzleStep
                { // 트리 리프 1 (GetCurrentStep() == 2)
                    id = 2, goal = "파이프를 하나라도 교체한다",
                    relatedObjectName = "Organ", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step2_Level1,
                        HintLanguageDataKey.HintOrgan_Step2_Level2,
                        HintLanguageDataKey.HintOrgan_Step2_Level3,
                        HintLanguageDataKey.HintOrgan_Step2_Level4,
                        HintLanguageDataKey.HintOrgan_Step2_Level5)
                },
                new PuzzleStep
                { // 트리 리프 2 (GetCurrentStep() == 3)
                    id = 3, goal = "파이프 1과 2를 찾는다",
                    relatedObjectName = "Pipe1", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step3_Level1,
                        HintLanguageDataKey.HintOrgan_Step3_Level2,
                        HintLanguageDataKey.HintOrgan_Step3_Level3,
                        HintLanguageDataKey.HintOrgan_Step3_Level4,
                        HintLanguageDataKey.HintOrgan_Step3_Level5)
                },
                new PuzzleStep
                { // 트리 리프 3 (GetCurrentStep() == 4)
                    id = 4, goal = "거실의 오르골을 재생해 멜로디를 확인한다",
                    relatedObjectName = "MusicBox", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step4_Level1,
                        HintLanguageDataKey.HintOrgan_Step4_Level2,
                        HintLanguageDataKey.HintOrgan_Step4_Level3,
                        HintLanguageDataKey.HintOrgan_Step4_Level4,
                        HintLanguageDataKey.HintOrgan_Step4_Level5)
                },
                new PuzzleStep
                { // 트리 리프 4 (GetCurrentStep() == 5)
                    id = 5, goal = "2층에 비밀번호 키패드가 있음을 인지한다",
                    relatedObjectName = "PwdDoor", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step5_Level1,
                        HintLanguageDataKey.HintOrgan_Step5_Level2,
                        HintLanguageDataKey.HintOrgan_Step5_Level3,
                        HintLanguageDataKey.HintOrgan_Step5_Level4,
                        HintLanguageDataKey.HintOrgan_Step5_Level5)
                },
                new PuzzleStep
                { // 트리 리프 5 (GetCurrentStep() == 6)
                    id = 6, goal = "오르골 멜로디로 비밀번호 방을 연다",
                    relatedObjectName = "PwdDoor", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step6_Level1,
                        HintLanguageDataKey.HintOrgan_Step6_Level2,
                        HintLanguageDataKey.HintOrgan_Step6_Level3,
                        HintLanguageDataKey.HintOrgan_Step6_Level4,
                        HintLanguageDataKey.HintOrgan_Step6_Level5)
                },
                new PuzzleStep
                { // 트리 리프 6 (GetCurrentStep() == 7)
                    id = 7, goal = "파이프 3을 얻는다",
                    relatedObjectName = "Pipe3", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step7_Level1,
                        HintLanguageDataKey.HintOrgan_Step7_Level2,
                        HintLanguageDataKey.HintOrgan_Step7_Level3,
                        HintLanguageDataKey.HintOrgan_Step7_Level4,
                        HintLanguageDataKey.HintOrgan_Step7_Level5)
                },
                new PuzzleStep
                { // 트리 리프 9 (GetCurrentStep() == 10)
                    id = 8, goal = "축음기 방에서 곡에 대한 책을 본다",
                    relatedObjectName = "LP", // LP_0~4를 들고 있으면 이 스텝으로 override + 위치는 턴테이블(GramophoneA) — OrganHintBridge
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step8_Level1,
                        HintLanguageDataKey.HintOrgan_Step8_Level2,
                        HintLanguageDataKey.HintOrgan_Step8_Level3,
                        HintLanguageDataKey.HintOrgan_Step8_Level4,
                        HintLanguageDataKey.HintOrgan_Step8_Level5)
                },
                new PuzzleStep
                { // 트리 리프 10 (GetCurrentStep() == 11)
                    id = 9, goal = "LP를 하나라도 찾는다",
                    relatedObjectName = "LP",
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step9_Level1,
                        HintLanguageDataKey.HintOrgan_Step9_Level2,
                        HintLanguageDataKey.HintOrgan_Step9_Level3,
                        HintLanguageDataKey.HintOrgan_Step9_Level4,
                        HintLanguageDataKey.HintOrgan_Step9_Level5)
                },
                new PuzzleStep
                { // 트리 리프 7 (GetCurrentStep() == 8)
                    id = 10, goal = "1층에서 LP를 찾는다",
                    relatedObjectName = "LP",
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step10_Level1,
                        HintLanguageDataKey.HintOrgan_Step10_Level2,
                        HintLanguageDataKey.HintOrgan_Step10_Level3,
                        HintLanguageDataKey.HintOrgan_Step10_Level4,
                        HintLanguageDataKey.HintOrgan_Step10_Level5)
                },
                new PuzzleStep
                { // 트리 리프 8 (GetCurrentStep() == 9)
                    id = 11, goal = "2층에서 LP를 찾는다",
                    relatedObjectName = "LP",
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step11_Level1,
                        HintLanguageDataKey.HintOrgan_Step11_Level2,
                        HintLanguageDataKey.HintOrgan_Step11_Level3,
                        HintLanguageDataKey.HintOrgan_Step11_Level4,
                        HintLanguageDataKey.HintOrgan_Step11_Level5)
                },
                new PuzzleStep
                { // 트리 리프 11 (GetCurrentStep() == 12)
                    id = 12, goal = "파이프 4개를 오르간에 교체한다",
                    relatedObjectName = "Organ", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step12_Level1,
                        HintLanguageDataKey.HintOrgan_Step12_Level2,
                        HintLanguageDataKey.HintOrgan_Step12_Level3,
                        HintLanguageDataKey.HintOrgan_Step12_Level4,
                        HintLanguageDataKey.HintOrgan_Step12_Level5)
                },
                // ── Phase 2 : 악보 4조각 찾기 ───────────────────────────
                new PuzzleStep
                { // 트리 리프 12 (GetCurrentStep() == 13)
                    id = 13, goal = "악보를 하나라도 찾는다",
                    relatedObjectName = "Organ", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step13_Level1,
                        HintLanguageDataKey.HintOrgan_Step13_Level2,
                        HintLanguageDataKey.HintOrgan_Step13_Level3,
                        HintLanguageDataKey.HintOrgan_Step13_Level4,
                        HintLanguageDataKey.HintOrgan_Step13_Level5)
                },
                new PuzzleStep
                { // 트리 리프 13 (GetCurrentStep() == 14)
                    id = 14, goal = "파이프 교체 후 흩어진 악보 조각1을 찾는다",
                    relatedObjectName = "SheetPiece2", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step14_Level1,
                        HintLanguageDataKey.HintOrgan_Step14_Level2,
                        HintLanguageDataKey.HintOrgan_Step14_Level3,
                        HintLanguageDataKey.HintOrgan_Step14_Level4,
                        HintLanguageDataKey.HintOrgan_Step14_Level5)
                },
                new PuzzleStep
                { // 트리 리프 14 (GetCurrentStep() == 15)
                    id = 15, goal = "비밀 통로를 통해 메트로놈 방으로 이동한다",
                    relatedObjectName = "Vent", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step15_Level1,
                        HintLanguageDataKey.HintOrgan_Step15_Level2,
                        HintLanguageDataKey.HintOrgan_Step15_Level3,
                        HintLanguageDataKey.HintOrgan_Step15_Level4,
                        HintLanguageDataKey.HintOrgan_Step15_Level5)
                },
                new PuzzleStep
                { // 트리 리프 15 (GetCurrentStep() == 16)
                    id = 16, goal = "메트로놈 박자를 맞춰 캐비닛을 연다",
                    relatedObjectName = "Metronome", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step16_Level1,
                        HintLanguageDataKey.HintOrgan_Step16_Level2,
                        HintLanguageDataKey.HintOrgan_Step16_Level3,
                        HintLanguageDataKey.HintOrgan_Step16_Level4,
                        HintLanguageDataKey.HintOrgan_Step16_Level5)
                },
                new PuzzleStep
                { // 트리 리프 16 (GetCurrentStep() == 17)
                    id = 17, goal = "캐비닛에서 악보 2를 찾는다",
                    relatedObjectName = "Metronome", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step17_Level1,
                        HintLanguageDataKey.HintOrgan_Step17_Level2,
                        HintLanguageDataKey.HintOrgan_Step17_Level3,
                        HintLanguageDataKey.HintOrgan_Step17_Level4,
                        HintLanguageDataKey.HintOrgan_Step17_Level5)
                },
                new PuzzleStep
                { // 트리 리프 17 (GetCurrentStep() == 18)
                    id = 18, goal = "책장을 조작해본다",
                    relatedObjectName = "BookShelf", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step18_Level1,
                        HintLanguageDataKey.HintOrgan_Step18_Level2,
                        HintLanguageDataKey.HintOrgan_Step18_Level3,
                        HintLanguageDataKey.HintOrgan_Step18_Level4,
                        HintLanguageDataKey.HintOrgan_Step18_Level5)
                },
                new PuzzleStep
                { // 트리 리프 18 (GetCurrentStep() == 19)
                    id = 19, goal = "캐비닛 안 힌트 그림대로 책장을 조작해 큰 방을 연다",
                    relatedObjectName = "BookShelf", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step19_Level1,
                        HintLanguageDataKey.HintOrgan_Step19_Level2,
                        HintLanguageDataKey.HintOrgan_Step19_Level3,
                        HintLanguageDataKey.HintOrgan_Step19_Level4,
                        HintLanguageDataKey.HintOrgan_Step19_Level5)
                },
                new PuzzleStep
                { // 트리 리프 19 (GetCurrentStep() == 20)
                    id = 20, goal = "책상 위에서 악보 조각 3을 찾는다",
                    relatedObjectName = "SheetPiece3", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step20_Level1,
                        HintLanguageDataKey.HintOrgan_Step20_Level2,
                        HintLanguageDataKey.HintOrgan_Step20_Level3,
                        HintLanguageDataKey.HintOrgan_Step20_Level4,
                        HintLanguageDataKey.HintOrgan_Step20_Level5)
                },
                new PuzzleStep
                { // 트리 리프 20 (GetCurrentStep() == 21)
                    id = 21, goal = "가족 사진 액자 안에서 악보 조각 4를 찾는다",
                    relatedObjectName = "SheetPiece4", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step21_Level1,
                        HintLanguageDataKey.HintOrgan_Step21_Level2,
                        HintLanguageDataKey.HintOrgan_Step21_Level3,
                        HintLanguageDataKey.HintOrgan_Step21_Level4,
                        HintLanguageDataKey.HintOrgan_Step21_Level5)
                },
                // ── Phase 3 : 오르간 연주 & 탈출 ────────────────────────
                new PuzzleStep
                { // 트리 리프 21 (GetCurrentStep() == 22)
                    id = 22, goal = "모은 악보 조각을 기존 조각과 조합해 완성한다",
                    relatedObjectName = "Organ", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step22_Level1,
                        HintLanguageDataKey.HintOrgan_Step22_Level2,
                        HintLanguageDataKey.HintOrgan_Step22_Level3,
                        HintLanguageDataKey.HintOrgan_Step22_Level4,
                        HintLanguageDataKey.HintOrgan_Step22_Level5)
                },
                new PuzzleStep
                { // 트리 리프 22 (GetCurrentStep() == 23)
                    id = 23, goal = "완성된 악보의 숫자 순서대로 건반을 눌러 연주한다",
                    relatedObjectName = "Organ", // OrganHintBridge 위치 매핑 키
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintOrgan_Step23_Level1,
                        HintLanguageDataKey.HintOrgan_Step23_Level2,
                        HintLanguageDataKey.HintOrgan_Step23_Level3,
                        HintLanguageDataKey.HintOrgan_Step23_Level4,
                        HintLanguageDataKey.HintOrgan_Step23_Level5)
                }
            }
        };
    }

    // ── 보류: 트리에 대응하는 리프가 없는 스텝 (민주 확인 후 살리거나 삭제) ──
    // 리프 11은 "파이프 다 찾음(c2)"과 "다음 LP를 이미 가지고 있음(c12)" 두 경로에서 같이 나와서,
    // 플레이리스트 재생 중인데도 "파이프 교체" 힌트가 나올 수 있음. 리프를 나눠주면 아래 두 스텝을 다시 넣고
    // HintLanguageData.OrganScene.cs에 키를 추가하면 됨.
    //  - "LP로 플레이리스트를 재생한다"        : 찾아낸 LP를 축음기에 넣고 돌려봐. / 플레이리스트대로 재생하면 돼.
    //  - "거실 그림을 보고 올바른 색 LP를 재생해 파이프 4를 찾는다"
    //                                        : 그림이 어떤 색 LP를 가리키는지 확인해봐. / 원이 제일 많은 색깔의 LP판을 턴테이블에 올려 재생하면 파이프가 나타나.
}