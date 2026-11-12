using System.Collections.Generic;

/// <summary>
/// 와인씬 퍼즐 체크리스트 (10월 리디자인 기준).
///
/// 흐름: 통로 벽 종이(Hint_Stain)·바닥 얼룩 → 알파벳 4개를 액자 아래 메모 종이에 기록
///       → 책장에서 그 글자로 시작하는 책의 숫자 확인
///       → 병을 색에 맞는 글자의 숫자 칸(와인렉)에 꽂기 → 장식장 열림
///       → 장식장 안 열쇠로 상자 열기
///
/// 스텝 완료는 전부 WineHintBridge가 처리함 (예전 FloorStainManager / WineLabelManager / BookShelfComplete는 씬에서 끔):
///  1 메모 종이 정답 기록 (LetterMemoPanel.IsSolved)  — 뒤 단계가 진행되면 자동 완료
///  2 병을 정답 번호 칸(어떤 병의 정답 칸이든)에 처음 꽂음 = 책 숫자를 찾은 것 — 병을 2개 이상 정답 칸에 꽂아도 완료
///  3 병 4개 전부 정답 칸 (Object_Puzzle pwd 일치)
///  4 열쇠로 상자 열기 — 탈출이라 완료 보고 없음 (진행 구간 "열쇠 아직 / 열쇠 보유"만 판단)
///
/// relatedObjectName은 두 군데에 쓰임:
///  - WineLocationProvider의 trackedObjects 이름 (거리 문장)
///  - WineHintBridge.GetHandObjectName()이 돌려주는 이름 (손/인벤토리 override)
///    병은 "wine_1"~"wine_4" → "wine"으로 묶어서 넘김, 열쇠는 "key1" 그대로
///
/// 레벨 1~2: 방향만 / 3: 무엇을 볼지 / 4: 어떻게 연결할지 / 5: 해야 할 행동 전체 (정답 글자·숫자는 끝까지 안 씀)
///
/// hintByLevel 문장(한·영·일)은 HintLanguageData.WineScene.cs에 있음 — 여기는 퍼즐 구조(스텝·대상 오브젝트)와 "어느 키를 쓸지"만.
/// GetConfig()는 힌트를 요청할 때마다 새로 불리므로, 그 순간의 언어 설정으로 문장이 채워짐 (게임 중 언어 변경도 다음 힌트부터 반영).
/// </summary>
public static class WineGlassRoomData
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
            puzzleId = "wine_glass_room",
            totalSteps = 4,
            requiredClues = new List<string> { "clue_memo_letters", "clue_book_numbers", "clue_wine_rack" },
            steps = new List<PuzzleStep>
            {
                new PuzzleStep
                {
                    id = 1,
                    goal = "통로 벽 종이와 바닥 얼룩으로 색별 알파벳 4개를 알아내 액자 아래 메모 종이에 기록한다",
                    relatedObjectName = "HintStain", // WineLocationProvider: Hint_Stain (통로 벽 종이)
                    // 아래 hintByLevel은 "벽 종이를 아직 안 봤을 때"만 쓰임 (종이를 찾게 하는 단계).
                    // 1단계 힌트는 모든 레벨·진행 구간에서 "알파벳"을 꼭 말함 (피드백 반영 — "글자"로만 말하면 바닥 알파벳 격자를 놓침)
                    // 종이를 본 뒤엔 WineHintBridge 진행 문장이 대신함:
                    //   종이 본 직후 첫 요청 → 종이 수수께끼 풀기(바닥 얼룩 ↔ 그림) / 그다음 요청부터 → 액자 아래 메모에 적기
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintWine_Step1_Level1,
                        HintLanguageDataKey.HintWine_Step1_Level2,
                        HintLanguageDataKey.HintWine_Step1_Level3,
                        HintLanguageDataKey.HintWine_Step1_Level4,
                        HintLanguageDataKey.HintWine_Step1_Level5)
                },
                new PuzzleStep
                {
                    id = 2,
                    goal = "메모의 알파벳으로 시작하는 책을 책장에서 찾아 책에 적힌 숫자를 확인한다",
                    relatedObjectName = "BookShelf", // WineLocationProvider: Room_3/BookShelfView
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintWine_Step2_Level1,
                        HintLanguageDataKey.HintWine_Step2_Level2,
                        HintLanguageDataKey.HintWine_Step2_Level3,
                        HintLanguageDataKey.HintWine_Step2_Level4,
                        HintLanguageDataKey.HintWine_Step2_Level5)
                },
                new PuzzleStep
                {
                    id = 3,
                    goal = "병 4개를 색에 맞는 글자의 책 숫자 칸(와인렉)에 꽂는다",
                    relatedObjectName = "wine", // WineLocationProvider: 와인렉(WineReck 또는 wine_slots) / 손에 병을 들면 override
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintWine_Step3_Level1,
                        HintLanguageDataKey.HintWine_Step3_Level2,
                        HintLanguageDataKey.HintWine_Step3_Level3,
                        HintLanguageDataKey.HintWine_Step3_Level4,
                        HintLanguageDataKey.HintWine_Step3_Level5)
                },
                new PuzzleStep
                {
                    id = 4,
                    goal = "열린 장식장 안의 열쇠를 꺼내 상자를 연다",
                    relatedObjectName = "key1", // WineLocationProvider: 상자(chestbox) / 열쇠를 들면 override
                    hintByLevel = Levels(
                        HintLanguageDataKey.HintWine_Step4_Level1,
                        HintLanguageDataKey.HintWine_Step4_Level2,
                        HintLanguageDataKey.HintWine_Step4_Level3,
                        HintLanguageDataKey.HintWine_Step4_Level4,
                        HintLanguageDataKey.HintWine_Step4_Level5)
                },
            }
        };
    }
}