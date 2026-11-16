using System.Collections.Generic;

/// <summary>
/// 오르간씬 번역 데이터 — 단계별 힌트(OrganRoomData.hintByLevel, 23단계 × 레벨 1~5).
/// LLM 힌트의 [Hint direction]과 LLM이 없을 때의 고정 문구(폴백) 둘 다 이 문장을 씀.
/// 모든 문장은 관찰 문장과 같은 반말. 비밀번호·건반 정답 숫자는 어떤 문장에도 넣지 않음.
///
/// 스텝 번호는 OrganRoomData의 id(진행 순서)와 같음. scene02 힌트 트리 리프 ↔ id 매핑은 OrganHintBridge.LeafToStep 참고.
/// 최근 행동 문장(organ_step_NN / organ_hold_*)은 HintLanguageData.Actions.cs에 이미 있음.
/// </summary>
public static partial class HintLanguageData
{
    private static readonly Dictionary<string, Dictionary<LanguageType, string[]>> OrganSceneData = new()
    {

        // ── Step 1 : 설명서 확인 ──
        {
            HintLanguageDataKey.HintOrgan_Step1_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "오르간 근처에 뭔가 있어." } },
                { LanguageType.English,  new[] { "There's something near the organ." } },
                { LanguageType.Japanese, new[] { "オルガンの近くに何かあるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step1_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "오르간 근처에 책이 있어." } },
                { LanguageType.English,  new[] { "There's a book near the organ." } },
                { LanguageType.Japanese, new[] { "オルガンの近くに本があるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step1_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "오르간 근처에 책이 있어. 그걸 읽어봐." } },
                { LanguageType.English,  new[] { "There's a book near the organ. Give it a read." } },
                { LanguageType.Japanese, new[] { "オルガンの近くに本があるよ。読んでみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step1_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "오르간 근처에 설명서가 있는 것 같아." } },
                { LanguageType.English,  new[] { "Looks like there's a manual near the organ." } },
                { LanguageType.Japanese, new[] { "オルガンの近くに説明書があるみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step1_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "오르간이 고장 난 것 같아. 근처에 있는 설명서를 읽어보자." } },
                { LanguageType.English,  new[] { "The organ seems to be broken. Let's read the manual nearby." } },
                { LanguageType.Japanese, new[] { "オルガンが壊れてるみたい。近くの説明書を読んでみよう。" } }
            }
        },

        // ── Step 2 : 파이프 교체 시작 ──
        {
            HintLanguageDataKey.HintOrgan_Step2_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "책에 나온 대로 오르간을 고쳐야 해." } },
                { LanguageType.English,  new[] { "You need to fix the organ the way the book says." } },
                { LanguageType.Japanese, new[] { "本に書いてあるとおりにオルガンを直さなきゃ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step2_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "책에 나온 대로 오르간 파이프를 교체해야 해." } },
                { LanguageType.English,  new[] { "You need to replace the organ pipes the way the book says." } },
                { LanguageType.Japanese, new[] { "本に書いてあるとおりにオルガンのパイプを交換しなきゃ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step2_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "책에 나온 대로 오르간 파이프를 교체해야 해. 근처에 파이프가 있는지 찾아봐." } },
                { LanguageType.English,  new[] { "You need to replace the organ pipes the way the book says. Look around for pipes nearby." } },
                { LanguageType.Japanese, new[] { "本に書いてあるとおりにオルガンのパイプを交換しなきゃ。近くにパイプがないか探してみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step2_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "책에 나온 대로 오르간 파이프를 교체해야 해. 근처에 파이프가 있는지 찾아봐." } },
                { LanguageType.English,  new[] { "You need to replace the organ pipes the way the book says. Look around for pipes nearby." } },
                { LanguageType.Japanese, new[] { "本に書いてあるとおりにオルガンのパイプを交換しなきゃ。近くにパイプがないか探してみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step2_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "책에 나온 대로 오르간 파이프를 교체해야 해. 근처에 파이프가 있는지 찾아봐." } },
                { LanguageType.English,  new[] { "You need to replace the organ pipes the way the book says. Look around for pipes nearby." } },
                { LanguageType.Japanese, new[] { "本に書いてあるとおりにオルガンのパイプを交換しなきゃ。近くにパイプがないか探してみて。" } }
            }
        },

        // ── Step 3 : 파이프 1·2 ──
        {
            HintLanguageDataKey.HintOrgan_Step3_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "1층에 파이프가 두 개 있어." } },
                { LanguageType.English,  new[] { "There are two pipes on the first floor." } },
                { LanguageType.Japanese, new[] { "1階にパイプが2本あるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step3_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "1층에 파이프가 두 개 있어." } },
                { LanguageType.English,  new[] { "There are two pipes on the first floor." } },
                { LanguageType.Japanese, new[] { "1階にパイプが2本あるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step3_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "오르간 계단 뒤쪽 공간에 파이프 하나가 있고 근처에 있는 방에도 파이프 하나가 있어." } },
                { LanguageType.English,  new[] { "There's a pipe in the space behind the organ stairs, and another one in a room nearby." } },
                { LanguageType.Japanese, new[] { "オルガンの階段の裏にパイプが1本、近くの部屋にももう1本あるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step3_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "오르간 계단 뒤쪽 공간에 파이프 하나가 있고 근처에 있는 방에도 파이프 하나가 있어." } },
                { LanguageType.English,  new[] { "There's a pipe in the space behind the organ stairs, and another one in a room nearby." } },
                { LanguageType.Japanese, new[] { "オルガンの階段の裏にパイプが1本、近くの部屋にももう1本あるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step3_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "오르간 계단 뒤쪽 공간에 파이프 하나가 있고 근처에 있는 방에도 파이프 하나가 있어." } },
                { LanguageType.English,  new[] { "There's a pipe in the space behind the organ stairs, and another one in a room nearby." } },
                { LanguageType.Japanese, new[] { "オルガンの階段の裏にパイプが1本、近くの部屋にももう1本あるよ。" } }
            }
        },

        // ── Step 4 : 오르골 ──
        {
            HintLanguageDataKey.HintOrgan_Step4_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "오르간 뒤쪽 공간에 오르골이 있어." } },
                { LanguageType.English,  new[] { "There's a music box in the space behind the organ." } },
                { LanguageType.Japanese, new[] { "オルガンの裏にオルゴールがあるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step4_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "오르간 뒤쪽 공간에 오르골이 있어." } },
                { LanguageType.English,  new[] { "There's a music box in the space behind the organ." } },
                { LanguageType.Japanese, new[] { "オルガンの裏にオルゴールがあるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step4_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "오르간 뒤쪽 공간에 오르골이 있어. 한번 재생해봐." } },
                { LanguageType.English,  new[] { "There's a music box in the space behind the organ. Try playing it." } },
                { LanguageType.Japanese, new[] { "オルガンの裏にオルゴールがあるよ。一度鳴らしてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step4_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "오르간 뒤쪽 공간에 오르골이 있어. 한번 재생해봐." } },
                { LanguageType.English,  new[] { "There's a music box in the space behind the organ. Try playing it." } },
                { LanguageType.Japanese, new[] { "オルガンの裏にオルゴールがあるよ。一度鳴らしてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step4_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "오르간 뒤쪽 공간에 오르골이 있어. 한번 재생해봐." } },
                { LanguageType.English,  new[] { "There's a music box in the space behind the organ. Try playing it." } },
                { LanguageType.Japanese, new[] { "オルガンの裏にオルゴールがあるよ。一度鳴らしてみて。" } }
            }
        },

        // ── Step 5 : 2층 키패드 ──
        {
            HintLanguageDataKey.HintOrgan_Step5_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "2층에 비밀번호 푸는 방이 있을 거야." } },
                { LanguageType.English,  new[] { "There should be a room with a code lock on the second floor." } },
                { LanguageType.Japanese, new[] { "2階に暗証番号を解く部屋があるはずだよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step5_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "2층에 비밀번호 푸는 방이 있을 거야." } },
                { LanguageType.English,  new[] { "There should be a room with a code lock on the second floor." } },
                { LanguageType.Japanese, new[] { "2階に暗証番号を解く部屋があるはずだよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step5_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "2층에 비밀번호 푸는 방이 있을 거야. 단서는 이미 확인했어." } },
                { LanguageType.English,  new[] { "There should be a room with a code lock on the second floor. You've already seen the clue." } },
                { LanguageType.Japanese, new[] { "2階に暗証番号を解く部屋があるはずだよ。手がかりはもう見てるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step5_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "2층에 비밀번호 푸는 방이 있을 거야. 단서는 이미 확인했어." } },
                { LanguageType.English,  new[] { "There should be a room with a code lock on the second floor. You've already seen the clue." } },
                { LanguageType.Japanese, new[] { "2階に暗証番号を解く部屋があるはずだよ。手がかりはもう見てるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step5_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "2층에 비밀번호 푸는 방이 있을 거야. 단서는 이미 확인했어." } },
                { LanguageType.English,  new[] { "There should be a room with a code lock on the second floor. You've already seen the clue." } },
                { LanguageType.Japanese, new[] { "2階に暗証番号を解く部屋があるはずだよ。手がかりはもう見てるよ。" } }
            }
        },

        // ── Step 6 : 비밀번호 방 ──
        {
            HintLanguageDataKey.HintOrgan_Step6_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "그러고 보니 비밀번호를 누를 때에도 소리가 났던 것 같아." } },
                { LanguageType.English,  new[] { "Come to think of it, the keypad made sounds when you pressed it too." } },
                { LanguageType.Japanese, new[] { "そういえば、暗証番号を押したときも音が鳴ってた気がする。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step6_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "그러고 보니 비밀번호를 누를 때에도 소리가 났던 것 같아." } },
                { LanguageType.English,  new[] { "Come to think of it, the keypad made sounds when you pressed it too." } },
                { LanguageType.Japanese, new[] { "そういえば、暗証番号を押したときも音が鳴ってた気がする。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step6_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "오르골과 비밀번호에서 같은 소리가 나도록 눌러야 해." } },
                { LanguageType.English,  new[] { "Press the keys so they make the same sounds as the music box." } },
                { LanguageType.Japanese, new[] { "オルゴールと同じ音が鳴るように暗証番号を押さなきゃ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step6_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "오르골과 비밀번호에서 같은 소리가 나도록 눌러야 해." } },
                { LanguageType.English,  new[] { "Press the keys so they make the same sounds as the music box." } },
                { LanguageType.Japanese, new[] { "オルゴールと同じ音が鳴るように暗証番号を押さなきゃ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step6_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "오르골과 비밀번호에서 같은 소리가 나도록 눌러야 해. 음정에 집중해봐." } },
                { LanguageType.English,  new[] { "Press the keys so they make the same sounds as the music box. Focus on the pitch." } },
                { LanguageType.Japanese, new[] { "オルゴールと同じ音が鳴るように暗証番号を押さなきゃ。音の高さに集中してみて。" } }
            }
        },

        // ── Step 7 : 파이프 3 ──
        {
            HintLanguageDataKey.HintOrgan_Step7_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "비밀번호를 풀었으니 뭔가 다른 게 있겠지." } },
                { LanguageType.English,  new[] { "You cracked the code, so there must be something else in there." } },
                { LanguageType.Japanese, new[] { "暗証番号を解いたんだから、何か別のものがあるはず。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step7_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "비밀번호를 풀었으니 거기에 파이프가 있겠지." } },
                { LanguageType.English,  new[] { "You cracked the code, so there must be a pipe in there." } },
                { LanguageType.Japanese, new[] { "暗証番号を解いたんだから、そこにパイプがあるはず。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step7_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "비밀번호로 푼 방에 파이프가 서랍 위에 올려져 있어." } },
                { LanguageType.English,  new[] { "In the room you unlocked, there's a pipe sitting on the drawers." } },
                { LanguageType.Japanese, new[] { "暗証番号で開けた部屋の引き出しの上にパイプが置いてあるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step7_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "비밀번호로 푼 방에 파이프가 서랍 위에 올려져 있어." } },
                { LanguageType.English,  new[] { "In the room you unlocked, there's a pipe sitting on the drawers." } },
                { LanguageType.Japanese, new[] { "暗証番号で開けた部屋の引き出しの上にパイプが置いてあるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step7_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "설마 이걸 못 찾는다고? 비밀번호로 푼 방에 파이프가 서랍 위에 올려져 있어." } },
                { LanguageType.English,  new[] { "Don't tell me you can't find it. In the room you unlocked, there's a pipe sitting on the drawers." } },
                { LanguageType.Japanese, new[] { "まさか見つからないの？暗証番号で開けた部屋の引き出しの上にパイプが置いてあるよ。" } }
            }
        },

        // ── Step 8 : 축음기 옆 책 ──
        {
            HintLanguageDataKey.HintOrgan_Step8_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "축음기 옆에 책이 하나 있어." } },
                { LanguageType.English,  new[] { "There's a book next to the gramophone." } },
                { LanguageType.Japanese, new[] { "蓄音機の横に本が1冊あるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step8_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "축음기 옆에 책이 하나 있어." } },
                { LanguageType.English,  new[] { "There's a book next to the gramophone." } },
                { LanguageType.Japanese, new[] { "蓄音機の横に本が1冊あるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step8_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "축음기 옆에 책이 하나 있는데 그 책을 봐야 해." } },
                { LanguageType.English,  new[] { "There's a book next to the gramophone, and you need to read it." } },
                { LanguageType.Japanese, new[] { "蓄音機の横に本が1冊あるから、それを読まなきゃ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step8_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "축음기 옆에 책이 하나 있는데 그 책을 봐야 해." } },
                { LanguageType.English,  new[] { "There's a book next to the gramophone, and you need to read it." } },
                { LanguageType.Japanese, new[] { "蓄音機の横に本が1冊あるから、それを読まなきゃ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step8_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "축음기 옆에 책이 하나 있는데 그 책을 봐야 해." } },
                { LanguageType.English,  new[] { "There's a book next to the gramophone, and you need to read it." } },
                { LanguageType.Japanese, new[] { "蓄音機の横に本が1冊あるから、それを読まなきゃ。" } }
            }
        },

        // ── Step 9 : LP 찾기 시작 ──
        {
            HintLanguageDataKey.HintOrgan_Step9_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "책에서 읽은 곡에 대한 설명과 일치하는 그림을 찾아봐." } },
                { LanguageType.English,  new[] { "Look for a painting that matches the song descriptions you read in the book." } },
                { LanguageType.Japanese, new[] { "本で読んだ曲の説明と一致する絵を探してみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step9_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "책에서 읽은 곡에 대한 설명과 일치하는 그림을 찾아봐." } },
                { LanguageType.English,  new[] { "Look for a painting that matches the song descriptions you read in the book." } },
                { LanguageType.Japanese, new[] { "本で読んだ曲の説明と一致する絵を探してみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step9_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "책에서 읽은 곡에 대한 설명과 일치하는 그림을 찾아봐. 근처에 LP가 있을 거야." } },
                { LanguageType.English,  new[] { "Look for a painting that matches the song descriptions you read in the book. There should be a record nearby." } },
                { LanguageType.Japanese, new[] { "本で読んだ曲の説明と一致する絵を探してみて。近くにレコードがあるはずだよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step9_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "책에서 읽은 곡에 대한 설명과 일치하는 그림을 찾아봐. 근처에 LP가 있을 거야." } },
                { LanguageType.English,  new[] { "Look for a painting that matches the song descriptions you read in the book. There should be a record nearby." } },
                { LanguageType.Japanese, new[] { "本で読んだ曲の説明と一致する絵を探してみて。近くにレコードがあるはずだよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step9_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "책에서 읽은 곡에 대한 설명과 일치하는 그림을 찾아봐. 근처에 LP가 있을 거야." } },
                { LanguageType.English,  new[] { "Look for a painting that matches the song descriptions you read in the book. There should be a record nearby." } },
                { LanguageType.Japanese, new[] { "本で読んだ曲の説明と一致する絵を探してみて。近くにレコードがあるはずだよ。" } }
            }
        },

        // ── Step 10 : 1층 LP ──
        {
            HintLanguageDataKey.HintOrgan_Step10_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "1층 가장 넓은 공간에 LP가 2개 있고 방에도 LP가 1개 있어." } },
                { LanguageType.English,  new[] { "There are two records in the biggest space on the first floor, and one more in a room." } },
                { LanguageType.Japanese, new[] { "1階でいちばん広い場所にレコードが2枚、部屋にも1枚あるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step10_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "1층 가장 넓은 공간에 LP가 2개 있고 방에도 LP가 1개 있어." } },
                { LanguageType.English,  new[] { "There are two records in the biggest space on the first floor, and one more in a room." } },
                { LanguageType.Japanese, new[] { "1階でいちばん広い場所にレコードが2枚、部屋にも1枚あるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step10_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "1층 가장 넓은 공간 마주 보는 벽에 LP가 2개 있고 방에도 LP가 1개 있어." } },
                { LanguageType.English,  new[] { "There are two records on the facing walls of the biggest space on the first floor, and one more in a room." } },
                { LanguageType.Japanese, new[] { "1階でいちばん広い場所の向かい合った壁にレコードが2枚、部屋にも1枚あるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step10_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "1층 가장 넓은 공간 마주 보는 벽에 LP가 2개 있고 방에도 LP가 1개 있어." } },
                { LanguageType.English,  new[] { "There are two records on the facing walls of the biggest space on the first floor, and one more in a room." } },
                { LanguageType.Japanese, new[] { "1階でいちばん広い場所の向かい合った壁にレコードが2枚、部屋にも1枚あるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step10_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "1층 가장 넓은 공간 마주 보는 벽에 LP가 2개 있고 방에도 LP가 1개 있어." } },
                { LanguageType.English,  new[] { "There are two records on the facing walls of the biggest space on the first floor, and one more in a room." } },
                { LanguageType.Japanese, new[] { "1階でいちばん広い場所の向かい合った壁にレコードが2枚、部屋にも1枚あるよ。" } }
            }
        },

        // ── Step 11 : 2층 LP ──
        {
            HintLanguageDataKey.HintOrgan_Step11_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "2층 곳곳을 돌아다니다 보면 LP를 총 2개 발견할 수 있을 거야." } },
                { LanguageType.English,  new[] { "If you wander around the second floor, you should find two records in total." } },
                { LanguageType.Japanese, new[] { "2階をあちこち回ってみたら、レコードが全部で2枚見つかるはずだよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step11_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "2층 곳곳을 돌아다니다 보면 LP를 총 2개 발견할 수 있을 거야." } },
                { LanguageType.English,  new[] { "If you wander around the second floor, you should find two records in total." } },
                { LanguageType.Japanese, new[] { "2階をあちこち回ってみたら、レコードが全部で2枚見つかるはずだよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step11_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "2층 곳곳을 돌아다니다 보면 LP를 총 2개 발견할 수 있을 거야. 계단과 거리가 먼 곳에 있어." } },
                { LanguageType.English,  new[] { "If you wander around the second floor, you should find two records in total. They're far from the stairs." } },
                { LanguageType.Japanese, new[] { "2階をあちこち回ってみたら、レコードが全部で2枚見つかるはずだよ。階段から離れた場所にあるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step11_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "2층 곳곳을 돌아다니다 보면 LP를 총 2개 발견할 수 있을 거야. 계단과 거리가 먼 곳에 있어." } },
                { LanguageType.English,  new[] { "If you wander around the second floor, you should find two records in total. They're far from the stairs." } },
                { LanguageType.Japanese, new[] { "2階をあちこち回ってみたら、レコードが全部で2枚見つかるはずだよ。階段から離れた場所にあるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step11_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "2층 곳곳을 돌아다니다 보면 LP를 총 2개 발견할 수 있을 거야. 계단과 거리가 먼 곳에 있어." } },
                { LanguageType.English,  new[] { "If you wander around the second floor, you should find two records in total. They're far from the stairs." } },
                { LanguageType.Japanese, new[] { "2階をあちこち回ってみたら、レコードが全部で2枚見つかるはずだよ。階段から離れた場所にあるよ。" } }
            }
        },

        // ── Step 12 : 파이프 4개 교체 ──
        {
            HintLanguageDataKey.HintOrgan_Step12_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "먼저 오르간에 있는 파이프를 빼고 새로 찾은 파이프를 교체해봐." } },
                { LanguageType.English,  new[] { "First pull out the pipes in the organ, then swap in the ones you found." } },
                { LanguageType.Japanese, new[] { "まずオルガンのパイプを抜いて、見つけたパイプに交換してみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step12_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "먼저 오르간에 있는 파이프를 빼고 새로 찾은 파이프를 교체해봐." } },
                { LanguageType.English,  new[] { "First pull out the pipes in the organ, then swap in the ones you found." } },
                { LanguageType.Japanese, new[] { "まずオルガンのパイプを抜いて、見つけたパイプに交換してみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step12_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "이미 파이프를 다 찾았어. 먼저 오르간에 있는 파이프를 빼고 새로 찾은 파이프를 교체해봐." } },
                { LanguageType.English,  new[] { "You've already found all the pipes. First pull out the pipes in the organ, then swap in the ones you found." } },
                { LanguageType.Japanese, new[] { "パイプはもう全部見つけたよ。まずオルガンのパイプを抜いて、見つけたパイプに交換してみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step12_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "이미 파이프를 다 찾았어. 먼저 오르간에 있는 파이프를 빼고 새로 찾은 파이프를 교체해봐." } },
                { LanguageType.English,  new[] { "You've already found all the pipes. First pull out the pipes in the organ, then swap in the ones you found." } },
                { LanguageType.Japanese, new[] { "パイプはもう全部見つけたよ。まずオルガンのパイプを抜いて、見つけたパイプに交換してみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step12_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "이미 파이프를 다 찾았어. 먼저 오르간에 있는 파이프를 빼고 새로 찾은 파이프를 교체해봐." } },
                { LanguageType.English,  new[] { "You've already found all the pipes. First pull out the pipes in the organ, then swap in the ones you found." } },
                { LanguageType.Japanese, new[] { "パイプはもう全部見つけたよ。まずオルガンのパイプを抜いて、見つけたパイプに交換してみて。" } }
            }
        },

        // ── Step 13 : 악보 찾기 시작 ──
        {
            HintLanguageDataKey.HintOrgan_Step13_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "어딘가에 숨겨진 악보를 찾아야 할 것 같아." } },
                { LanguageType.English,  new[] { "Looks like you need to find some hidden sheet music." } },
                { LanguageType.Japanese, new[] { "どこかに隠された楽譜を探さなきゃいけないみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step13_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "어딘가에 숨겨진 악보를 찾아야 할 것 같아." } },
                { LanguageType.English,  new[] { "Looks like you need to find some hidden sheet music." } },
                { LanguageType.Japanese, new[] { "どこかに隠された楽譜を探さなきゃいけないみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step13_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "어딘가에 숨겨진 악보를 찾아서 오르간을 연주해야 할 것 같아." } },
                { LanguageType.English,  new[] { "Looks like you need to find the hidden sheet music and play the organ." } },
                { LanguageType.Japanese, new[] { "どこかに隠された楽譜を見つけて、オルガンを弾かなきゃいけないみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step13_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "어딘가에 숨겨진 악보를 찾아서 오르간을 연주해야 할 것 같아." } },
                { LanguageType.English,  new[] { "Looks like you need to find the hidden sheet music and play the organ." } },
                { LanguageType.Japanese, new[] { "どこかに隠された楽譜を見つけて、オルガンを弾かなきゃいけないみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step13_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "어딘가에 숨겨진 악보를 찾아서 오르간을 연주해야 할 것 같아." } },
                { LanguageType.English,  new[] { "Looks like you need to find the hidden sheet music and play the organ." } },
                { LanguageType.Japanese, new[] { "どこかに隠された楽譜を見つけて、オルガンを弾かなきゃいけないみたい。" } }
            }
        },

        // ── Step 14 : 떨어진 책 속 악보 ──
        {
            HintLanguageDataKey.HintOrgan_Step14_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "갑자기 책들이 떨어졌어." } },
                { LanguageType.English,  new[] { "Some books suddenly fell down." } },
                { LanguageType.Japanese, new[] { "急に本が落ちてきたね。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step14_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "갑자기 책들이 떨어졌어." } },
                { LanguageType.English,  new[] { "Some books suddenly fell down." } },
                { LanguageType.Japanese, new[] { "急に本が落ちてきたね。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step14_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "갑자기 책들이 떨어졌어. 그 책 사이에 악보가 있는 것 같아." } },
                { LanguageType.English,  new[] { "Some books suddenly fell down. There seems to be sheet music among them." } },
                { LanguageType.Japanese, new[] { "急に本が落ちてきたね。その本の間に楽譜があるみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step14_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "갑자기 책들이 떨어졌어. 그 책 사이에 악보가 있는 것 같아." } },
                { LanguageType.English,  new[] { "Some books suddenly fell down. There seems to be sheet music among them." } },
                { LanguageType.Japanese, new[] { "急に本が落ちてきたね。その本の間に楽譜があるみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step14_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "갑자기 책들이 떨어졌어. 그 책 사이에 악보가 있는 것 같아." } },
                { LanguageType.English,  new[] { "Some books suddenly fell down. There seems to be sheet music among them." } },
                { LanguageType.Japanese, new[] { "急に本が落ちてきたね。その本の間に楽譜があるみたい。" } }
            }
        },

        // ── Step 15 : 비밀 통로 ──
        {
            HintLanguageDataKey.HintOrgan_Step15_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "아까 무언가 부서지는 소리가 들리지 않았어?" } },
                { LanguageType.English,  new[] { "Didn't you hear something break a moment ago?" } },
                { LanguageType.Japanese, new[] { "さっき何かが壊れる音がしなかった？" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step15_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "아까 무언가 부서지는 소리가 들리지 않았어?" } },
                { LanguageType.English,  new[] { "Didn't you hear something break a moment ago?" } },
                { LanguageType.Japanese, new[] { "さっき何かが壊れる音がしなかった？" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step15_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "아까 무언가 부서지는 소리가 들리지 않았어? 어딘가 공간이 열린 것 같아." } },
                { LanguageType.English,  new[] { "Didn't you hear something break a moment ago? Looks like a space opened up somewhere." } },
                { LanguageType.Japanese, new[] { "さっき何かが壊れる音がしなかった？どこかの空間が開いたみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step15_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "아까 무언가 부서지는 소리가 들리지 않았어? 어딘가 공간이 열린 것 같아." } },
                { LanguageType.English,  new[] { "Didn't you hear something break a moment ago? Looks like a space opened up somewhere." } },
                { LanguageType.Japanese, new[] { "さっき何かが壊れる音がしなかった？どこかの空間が開いたみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step15_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "아까 무언가 부서지는 소리가 들리지 않았어? 막혀 있던 공간이 열린 것 같아." } },
                { LanguageType.English,  new[] { "Didn't you hear something break a moment ago? Looks like a blocked-off space opened up." } },
                { LanguageType.Japanese, new[] { "さっき何かが壊れる音がしなかった？ふさがってた場所が開いたみたい。" } }
            }
        },

        // ── Step 16 : 메트로놈 ──
        {
            HintLanguageDataKey.HintOrgan_Step16_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "메트로놈 바늘을 올바른 타이밍에 눌러야 해." } },
                { LanguageType.English,  new[] { "You need to press the metronome needle at the right moment." } },
                { LanguageType.Japanese, new[] { "メトロノームの針を正しいタイミングで押さなきゃ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step16_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "메트로놈 바늘을 올바른 타이밍에 눌러야 해." } },
                { LanguageType.English,  new[] { "You need to press the metronome needle at the right moment." } },
                { LanguageType.Japanese, new[] { "メトロノームの針を正しいタイミングで押さなきゃ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step16_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "메트로놈 바늘이 그림 액자 속 붉은색에 있을 때 버튼을 눌러야 하는 것 같아." } },
                { LanguageType.English,  new[] { "Looks like you need to press the button when the needle is on the red part shown in the framed picture." } },
                { LanguageType.Japanese, new[] { "メトロノームの針が額縁の絵の赤い部分にあるときにボタンを押せばいいみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step16_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "메트로놈 바늘이 그림 액자 속 붉은색에 있을 때 버튼을 눌러야 하는 것 같아." } },
                { LanguageType.English,  new[] { "Looks like you need to press the button when the needle is on the red part shown in the framed picture." } },
                { LanguageType.Japanese, new[] { "メトロノームの針が額縁の絵の赤い部分にあるときにボタンを押せばいいみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step16_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "메트로놈 바늘이 그림 액자 속 붉은색에 있을 때 버튼을 눌러야 하는 것 같아." } },
                { LanguageType.English,  new[] { "Looks like you need to press the button when the needle is on the red part shown in the framed picture." } },
                { LanguageType.Japanese, new[] { "メトロノームの針が額縁の絵の赤い部分にあるときにボタンを押せばいいみたい。" } }
            }
        },

        // ── Step 17 : 캐비닛 악보 ──
        {
            HintLanguageDataKey.HintOrgan_Step17_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "메트로놈을 해결한 후 초록색 사물함이 열렸어." } },
                { LanguageType.English,  new[] { "After you solved the metronome, the green cabinet opened." } },
                { LanguageType.Japanese, new[] { "メトロノームを解いたら、緑のロッカーが開いたよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step17_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "메트로놈을 해결한 후 초록색 사물함이 열렸어." } },
                { LanguageType.English,  new[] { "After you solved the metronome, the green cabinet opened." } },
                { LanguageType.Japanese, new[] { "メトロノームを解いたら、緑のロッカーが開いたよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step17_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "초록색 사물함 안에 악보가 있었을 거야." } },
                { LanguageType.English,  new[] { "There should be sheet music inside the green cabinet." } },
                { LanguageType.Japanese, new[] { "緑のロッカーの中に楽譜があったはずだよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step17_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "초록색 사물함 안에 악보가 있었을 거야." } },
                { LanguageType.English,  new[] { "There should be sheet music inside the green cabinet." } },
                { LanguageType.Japanese, new[] { "緑のロッカーの中に楽譜があったはずだよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step17_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "초록색 사물함 안에 악보가 있었을 거야." } },
                { LanguageType.English,  new[] { "There should be sheet music inside the green cabinet." } },
                { LanguageType.Japanese, new[] { "緑のロッカーの中に楽譜があったはずだよ。" } }
            }
        },

        // ── Step 18 : 책장 만지기 ──
        {
            HintLanguageDataKey.HintOrgan_Step18_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "2층 책장에 있는 책들이 뭔가 수상해." } },
                { LanguageType.English,  new[] { "Something's off about the books on the second-floor bookshelf." } },
                { LanguageType.Japanese, new[] { "2階の本棚の本、なんだか怪しいね。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step18_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "2층 책장에 있는 책들이 뭔가 수상해." } },
                { LanguageType.English,  new[] { "Something's off about the books on the second-floor bookshelf." } },
                { LanguageType.Japanese, new[] { "2階の本棚の本、なんだか怪しいね。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step18_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "2층 책장에 있는 책들을 하나씩 밀거나 당길 수 있는 것 같아." } },
                { LanguageType.English,  new[] { "It seems you can push or pull the books on the second-floor bookshelf one by one." } },
                { LanguageType.Japanese, new[] { "2階の本棚の本は、1冊ずつ押したり引いたりできるみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step18_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "2층 책장에 있는 책들을 하나씩 밀거나 당길 수 있는 것 같아." } },
                { LanguageType.English,  new[] { "It seems you can push or pull the books on the second-floor bookshelf one by one." } },
                { LanguageType.Japanese, new[] { "2階の本棚の本は、1冊ずつ押したり引いたりできるみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step18_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "2층 책장에 있는 책들을 하나씩 밀거나 당길 수 있는 것 같아." } },
                { LanguageType.English,  new[] { "It seems you can push or pull the books on the second-floor bookshelf one by one." } },
                { LanguageType.Japanese, new[] { "2階の本棚の本は、1冊ずつ押したり引いたりできるみたい。" } }
            }
        },

        // ── Step 19 : 책장 열기 ──
        {
            HintLanguageDataKey.HintOrgan_Step19_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "메트로놈이 있던 방에 책장을 움직일 수 있는 단서가 있어." } },
                { LanguageType.English,  new[] { "The room with the metronome has a clue for moving the bookshelf." } },
                { LanguageType.Japanese, new[] { "メトロノームがあった部屋に、本棚を動かす手がかりがあるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step19_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "메트로놈이 있던 방에 책장을 움직일 수 있는 단서가 있어." } },
                { LanguageType.English,  new[] { "The room with the metronome has a clue for moving the bookshelf." } },
                { LanguageType.Japanese, new[] { "メトロノームがあった部屋に、本棚を動かす手がかりがあるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step19_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "초록색 사물함 안에 책장을 움직일 수 있는 단서가 있어." } },
                { LanguageType.English,  new[] { "The green cabinet has a clue for moving the bookshelf." } },
                { LanguageType.Japanese, new[] { "緑のロッカーの中に、本棚を動かす手がかりがあるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step19_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "초록색 사물함 안에 책장을 움직일 수 있는 단서가 있어." } },
                { LanguageType.English,  new[] { "The green cabinet has a clue for moving the bookshelf." } },
                { LanguageType.Japanese, new[] { "緑のロッカーの中に、本棚を動かす手がかりがあるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step19_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "초록색 사물함 안에 책장을 움직일 수 있는 단서가 있어." } },
                { LanguageType.English,  new[] { "The green cabinet has a clue for moving the bookshelf." } },
                { LanguageType.Japanese, new[] { "緑のロッカーの中に、本棚を動かす手がかりがあるよ。" } }
            }
        },

        // ── Step 20 : 책상 위 악보 ──
        {
            HintLanguageDataKey.HintOrgan_Step20_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "책장 뒤에 있던 방에 악보 조각 하나가 있던데." } },
                { LanguageType.English,  new[] { "There was a piece of sheet music in the room behind the bookshelf." } },
                { LanguageType.Japanese, new[] { "本棚の裏の部屋に楽譜の切れ端が1枚あったよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step20_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "책장 뒤에 있던 방에 악보 조각 하나가 있던데." } },
                { LanguageType.English,  new[] { "There was a piece of sheet music in the room behind the bookshelf." } },
                { LanguageType.Japanese, new[] { "本棚の裏の部屋に楽譜の切れ端が1枚あったよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step20_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "책상 위에서 악보 조각 하나를 본 거 같은데." } },
                { LanguageType.English,  new[] { "I think I saw a piece of sheet music on the desk." } },
                { LanguageType.Japanese, new[] { "机の上で楽譜の切れ端を1枚見た気がする。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step20_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "책상 위에서 악보 조각 하나를 본 거 같은데." } },
                { LanguageType.English,  new[] { "I think I saw a piece of sheet music on the desk." } },
                { LanguageType.Japanese, new[] { "机の上で楽譜の切れ端を1枚見た気がする。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step20_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "책상 위에서 악보 조각 하나를 본 거 같은데." } },
                { LanguageType.English,  new[] { "I think I saw a piece of sheet music on the desk." } },
                { LanguageType.Japanese, new[] { "机の上で楽譜の切れ端を1枚見た気がする。" } }
            }
        },

        // ── Step 21 : 가족사진 속 악보 ──
        {
            HintLanguageDataKey.HintOrgan_Step21_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "방에 가족사진이 있었지? 거기에서 이질감이 느껴져." } },
                { LanguageType.English,  new[] { "There was a family photo in the room, right? Something feels off about it." } },
                { LanguageType.Japanese, new[] { "部屋に家族写真があったよね？なんだか違和感があるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step21_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "방에 가족사진이 있었지? 거기에서 이질감이 느껴져." } },
                { LanguageType.English,  new[] { "There was a family photo in the room, right? Something feels off about it." } },
                { LanguageType.Japanese, new[] { "部屋に家族写真があったよね？なんだか違和感があるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step21_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "방에 가족사진이 있었지? 사진을 빼서 조사해봐." } },
                { LanguageType.English,  new[] { "There was a family photo in the room, right? Take the photo out and check it." } },
                { LanguageType.Japanese, new[] { "部屋に家族写真があったよね？写真を取り出して調べてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step21_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "방에 가족사진이 있었지? 사진을 빼서 조사해봐." } },
                { LanguageType.English,  new[] { "There was a family photo in the room, right? Take the photo out and check it." } },
                { LanguageType.Japanese, new[] { "部屋に家族写真があったよね？写真を取り出して調べてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step21_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "방에 가족사진이 있었지? 사진을 빼서 조사해봐." } },
                { LanguageType.English,  new[] { "There was a family photo in the room, right? Take the photo out and check it." } },
                { LanguageType.Japanese, new[] { "部屋に家族写真があったよね？写真を取り出して調べてみて。" } }
            }
        },

        // ── Step 22 : 악보 조합 ──
        {
            HintLanguageDataKey.HintOrgan_Step22_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "찾은 악보를 오르간 위에 올려놔 보자." } },
                { LanguageType.English,  new[] { "Let's put the sheet music you found on the organ." } },
                { LanguageType.Japanese, new[] { "見つけた楽譜をオルガンの上に置いてみよう。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step22_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "찾은 악보를 오르간 위에 올려놔 보자." } },
                { LanguageType.English,  new[] { "Let's put the sheet music you found on the organ." } },
                { LanguageType.Japanese, new[] { "見つけた楽譜をオルガンの上に置いてみよう。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step22_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "악보를 다 찾았어. 찾은 악보를 오르간 위에 올려놔 보자." } },
                { LanguageType.English,  new[] { "You've found all the sheet music. Let's put it on the organ." } },
                { LanguageType.Japanese, new[] { "楽譜は全部見つけたよ。オルガンの上に置いてみよう。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step22_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "악보를 다 찾았어. 찾은 악보를 오르간 위에 올려놔 보자." } },
                { LanguageType.English,  new[] { "You've found all the sheet music. Let's put it on the organ." } },
                { LanguageType.Japanese, new[] { "楽譜は全部見つけたよ。オルガンの上に置いてみよう。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step22_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "악보를 다 찾았어. 찾은 악보를 오르간 위에 올려놔 보자." } },
                { LanguageType.English,  new[] { "You've found all the sheet music. Let's put it on the organ." } },
                { LanguageType.Japanese, new[] { "楽譜は全部見つけたよ。オルガンの上に置いてみよう。" } }
            }
        },

        // ── Step 23 : 오르간 연주 ──
        {
            HintLanguageDataKey.HintOrgan_Step23_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "악보를 연주해봐." } },
                { LanguageType.English,  new[] { "Try playing the sheet music." } },
                { LanguageType.Japanese, new[] { "楽譜を弾いてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step23_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "악보를 연주해봐." } },
                { LanguageType.English,  new[] { "Try playing the sheet music." } },
                { LanguageType.Japanese, new[] { "楽譜を弾いてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step23_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "악보에 적힌 숫자대로 건반을 누르면 될 거야." } },
                { LanguageType.English,  new[] { "Just press the keys in the order of the numbers on the sheet music." } },
                { LanguageType.Japanese, new[] { "楽譜に書いてある数字のとおりに鍵盤を押せばいいはずだよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step23_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "악보에 적힌 숫자대로 건반을 누르면 될 거야." } },
                { LanguageType.English,  new[] { "Just press the keys in the order of the numbers on the sheet music." } },
                { LanguageType.Japanese, new[] { "楽譜に書いてある数字のとおりに鍵盤を押せばいいはずだよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintOrgan_Step23_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "악보에 적힌 숫자대로 건반을 누르면 될 거야." } },
                { LanguageType.English,  new[] { "Just press the keys in the order of the numbers on the sheet music." } },
                { LanguageType.Japanese, new[] { "楽譜に書いてある数字のとおりに鍵盤を押せばいいはずだよ。" } }
            }
        }
    };
}