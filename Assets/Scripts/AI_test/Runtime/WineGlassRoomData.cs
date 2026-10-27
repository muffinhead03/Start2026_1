using System.Collections.Generic;

/// <summary>
/// 와인씬 퍼즐 체크리스트 (10월 리디자인 기준).
///
/// 흐름: 벽 그림(Hint_Stain)·바닥 얼룩 → 알파벳 4개를 메모 종이에 기록
///       → 책장에서 그 글자로 시작하는 책의 숫자 확인
///       → 병을 색에 맞는 글자의 숫자 칸(와인렉)에 꽂기 → 장식장 열림
///       → 장식장 안 열쇠로 상자 열기
///
/// 스텝 완료는 전부 WineHintBridge가 처리함 (예전 FloorStainManager / WineLabelManager / BookShelfComplete는 씬에서 끔):
///  1 메모 종이 정답 기록 (LetterMemoPanel.IsSolved)  — 뒤 단계가 진행되면 자동 완료
///  2 글자를 안 상태에서 책장 확대 보기 (BookShelfView Fixed) — 병을 2개 이상 정답 칸에 꽂아도 완료
///  3 병 4개 전부 정답 칸 (Object_Puzzle pwd 일치)
///  4 열쇠로 상자 열기 — 탈출이라 완료 보고 없음 (진행 구간 "열쇠 아직 / 열쇠 보유"만 판단)
///
/// relatedObjectName은 두 군데에 쓰임:
///  - WineLocationProvider의 trackedObjects 이름 (거리 문장)
///  - WineHintBridge.GetHandObjectName()이 돌려주는 이름 (손/인벤토리 override)
///    병은 "wine_1"~"wine_4" → "wine"으로 묶어서 넘김, 열쇠는 "key1" 그대로
///
/// 레벨 1~2: 방향만 / 3: 무엇을 볼지 / 4: 어떻게 연결할지 / 5: 해야 할 행동 전체 (정답 글자·숫자는 끝까지 안 씀)
/// </summary>
public static class WineGlassRoomData
{
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
                    goal = "벽 그림과 바닥 얼룩으로 색별 알파벳 4개를 알아내 메모 종이에 기록한다",
                    relatedObjectName = "HintStain", // WineLocationProvider: Room_1/Hint_Stain
                    hintByLevel = new[]
                    {
                        "이 방 벽과 바닥을 천천히 둘러보세요.",
                        "벽에 걸린 그림이 바닥 얼룩과 닮아 보이지 않나요?",
                        "벽 그림을 가까이서 보고, 같은 색깔의 바닥 얼룩을 찾아보세요.",
                        "얼룩 색깔마다 숨어 있는 글자가 있어요. 찾은 글자는 책장 옆 메모 종이에 적을 수 있어요.",
                        "벽 그림 속 색깔 얼룩과 같은 색의 바닥 얼룩에서 글자를 하나씩 찾고, 메모 종이의 같은 색 칸에 그 글자를 적으세요."
                    }
                },
                new PuzzleStep
                {
                    id = 2,
                    goal = "메모의 알파벳으로 시작하는 책을 책장에서 찾아 책에 적힌 숫자를 확인한다",
                    relatedObjectName = "BookShelf", // WineLocationProvider: Room_3/BookShelfView
                    hintByLevel = new[]
                    {
                        "적어둔 메모를 어디에 쓸 수 있을지 생각해보세요.",
                        "메모 종이 가까이에 있는 가구를 살펴보세요.",
                        "책장을 가까이서 보세요. 책 제목이 눈에 띌 거예요.",
                        "메모에 적은 글자로 시작하는 책을 찾아보세요. 그 책에 숫자가 있어요.",
                        "책장을 확대해서 메모의 각 글자로 시작하는 제목의 책을 찾고, 그 책에 적힌 숫자를 색깔별로 기억해두세요."
                    }
                },
                new PuzzleStep
                {
                    id = 3,
                    goal = "병 4개를 색에 맞는 글자의 책 숫자 칸(와인렉)에 꽂는다",
                    relatedObjectName = "wine", // WineLocationProvider: 와인렉(WineReck 또는 wine_slots) / 손에 병을 들면 override
                    hintByLevel = new[]
                    {
                        "알아낸 숫자를 쓸 만한 곳이 있을 거예요.",
                        "와인렉의 빈 칸들을 살펴보세요.",
                        "와인렉 칸마다 번호가 있어요. 책에서 본 숫자와 연결해보세요.",
                        "병마다 해당하는 색이 있어요. 그 색의 글자로 찾은 책 숫자 칸에 병을 꽂아보세요.",
                        "병 하나를 들고 그 병 색에 맞는 메모 글자를 떠올린 뒤, 그 글자 책의 숫자와 같은 번호의 와인렉 칸에 꽂으세요. 네 병 모두 맞추면 돼요."
                    }
                },
                new PuzzleStep
                {
                    id = 4,
                    goal = "열린 장식장 안의 열쇠를 꺼내 상자를 연다",
                    relatedObjectName = "key1", // WineLocationProvider: 상자(chestbox) / 열쇠를 들면 override
                    hintByLevel = new[]
                    {
                        "방 안에 방금 달라진 곳이 있어요.",
                        "잠겨 있던 장식장을 다시 보세요.",
                        "장식장 안에 작은 물건이 있어요.",
                        "장식장 안의 열쇠를 꺼내서 쓸 곳을 찾아보세요.",
                        "열린 장식장에서 열쇠를 꺼내 상자에 넣으면 탈출구가 열려요."
                    }
                },
            }
        };
    }
}