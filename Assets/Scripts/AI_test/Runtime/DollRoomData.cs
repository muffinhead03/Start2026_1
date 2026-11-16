using System.Collections.Generic;

/// <summary>
/// 인형씬 퍼즐 체크리스트 (2026-11 기획 변경 반영 — 9단계).
///
/// 흐름: 그림책(시소 그림)으로 마트료시카 크기별 무게 알아내기
///       → 장식장 위에 서 있는 순서대로 무게를 자물쇠에 입력해 목마 구역 열기
///       → 다리 교체 → 곰 인형 던져 동전 → 오락기에 동전 넣어 팔 → 팔 교체
///       → 선로 맞춰 태엽 → 태엽 장착 → 태엽 돌려 열쇠 얻고 탈출
///
/// 스텝 완료는 전부 DollHintBridge가 처리함 (1단계는 Book_Matryo 조사 이벤트 → ReportBookViewed).
///
/// relatedObjectName은 두 가지 용도로 쓰임:
/// - 3/4/5/6/8번: Object_Grabbable.objectName과 같은 값 → 손/인벤토리 보유 시 HintEngine이 그 스텝으로 override
/// - 1/2/7/9번: 위치 전용 키 (실제 오브젝트 이름과 겹치지 않게 지음) → DollHintBridge.ResolveTarget()이 Transform으로 매핑
///
/// hintByLevel 문장(한·영·일)은 HintLanguageData.DollScene.cs에 있음 — 여기는 퍼즐 구조(스텝·대상 오브젝트)와 "어느 키를 쓸지"만.
/// GetConfig()는 힌트를 요청할 때마다 새로 불리므로, 그 순간의 언어 설정으로 문장이 채워짐.
/// </summary>
public static class DollRoomData
{
    // 레벨 1~5 키 → 현재 언어 문장 (없으면 한국어)
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
            puzzleId = "doll_room",
            totalSteps = 9,
            requiredClues = new List<string>
            {
                "clue_matryoshka_book",
                "clue_leg_lock_code",
                "clue_doll_leg",
                "clue_coin",
                "clue_doll_arm",
                "clue_spring",
            },
            steps = new List<PuzzleStep>
            {
                new PuzzleStep
                {
                    id = 1, goal = "그림책을 보고 마트료시카의 무게를 알아낸다",
                    relatedObjectName = "MatryoBook", // 위치 전용 키 (DollHintBridge) — 1. DollFencePart/Book_Matryo
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintDoll_Step1_Level1,
                        HintLanguageDataKey.HintDoll_Step1_Level2,
                        HintLanguageDataKey.HintDoll_Step1_Level3,
                        HintLanguageDataKey.HintDoll_Step1_Level4,
                        HintLanguageDataKey.HintDoll_Step1_Level5)
                },
                new PuzzleStep
                {
                    id = 2, goal = "책을 보고난 후 자물쇠 번호를 알아내 목마 구역을 연다",
                    relatedObjectName = "LockArea", // 위치 전용 키 (DollHintBridge) — 관찰형 퍼즐
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintDoll_Step2_Level1,
                        HintLanguageDataKey.HintDoll_Step2_Level2,
                        HintLanguageDataKey.HintDoll_Step2_Level3,
                        HintLanguageDataKey.HintDoll_Step2_Level4,
                        HintLanguageDataKey.HintDoll_Step2_Level5)
                },
                new PuzzleStep
                {
                    id = 3, goal = "목마 구역에서 부러진 다리를 찾아 교체한다",
                    relatedObjectName = "DollLeg", // Object_Grabbable.objectName Inspector 값으로 확인 완료
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintDoll_Step3_Level1,
                        HintLanguageDataKey.HintDoll_Step3_Level2,
                        HintLanguageDataKey.HintDoll_Step3_Level3,
                        HintLanguageDataKey.HintDoll_Step3_Level4,
                        HintLanguageDataKey.HintDoll_Step3_Level5)
                },
                new PuzzleStep
                {
                    id = 4, goal = "말하는 곰 인형을 반복해서 던져 동전을 얻는다",
                    relatedObjectName = "TeddyBear", // 씬의 곰 인형 objectName은 비어있음 — DollHintBridge.ResolveObjectName()이 SpeakingBearDoll 컴포넌트로 식별해서 이 값으로 치환
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintDoll_Step4_Level1,
                        HintLanguageDataKey.HintDoll_Step4_Level2,
                        HintLanguageDataKey.HintDoll_Step4_Level3,
                        HintLanguageDataKey.HintDoll_Step4_Level4,
                        HintLanguageDataKey.HintDoll_Step4_Level5)
                },
                new PuzzleStep
                {
                    id = 5, goal = "오락기에 동전을 넣고 화면 연출이 끝나면 나오는 팔을 얻는다",
                    relatedObjectName = "동전", // Object_Grabbable.objectName Inspector 값으로 확인 완료 (GameObject 이름은 Bear_Coin이지만 필드값은 "동전")
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintDoll_Step5_Level1,
                        HintLanguageDataKey.HintDoll_Step5_Level2,
                        HintLanguageDataKey.HintDoll_Step5_Level3,
                        HintLanguageDataKey.HintDoll_Step5_Level4,
                        HintLanguageDataKey.HintDoll_Step5_Level5)
                },
                new PuzzleStep
                {
                    id = 6, goal = "획득한 팔을 인형에 교체한다",
                    relatedObjectName = "DollArm", // Object_Grabbable.objectName Inspector 값으로 확인 완료
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintDoll_Step6_Level1,
                        HintLanguageDataKey.HintDoll_Step6_Level2,
                        HintLanguageDataKey.HintDoll_Step6_Level3,
                        HintLanguageDataKey.HintDoll_Step6_Level4,
                        HintLanguageDataKey.HintDoll_Step6_Level5)
                },
                new PuzzleStep
                {
                    id = 7, goal = "버튼으로 선로를 맞춰 검은 기차를 밀어내고 태엽을 얻는다",
                    relatedObjectName = "TrainArea", // 위치 전용 키 (DollHintBridge) — 선로/기차 조작형 퍼즐
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintDoll_Step7_Level1,
                        HintLanguageDataKey.HintDoll_Step7_Level2,
                        HintLanguageDataKey.HintDoll_Step7_Level3,
                        HintLanguageDataKey.HintDoll_Step7_Level4,
                        HintLanguageDataKey.HintDoll_Step7_Level5)
                },
                new PuzzleStep
                {
                    id = 8, goal = "얻은 태엽을 인형에 장착한다",
                    relatedObjectName = "Spring", // Object_Grabbable.objectName Inspector 값으로 확인 완료
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintDoll_Step8_Level1,
                        HintLanguageDataKey.HintDoll_Step8_Level2,
                        HintLanguageDataKey.HintDoll_Step8_Level3,
                        HintLanguageDataKey.HintDoll_Step8_Level4,
                        HintLanguageDataKey.HintDoll_Step8_Level5)
                },
                new PuzzleStep
                {
                    id = 9, goal = "완성된 인형의 태엽을 돌려 수리를 완료하고 열쇠를 얻어 탈출한다",
                    relatedObjectName = "DollStand", // 위치 전용 키 (DollHintBridge) — 다리/팔/태엽이 전부 장착된 인형 본체
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintDoll_Step9_Level1,
                        HintLanguageDataKey.HintDoll_Step9_Level2,
                        HintLanguageDataKey.HintDoll_Step9_Level3,
                        HintLanguageDataKey.HintDoll_Step9_Level4,
                        HintLanguageDataKey.HintDoll_Step9_Level5)
                },
            }
        };
    }
}