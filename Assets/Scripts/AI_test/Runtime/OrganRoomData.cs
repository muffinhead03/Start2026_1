using System.Collections.Generic;

/// <summary>
/// 오르간씬 퍼즐 설정. relatedObjectName은 OrganHintBridge.ResolveTarget()의 위치 키와 1:1로 맞춰둔 값.
/// step 6의 "LP"만 손/인벤토리 override에도 쓰임 (OrganHintBridge가 LP_0~LP_4를 "LP"로 묶어서 넘김).
/// 나머지 키는 실제 objectName과 겹치지 않게 지어서 위치 전용으로만 동작.
/// </summary>
public static class OrganRoomData
{
    public static PuzzleConfig GetConfig()
    {
        return new PuzzleConfig
        {
            puzzleId = "organ_room",
            totalSteps = 17, // Phase 1(7) + Phase 2(7) + Phase 3(3)
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
                {
                    id = 1, goal = "오르간 설명서 책을 확인한다",
                    relatedObjectName = "Organ", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "오르간 근처에 뭔가 있어.",
                        "오르간 근처에 책이 있어.",
                        "오르간 근처에 책이 있어. 그걸 읽어봐.",
                        "오르간 근처에 설명서가 있는 것 같아.",
                        "오르간이 고장난 것 같아. 근처에 있는 설명서를 읽어보자."
                    }
                },
                new PuzzleStep
                {
                    id = 1, goal = "파이프를 하나라도 교체한다",
                    relatedObjectName = "Organ", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "책에 나온대로 오르간을 고쳐야해.",
                        "책에 나온대로 오르간 파이프를 교체해야 해.",
                        "책에 나온대로 오르간 파이프를 교체해야 해. 근처에 파이프가 있는지 찾아봐.",
                        "책에 나온대로 오르간 파이프를 교체해야 해. 근처에 파이프가 있는지 찾아봐.",
                        "책에 나온대로 오르간 파이프를 교체해야 해. 근처에 파이프가 있는지 찾아봐."
                    }
                },
                new PuzzleStep
                {
                    id = 2, goal = "파이프 1과 2를 찾는다",
                    relatedObjectName = "Pipe1", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "1층에 파이프가 두 개 있어.",
                        "1층에 파이프가 두 개 있어.",
                        "오르간 계단 뒤쪽 공간에 파이프 하나가 있고 근처에 있는 방에도 파이프 하나가 있어.",
                        "오르간 계단 뒤쪽 공간에 파이프 하나가 있고 근처에 있는 방에도 파이프 하나가 있어.",
                        "오르간 계단 뒤쪽 공간에 파이프 하나가 있고 근처에 있는 방에도 파이프 하나가 있어."
                    }
                },
                new PuzzleStep
                {
                    id = 4, goal = "거실의 오르골을 재생해 멜로디를 확인한다",
                    relatedObjectName = "MusicBox", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "오르간 뒤쪽 공간에 오르골이 있어.",
                        "오르간 뒤쪽 공간에 오르골이 있어.",
                        "오르간 뒤쪽 공간에 오르골이 있어. 한번 재생해봐.",
                        "오르간 뒤쪽 공간에 오르골이 있어. 한번 재생해봐.",
                        "오르간 뒤쪽 공간에 오르골이 있어. 한번 재생해봐."
                    }
                },
                new PuzzleStep
                {
                    id = 4, goal = "2층에 비밀번호 키패드가 있음을 인지한다",
                    relatedObjectName = "PwdDoor", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "2층에 비밀번호 푸는 방이 있을거야.",
                        "2층에 비밀번호 푸는 방이 있을거야.",
                        "2층에 비밀번호 푸는 방이 있을거야. 단서는 이미 확인했어.",
                        "2층에 비밀번호 푸는 방이 있을거야. 단서는 이미 확인했어.",
                        "2층에 비밀번호 푸는 방이 있을거야. 단서는 이미 확인했어."
                    }
                },
                new PuzzleStep
                {
                    id = 5, goal = "오르골 멜로디로 비밀번호 방을 연다",
                    relatedObjectName = "PwdDoor", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "그러고보니 비밀번호를 누를 때에도 소리가 났던 것 같아.",
                        "그러고보니 비밀번호를 누를 때에도 소리가 났던 것 같아.",
                        "오르골과 비밀번호에서 같은 소리가 나도록 눌러야 해.",
                        "오르골과 비밀번호에서 같은 소리가 나도록 눌러야 해.",
                        "오르골과 비밀번호에서 같은 소리가 나도록 눌러야 해. 음정에 집중해봐."
                    }
                },
                new PuzzleStep
                {
                    id = 5, goal = "파이프 3을 얻는다",
                    relatedObjectName = "Pipe3", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "비밀번호를 풀었으니 뭔가 다른 게 있겠지.",
                        "비밀번호를 풀었으니 거기에 파이프가 있겠지.",
                        "비밀번호로 푼 방에 파이프가 서랍 위에 올려져 있어.",
                        "비밀번호로 푼 방에 파이프가 서랍 위에 올려져 있어.",
                        "설마 이걸 못찾는다고? 비밀번호로 푼 방에 파이프가 서랍 위에 올려져 있어."
                    }
                },
                new PuzzleStep
                {
                    id = 6, goal = "축음기 방에서 곡에 대한 책을 본다",
                    relatedObjectName = "LP", // LP_0~4를 들고 있으면 이 스텝으로 override + 위치는 턴테이블(GramophoneA) — OrganHintBridge
                    hintByLevel = new[]
                    {
                        "축음기 옆에 책이 하나 있어.",
                        "축음기 옆에 책이 하나 있어.",
                        "축음기 옆에 책이 하나 있는데 그 책을 봐야 해.",
                        "축음기 옆에 책이 하나 있는데 그 책을 봐야 해.",
                        "축음기 옆에 책이 하나 있는데 그 책을 봐야 해."
                    }
                },
                new PuzzleStep
                {
                    id = 7, goal = "LP를 하나라도 찾는다",
                    relatedObjectName = "LP",
                    hintByLevel = new[]
                    {
                        "책에서 읽은 곡에 대한 설명과 일치하는 그림을 찾아봐.",
                        "책에서 읽은 곡에 대한 설명과 일치하는 그림을 찾아봐.",
                        "책에서 읽은 곡에 대한 설명과 일치하는 그림을 찾아봐. 근처에 LP가 있을 거야.",
                        "책에서 읽은 곡에 대한 설명과 일치하는 그림을 찾아봐. 근처에 LP가 있을 거야.",
                        "책에서 읽은 곡에 대한 설명과 일치하는 그림을 찾아봐. 근처에 LP가 있을 거야."
                    }
                },
                new PuzzleStep
                {
                    id = 8, goal = "1층에서 LP를 찾는다",
                    relatedObjectName = "LP",
                    hintByLevel = new[]
                    {
                        "1층 가장 넓은 공간에 LP가 2개 있고 방에도 LP가 1개 있어.",
                        "1층 가장 넓은 공간에 LP가 2개 있고 방에도 LP가 1개 있어.",
                        "1층 가장 넓은 공간 마주보는 벽에 LP가 2개 있고 방에도 LP가 1개 있어.",
                        "1층 가장 넓은 공간 마주보는 벽에 LP가 2개 있고 방에도 LP가 1개 있어.",
                        "1층 가장 넓은 공간 마주보는 벽에 LP가 2개 있고 방에도 LP가 1개 있어."
                    }
                },
                new PuzzleStep
                {
                    id = 9, goal = "2층에서 LP를 찾는다",
                    relatedObjectName = "LP",
                    hintByLevel = new[]
                    {
                        "2층 곳곳을 돌아다니다보면 LP를 총 2개 발견할 수 있을 거야.",
                        "2층 곳곳을 돌아다니다보면 LP를 총 2개 발견할 수 있을 거야.",
                        "2층 곳곳을 돌아다니다보면 LP를 총 2개 발견할 수 있을 거야. 계단과 거리가 먼 곳에 있어.",
                        "2층 곳곳을 돌아다니다보면 LP를 총 2개 발견할 수 있을 거야. 계단과 거리가 먼 곳에 있어.",
                        "2층 곳곳을 돌아다니다보면 LP를 총 2개 발견할 수 있을 거야. 계단과 거리가 먼 곳에 있어."
                    }
                },
                new PuzzleStep
                {
                    id = 10, goal = "LP로 플레이리스트를 재생한다",
                    relatedObjectName = "LP",
                    hintByLevel = new[]
                    {
                        "찾아낸 LP를 축음기에 넣고 돌려봐.",
                        "찾아낸 LP를 축음기에 넣고 돌려봐.",
                        "찾아낸 LP를 축음기에 넣고 돌려봐. 플레이리스트대로 재생하면 돼.",
                        "찾아낸 LP를 축음기에 넣고 돌려봐. 플레이리스트대로 재생하면 돼.",
                        "찾아낸 LP를 축음기에 넣고 돌려봐. 플레이리스트대로 재생하면 돼."
                    }
                },
                new PuzzleStep
                {
                    id = 11, goal = "거실 그림을 보고 올바른 색 LP를 재생해 파이프 4를 찾는다",
                    relatedObjectName = "LP", // LP_0~4를 들고 있으면 이 스텝으로 override + 위치는 턴테이블(GramophoneA) — OrganHintBridge
                    hintByLevel = new[]
                    {
                        "거실에 걸린 것도 관련이 있을 수 있어요.",
                        "그림이 뭔가를 가리키고 있을지도 몰라요.",
                        "그림이 어떤 색 LP를 가리키는지 확인하세요.",
                        "그림에 나온 원의 개수가 가리키는 색의 LP를 재생하면 파이프가 나타날 거예요.",
                        "그림에 나온 원의 개수를 확인하세요. 원이 제일 많은 색깔의 LP판을 턴테이블에 올려 재생하세요. 그러면 파이프가 나타나요."
                    }
                },
                new PuzzleStep
                {
                    id = 12, goal = "파이프 4개를 오르간에 교체한다",
                    relatedObjectName = "Organ", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "먼저 오르간에 있는 파이프를 빼고 새로 찾은 파이프를 교체해봐.",
                        "먼저 오르간에 있는 파이프를 빼고 새로 찾은 파이프를 교체해봐.",
                        "이미 파이프를 다 찾았어. 먼저 오르간에 있는 파이프를 빼고 새로 찾은 파이프를 교체해봐.",
                        "이미 파이프를 다 찾았어. 먼저 오르간에 있는 파이프를 빼고 새로 찾은 파이프를 교체해봐.",
                        "이미 파이프를 다 찾았어. 먼저 오르간에 있는 파이프를 빼고 새로 찾은 파이프를 교체해봐."
                    }
                },

                // ── Phase 2 : 악보 4조각 찾기 ───────────────────────────
                new PuzzleStep
                {
                    id = 13, goal = "악보를 하나라도 찾는다.",
                    relatedObjectName = "Organ", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "어딘가에 숨겨진 악보를 찾아야 할 것 같아.",
                        "어딘가에 숨겨진 악보를 찾아야 할 것 같아.",
                        "어딘가에 숨겨진 악보를 찾아서 오르간을 연주해야 할 것 같아.",
                        "어딘가에 숨겨진 악보를 찾아서 오르간을 연주해야 할 것 같아.",
                        "어딘가에 숨겨진 악보를 찾아서 오르간을 연주해야 할 것 같아."
                    }
                },
                new PuzzleStep
                {
                    id = 14, goal = "파이프 교체 후 흩어진 악보 조각1을 찾는다",
                    relatedObjectName = "SheetPiece2", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "갑자기 책들이 떨어졌어.",
                        "갑자기 책들이 떨어졌어.",
                        "갑자기 책들이 떨어졌어. 그 책 사이에 악보가 있는 것 같아.",
                        "갑자기 책들이 떨어졌어. 그 책 사이에 악보가 있는 것 같아.",
                        "갑자기 책들이 떨어졌어. 그 책 사이에 악보가 있는 것 같아."
                    }
                },
                new PuzzleStep
                {
                    id = 15, goal = "비밀 통로를 통해 메트로놈 방으로 이동한다",
                    relatedObjectName = "Vent", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "아까 무언가 부서지는 소리가 들리지 않았어?",
                        "아까 무언가 부서지는 소리가 들리지 않았어?",
                        "아까 무언가 부서지는 소리가 들리지 않았어? 어딘가 공간이 열린 것 같아.",
                        "아까 무언가 부서지는 소리가 들리지 않았어? 어딘가 공간이 열린 것 같아.",
                        "아까 무언가 부서지는 소리가 들리지 않았어? 막혀있던 공간이 열린 것 같아."
                    }
                },
                new PuzzleStep
                {
                    id = 16, goal = "메트로놈 박자를 맞춰 캐비닛을 연다",
                    relatedObjectName = "Metronome", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "메트로놈 바늘을 올바른 타이밍에 눌러야 해.",
                        "메트로놈 바늘이 올바른 타이밍에 눌러야 해.",
                        "메트로놈 바늘이 그림 액자 속 붉은색에 있을 때 버튼을 눌러야 하는 것 같아.",
                        "메트로놈 바늘이 그림 액자 속 붉은색에 있을 때 버튼을 눌러야 하는 것 같아.",
                        "메트로놈 바늘이 그림 액자 속 붉은색에 있을 때 버튼을 눌러야 하는 것 같아."
                    }
                },
                new PuzzleStep
                {
                    id = 17, goal = "캐비닛에서 악보 2를 찾는다",
                    relatedObjectName = "Metronome", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "메트로놈을 해결한 후 초록색 사물함이 열렸어.",
                        "메트로놈을 해결한 후 초록색 사물함이 열렸어.",
                        "초록색 사물함 안에 악보가 있었을 거야.",
                        "초록색 사물함 안에 악보가 있었을 거야.",
                        "초록색 사물함 안에 악보가 있었을 거야."
                    }
                },
                new PuzzleStep
                {
                    id = 18, goal = "책장을 조작해본다",
                    relatedObjectName = "BookShelf", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "2층 책장에 있는 책들이 뭔가 수상해.",
                        "2층 책장에 있는 책들이 뭔가 수상해.",
                        "2층 책장에 있는 책들 하나씩 밀거나 당길 수 있는 것 같아.",
                        "2층 책장에 있는 책들 하나씩 밀거나 당길 수 있는 것 같아.",
                        "2층 책장에 있는 책들 하나씩 밀거나 당길 수 있는 것 같아."
                    }
                },
                new PuzzleStep
                {
                    id = 19, goal = "캐비닛 안 힌트 그림대로 책장을 조작해 큰 방을 연다",
                    relatedObjectName = "BookShelf", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "메트로놈이 있던 방에 책장을 움직일 수 있는 단서가 있어.",
                        "메트로놈이 있던 방에 책장을 움직일 수 있는 단서가 있어.",
                        "초록색 사물함 안에 책장을 움직일 수 있는 단서가 있어.",
                        "초록색 사물함 안에 책장을 움직일 수 있는 단서가 있어.",
                        "초록색 사물함 안에 책장을 움직일 수 있는 단서가 있어."
                    }
                },

                new PuzzleStep
                {
                    id = 20, goal = "책상 위에서 악보 조각 3를 찾는다",
                    relatedObjectName = "SheetPiece3", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "책장 뒤에 있던 방에 악보 조각 하나가 있던데.",
                        "책장 뒤에 있던 방에 악보 조각 하나가 있던데.",
                        "책상 위에 악보 조각 하나를 본 거 같은데.",
                        "책상 위에 악보 조각 하나를 본 거 같은데.",
                        "책상 위에 악보 조각 하나를 본 거 같은데."
                    }
                },
                new PuzzleStep
                {
                    id = 21, goal = "가족 사진 액자 안에서 악보 조각 4을 찾는다책상 위에서 악보 조각 4를 찾는다",
                    relatedObjectName = "SheetPiece4", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "방에 가족사진이 있었지? 거기에서 이질감이 느껴져.",
                        "방에 가족사진이 있었지? 거기에서 이질감이 느껴져.",
                        "방에 가족사진이 있었지? 사진을 빼서 조사해봐.",
                        "방에 가족사진이 있었지? 사진을 빼서 조사해봐.",
                        "방에 가족사진이 있었지? 사진을 빼서 조사해봐."
                    }
                },

                // ── Phase 3 : 오르간 연주 & 탈출 ────────────────────────
                new PuzzleStep
                {
                    id = 22, goal = "모은 악보 조각을 기존 조각과 조합해 완성한다",
                    relatedObjectName = "Organ", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "찾은 악보를 오르간 위에 올려놔보자.",
                        "찾은 악보를 오르간 위에 올려놔보자.",
                        "악보를 다 찾았어. 찾은 악보를 오르간 위에 올려놔보자.",
                        "악보를 다 찾았어. 찾은 악보를 오르간 위에 올려놔보자.",
                        "악보를 다 찾았어. 찾은 악보를 오르간 위에 올려놔보자."
                    }
                },
                new PuzzleStep
                {
                    id = 23, goal = "완성된 악보의 숫자 순서대로 건반을 눌러 연주한다",
                    relatedObjectName = "Organ", // OrganHintBridge 위치 매핑 키
                    hintByLevel = new[]
                    {
                        "악보를 연주해봐.",
                        "악보를 연주해봐.",
                        "악보에 적힌 숫자대로 건반을 누르면 될 거야.",
                        "악보에 적힌 숫자대로 건반을 누르면 될 거야.",
                        "악보에 적힌 숫자대로 건반을 누르면 될 거야."
                    }
                }
            }
        };
    }
}