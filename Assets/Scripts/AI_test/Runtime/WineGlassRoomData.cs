using System.Collections.Generic;

public static class WineGlassRoomData
{
    public static PuzzleConfig GetConfig()
    {
        return new PuzzleConfig
        {
            puzzleId = "wine_glass_room",
            totalSteps = 6,
            requiredClues = new List<string> { "clue_wine_stains", "clue_wine_labels", "clue_bookshelf_order" },
            steps = new List<PuzzleStep>
            {
                new PuzzleStep
                {
                    id = 1,
                    goal = "바닥의 와인 얼룩과 알파벳을 조사한다",
                    hintByLevel = new[]
                    {
                        "발밑을 한번 살펴보는 게 어떨까요.",
                        "바닥에 흩어진 붉은 흔적들 사이에 뭔가 있을 수도 있어요.",
                        "바닥의 와인 얼룩들을 자세히 보면 알파벳이 숨어있을지도 몰라요.",
                        "같은 색깔의 얼룩 두 개를 찾아, 그 사이 중간 지점을 확인해보세요."
                    }
                },
                new PuzzleStep
                {
                    id = 2,
                    goal = "같은 색 얼룩끼리 짝지어 알파벳 4글자를 알아낸다",
                    hintByLevel = new[]
                    {
                        "얼룩들 사이에 뭔가 규칙이 있을지도 몰라요.",
                        "색깔이 같은 얼룩들끼리 연관이 있어 보여요.",
                        "같은 색 얼룩 두 개의 중점에 알파벳이 하나씩 있어요.",
                        "모든 색깔 쌍의 중점 알파벳을 모으면 네 글자가 나올 거예요."
                    }
                },
                new PuzzleStep
                {
                    id = 3,
                    goal = "와인렉 라벨의 숫자를 확인한다",
                    hintByLevel = new[]
                    {
                        "이 방에 있는 다른 물건도 뭔가 관련 있어 보여요.",
                        "선반에 놓인 병들을 살펴보세요.",
                        "와인렉에 꽂힌 병들의 라벨에 적힌 숫자를 확인해보세요.",
                        "라벨의 빈티지 연도 뒤 두 자리 숫자가 중요한 단서예요."
                    }
                },
                new PuzzleStep
                {
                    id = 4,
                    goal = "얼룩 색과 같은 색 와인의 숫자만큼 알파벳을 밀어 진짜 알파벳을 알아낸다",
                    hintByLevel = new[]
                    {
                        "두 가지 단서가 서로 연결될지도 몰라요.",
                        "색깔이 두 단서를 이어주는 열쇠일 수 있어요.",
                        "얼룩 색과 같은 색의 와인 병 번호를 짝지어보세요.",
                        "그 숫자만큼 알파벳을 순서대로 밀어보세요. 예를 들어 A와 1이면 B가 돼요."
                    }
                },
                new PuzzleStep
                {
                    id = 5,
                    goal = "책장에서 알아낸 알파벳에 해당하는 책 4권을 찾는다",
                    hintByLevel = new[]
                    {
                        "이 방을 벗어나 다른 공간도 살펴볼 때가 된 것 같아요.",
                        "오래된 책장이 있는 곳을 찾아보세요.",
                        "책장의 책들은 제목 첫 글자 순서대로 꽂혀 있어요.",
                        "알아낸 알파벳과 같은 첫 글자를 가진 책 네 권을 꺼내보세요."
                    }
                },
                new PuzzleStep
                {
                    id = 6,
                    goal = "책을 와인렉의 와인 배치 순서대로 꽂는다",
                    hintByLevel = new[]
                    {
                        "이제 마지막으로 정리할 일이 남은 것 같아요.",
                        "책의 순서가 다른 곳에 있는 무언가와 관련 있을 수 있어요.",
                        "와인렉에 병들이 놓인 순서를 다시 확인해보세요.",
                        "찾은 책 네 권을 와인렉의 병 순서와 똑같이 배치해보세요."
                    }
                },
            }
        };
    }
}