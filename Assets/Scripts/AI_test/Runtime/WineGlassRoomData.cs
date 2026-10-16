using System.Collections.Generic;

/// <summary>
/// 와인씬 퍼즐 체크리스트.
/// 스텝 번호를 바꾸면 completedSteps.Add를 부르는 쪽도 같이 맞춰야 함:
///  1 FloorStainManager (+ WineHintBridge: 진짜 쪽지를 찾으면 자동 완료)
///  2 FloorStainManager
///  3 WineHintBridge (HintPaperNotesManager 부착 개수 폴링)
///  4, 5 WineLabelManager
///  6, 7 BookShelfComplete
/// 2·3단계는 WineHintBridge(IStepProgressProvider)가 진행 구간별 문장으로 hintByLevel을 대신하는 경우가 많음
/// (쪽지 일부 찾음 / 다 모았는데 안 붙임 / 붙이는 중 / 가짜 쪽지 시도).
/// </summary>
public static class WineGlassRoomData
{
    public static PuzzleConfig GetConfig()
    {
        return new PuzzleConfig
        {
            puzzleId = "wine_glass_room",
            totalSteps = 7,
            requiredClues = new List<string> { "clue_wine_stains", "clue_hint_paper", "clue_wine_labels", "clue_bookshelf_order" },
            steps = new List<PuzzleStep>
            {
                new PuzzleStep
                {
                    id = 1,
                    goal = "바닥의 와인 얼룩을 조사한다",
                    relatedObjectName = "WineStains", // WineLocationProvider에 등록된 "1.WineStains" 영역과 매칭
                    hintByLevel = new[]
                    {
                        "발밑을 한번 살펴보는 게 어떨까요.",
                        "바닥에 흩어진 붉은 흔적들 사이에 뭔가 있을 수도 있어요.",
                        "바닥의 와인 얼룩들을 색깔별로 나눠서 살펴보세요.",
                        "같은 색깔의 얼룩 두 개를 찾아, 그 사이 중간 지점을 확인해보세요.",
                        "바닥에 흩어진 붉은 얼룩들을 색깔별로 나눠 보고, 같은 색 얼룩 두 개의 정확히 중간 지점을 보면 알파벳 쪽지가 놓여 있어요."
                    }
                },
                new PuzzleStep
                {
                    id = 2,
                    goal = "같은 색 얼룩 쌍의 중점에 놓인 진짜 알파벳 쪽지 4장을 찾는다 (바닥엔 가짜 쪽지도 섞여 있음)",
                    relatedObjectName = "StainCrossPoints", // "2.StainCrossPoints" 영역과 매칭
                    hintByLevel = new[]
                    {
                        "얼룩들 사이에 뭔가 규칙이 있을지도 몰라요.",
                        "색깔이 같은 얼룩들끼리 연관이 있어 보여요.",
                        "같은 색 얼룩 두 개의 중점에 알파벳 쪽지가 하나씩 놓여 있어요.",
                        "바닥엔 쪽지가 여러 장 있지만, 같은 색 얼룩 쌍의 중점에 있는 쪽지만 진짜예요.",
                        "바닥에는 색이 다른 얼룩 쌍이 네 개 있어요. 각 색깔 쌍의 정확한 중간 지점에 놓인 쪽지를 하나씩 주워서 네 장을 모으세요."
                    }
                },
                new PuzzleStep
                {
                    id = 3,
                    goal = "모은 진짜 쪽지 4장을 벽의 낡은 종이에 붙인다",
                    relatedObjectName = "HintPaper", // WineLocationProvider에 벽 종이(HintPaper)를 등록해야 거리 문장이 붙음
                    // 실제로는 WineHintBridge의 진행 구간 문장(다 모음 / 붙이는 중 / 가짜 시도)이 거의 항상 이걸 대신함
                    hintByLevel = new[]
                    {
                        "모은 쪽지를 어디에 쓸 수 있을지 생각해보세요.",
                        "이 쪽지들을 붙일 만한 종이가 어딘가 있을 거예요.",
                        "벽 쪽을 살펴보세요. 쪽지를 붙일 수 있는 낡은 종이가 있어요.",
                        "모은 쪽지를 들고 벽에 붙은 낡은 종이로 가보세요.",
                        "바닥에서 모은 쪽지 네 장을 하나씩 손에 들고, 벽에 붙은 낡은 종이의 빈 칸에 붙이세요."
                    }
                },
                new PuzzleStep
                {
                    id = 4,
                    goal = "벽 종이의 수수께끼를 보고, 와인렉 라벨의 숫자를 확인한다",
                    relatedObjectName = "WineRackLabels", // "3.WineRackLabels" 영역과 매칭
                    hintByLevel = new[]
                    {
                        "벽 종이에 나타난 수수께끼를 풀어보세요.",
                        "수수께끼는 알파벳에 숫자를 더하는 규칙 같아요. 더할 숫자는 이 방 선반의 병들에 있을지도 몰라요.",
                        "와인렉에 꽂힌 병들의 라벨에 적힌 숫자를 확인해보세요.",
                        "라벨의 빈티지 연도 뒤 두 자리 숫자가 수수께끼에 들어갈 숫자예요.",
                        "와인렉에 꽂힌 병 하나하나의 라벨을 확인해서 빈티지 연도 뒤 두 자리 숫자를 전부 적어두세요. 벽 종이의 수수께끼처럼 이 숫자만큼 알파벳을 밀게 될 거예요."
                    }
                },
                new PuzzleStep
                {
                    id = 5,
                    goal = "얼룩 색과 같은 색 와인의 숫자만큼 알파벳을 밀어 진짜 알파벳을 알아낸다",
                    // relatedObjectName 없음: 쪽지 알파벳과 라벨 숫자를 머릿속에서 조합하는 계산 단계라
                    // 특정 물리적 위치가 없음 — proximityNote는 자동으로 안 붙고 나머지 힌트는 정상 동작함
                    hintByLevel = new[]
                    {
                        "두 가지 단서가 서로 연결될지도 몰라요.",
                        "색깔이 두 단서를 이어주는 열쇠일 수 있어요.",
                        "얼룩 색과 같은 색의 와인 병 번호를 짝지어보세요.",
                        "벽 종이의 수수께끼처럼, 그 숫자만큼 알파벳을 순서대로 밀어보세요. 예를 들어 A와 3이면 D가 돼요.",
                        "바닥에서 모은 쪽지 알파벳 네 개를, 같은 색 와인 라벨의 두 자리 숫자만큼 알파벳 순서로 하나씩 밀어서 바꾸세요. 그렇게 바뀐 네 글자가 진짜 정답이에요."
                    }
                },
                new PuzzleStep
                {
                    id = 6,
                    goal = "책장에서 알아낸 알파벳에 해당하는 책 4권을 찾는다",
                    relatedObjectName = "PuzzleBookShelf", // "5_6.PuzzleBookShelf" 영역과 매칭
                    hintByLevel = new[]
                    {
                        "이 방을 벗어나 다른 공간도 살펴볼 때가 된 것 같아요.",
                        "오래된 책장이 있는 곳을 찾아보세요.",
                        "책장의 책들은 제목 첫 글자 순서대로 꽂혀 있어요.",
                        "알아낸 알파벳과 같은 첫 글자를 가진 책 네 권을 꺼내보세요.",
                        "책장에서 앞에서 알아낸 진짜 알파벳 네 글자와 제목 첫 글자가 같은 책을 각각 하나씩 찾아 총 네 권을 꺼내세요."
                    }
                },
                new PuzzleStep
                {
                    id = 7,
                    goal = "책을 와인렉의 와인 배치 순서대로 꽂는다",
                    relatedObjectName = "PuzzleBookShelf", // 6단계와 같은 책장 — 배치만 다름
                    hintByLevel = new[]
                    {
                        "이제 마지막으로 정리할 일이 남은 것 같아요.",
                        "책의 순서가 다른 곳에 있는 무언가와 관련 있을 수 있어요.",
                        "와인렉에 병들이 놓인 순서를 다시 확인해보세요.",
                        "찾은 책 네 권을 와인렉의 병 순서와 똑같이 배치해보세요.",
                        "와인렉에 병이 놓인 왼쪽부터의 순서를 확인하고, 그 순서와 똑같은 순서로 책장 슬롯에 책 네 권을 꽂으면 퍼즐이 완성돼요."
                    }
                },
            }
        };
    }
}