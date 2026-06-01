using System.Collections.Generic;

public static class OrganRoomData
{
    public static PuzzleConfig GetConfig()
    {
        return new PuzzleConfig
        {
            puzzleId = "organ_room",
            totalSteps = 9, // 시연 범위: Phase 1만 (Phase 2까지 구현 시 totalsteps = 12), 3(18), 4(22) 구현 후 22으로 변경)
            requiredClues = new List<string>
            {
                // ── Phase 1 (시연 범위) ──────────────────────────────
                "clue_organ_manual",  // 오르간 설명서
                "clue_pipe_1",        // 파이프 1
                "clue_pipe_2",        // 파이프 2
                "clue_pipe_3",        // 파이프 3
                "clue_pipe_4",        // 파이프 4
                "clue_music_box",     // 오르골 (방3 키패드 단서)
                "clue_room2_key",     // 방2 열쇠
                "clue_lp_hint",       // LP 단서 (어떤 LP를 넣어야 하는지)

                // ── Phase 2 (구현 후 주석 해제) ──────────────────────
                // "clue_entity_pattern",
                // "clue_room4_key",

                // ── Phase 3 (구현 후 주석 해제) ──────────────────────
                // "clue_torn_sheet_1",
                // "clue_torn_sheet_2",
                // "clue_torn_sheet_3",
                // "clue_torn_sheet_4",
                // "clue_drawer_password",
                // "clue_room5_switch",

                // ── Phase 4 (구현 후 주석 해제) ──────────────────────
                // "clue_completed_sheet",
                // "clue_grandson_diary",
            },
            steps = new List<PuzzleStep>
            {
                // ── Phase 1 : 오르간 수리하기 ──────────────────────────
                new PuzzleStep { id = 1, goal = "오르간 설명서를 읽고 파이프 교체가 필요함을 파악한다",             hintDirection = "오르간 근처에 무언가 단서가 있을 것입니다" },
                new PuzzleStep { id = 2, goal = "파이프 1, 2를 수집한다",                                           hintDirection = "파이프는 공간 곳곳에 숨겨져 있습니다" },
                new PuzzleStep { id = 3, goal = "방2 열쇠로 방2를 열고 방3 단서를 확인한다",                       hintDirection = "잠긴 문을 열 수 있는 무언가를 찾아보세요" },
                new PuzzleStep { id = 4, goal = "오르골을 재생해 방3 키패드 번호를 알아낸다",                      hintDirection = "음악 소리가 힌트를 줄 수도 있습니다" },
                new PuzzleStep { id = 5, goal = "키패드에 4025를 입력해 방3을 열고 파이프 3을 수집한다",           hintDirection = "들었던 소리와 숫자를 연결해보세요" },
                new PuzzleStep { id = 6, goal = "LP 단서를 확인해 어떤 LP를 넣어야 하는지 파악한다",               hintDirection = "LP 플레이어 근처에 단서가 있을 것입니다" },
                new PuzzleStep { id = 7, goal = "초록색 7번 LP를 LP 플레이어에 넣어 파이프 4를 수집한다",          hintDirection = "올바른 LP를 플레이어에 넣어보세요" },
                new PuzzleStep { id = 8, goal = "파이프 4개를 오르간에 교체해 수리를 완료한다",                    hintDirection = "모은 것들을 오르간에 사용해보세요" },
                new PuzzleStep { id = 9, goal = "오르간 수리 완료 후 다음 단계를 인지한다",                        hintDirection = "무언가 달라진 것이 있습니다" },

                // ── Phase 2 (구현 후 주석 해제, totalSteps = 12) ─────
                // new PuzzleStep { id = 10, goal = "오르간 수리 후 엔티티가 등장함을 인지한다",          hintDirection = "소리에 집중하세요 — 엔티티는 눈에 보이지 않습니다" },
                // new PuzzleStep { id = 11, goal = "방1의 LP를 작동시켜 엔티티를 방1로 유인한다",       hintDirection = "방1 LP 플레이어로 엔티티의 주의를 돌리세요" },
                // new PuzzleStep { id = 12, goal = "2층 복도에서 방4 열쇠를 획득하고 방4를 개방한다",   hintDirection = "2층 복도 바닥을 확인하세요" },

                // ── Phase 3 (구현 후 주석 해제, totalSteps = 18) ─────
                // new PuzzleStep { id = 13, goal = "방4에서 메트로놈 박자를 맞춰 악보 조각 1을 획득한다",           hintDirection = "방4 메트로놈의 박자에 맞게 상호작용하세요" },
                // new PuzzleStep { id = 14, goal = "1층 거실 바닥에서 악보 조각 2를 찾는다",                        hintDirection = "바람에 흩어진 악보들 사이 거실 바닥을 살펴보세요" },
                // new PuzzleStep { id = 15, goal = "거실 첫 번째 책장 두 번째 줄 스위치로 방5를 개방한다",          hintDirection = "첫 번째 책장을 자세히 살펴보세요" },
                // new PuzzleStep { id = 16, goal = "방5 책 사이에서 악보 조각 3을 찾는다",                          hintDirection = "방5 책장의 책들을 하나씩 확인하세요" },
                // new PuzzleStep { id = 17, goal = "방5 책장 두 번째 줄 책 번호를 나열해 서랍 비밀번호를 알아낸다", hintDirection = "책등에 적힌 번호의 순서를 확인하세요" },
                // new PuzzleStep { id = 18, goal = "잠긴 서랍을 열어 악보 조각 4를 획득하고 악보를 완성한다",       hintDirection = "알아낸 번호로 서랍을 열어보세요" },

                // ── Phase 4 (구현 후 주석 해제, totalSteps = 22) ─────
                // new PuzzleStep { id = 19, goal = "완성된 악보를 들고 오르간과 상호작용해 악보를 세팅한다",         hintDirection = "오르간 앞에서 완성된 악보를 사용하세요" },
                // new PuzzleStep { id = 20, goal = "오르간을 다시 상호작용해 연주 모드로 진입한다",                  hintDirection = "악보가 세팅된 오르간을 다시 조작하세요" },
                // new PuzzleStep { id = 21, goal = "악보에 표시된 번호 순서대로 건반을 눌러 곡을 완주한다",          hintDirection = "악보의 번호와 건반의 번호를 맞춰 순서대로 연주하세요" },
                // new PuzzleStep { id = 22, goal = "엔티티가 사라진 후 거실에서 열쇠를 찾아 상자에 넣고 탈출한다",   hintDirection = "연주가 끝나면 거실을 확인하세요" },
            }
        };
    }
}