using System.Collections.Generic;

/// <summary>
/// 와인씬 번역 데이터 — 구역·랜드마크 이름 / 단계별 힌트(hintByLevel) / 진행 문장(WineHintBridge).
/// 구역·랜드마크: WineLocationProvider Inspector의 Display Name / Landmark Name 칸에 키를 적으면 관찰 문장에 현재 언어 이름으로 들어감.
/// 단계별 힌트·진행 문장: LLM 힌트의 [Hint direction]과 LLM이 없을 때의 고정 문구(폴백) 둘 다 이 문장을 씀.
/// 와인씬 1단계 문장은 모든 레벨·구간에서 "알파벳"(영어 letters / 일본어 アルファベット)을 꼭 말함 (피드백 반영).
/// 정답 글자·숫자·칸 번호는 어떤 문장에도 넣지 않음.
/// </summary>
public static partial class HintLanguageData
{
    private static readonly Dictionary<string, Dictionary<LanguageType, string[]>> WineSceneData = new()
    {
        /*
         * Zones
         */

        {
            HintLanguageDataKey.HintWine_ZoneRoom1,
            new()
            {
                { LanguageType.Korean,   new[] { "첫 번째 방" } },
                { LanguageType.English,  new[] { "the first room" } },
                { LanguageType.Japanese, new[] { "一つ目の部屋" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_ZoneRoom2,
            new()
            {
                { LanguageType.Korean,   new[] { "두 번째 방" } },
                { LanguageType.English,  new[] { "the second room" } },
                { LanguageType.Japanese, new[] { "二つ目の部屋" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_ZoneRoom3,
            new()
            {
                { LanguageType.Korean,   new[] { "세 번째 방" } },
                { LanguageType.English,  new[] { "the third room" } },
                { LanguageType.Japanese, new[] { "三つ目の部屋" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_ZoneCorridor,
            new()
            {
                { LanguageType.Korean,   new[] { "통로" } },
                { LanguageType.English,  new[] { "the corridor" } },
                { LanguageType.Japanese, new[] { "通路" } }
            }
        },

        /*
         * Landmarks
         */

        {
            HintLanguageDataKey.HintWine_LandmarkWallPaper,
            new()
            {
                { LanguageType.Korean,   new[] { "벽 종이" } },
                { LanguageType.English,  new[] { "the paper on the wall" } },
                { LanguageType.Japanese, new[] { "壁の紙" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_LandmarkBookshelf,
            new()
            {
                { LanguageType.Korean,   new[] { "책장" } },
                { LanguageType.English,  new[] { "the bookshelf" } },
                { LanguageType.Japanese, new[] { "本棚" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_LandmarkWineRack,
            new()
            {
                { LanguageType.Korean,   new[] { "와인렉" } },
                { LanguageType.English,  new[] { "the wine rack" } },
                { LanguageType.Japanese, new[] { "ワインラック" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_LandmarkBox,
            new()
            {
                { LanguageType.Korean,   new[] { "상자" } },
                { LanguageType.English,  new[] { "the box" } },
                { LanguageType.Japanese, new[] { "箱" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_LandmarkMemo,
            new()
            {
                { LanguageType.Korean,   new[] { "액자 아래 종이" } },
                { LanguageType.English,  new[] { "the paper under the frame" } },
                { LanguageType.Japanese, new[] { "額縁の下の紙" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_LandmarkCabinet,
            new()
            {
                { LanguageType.Korean,   new[] { "장식장" } },
                { LanguageType.English,  new[] { "the display cabinet" } },
                { LanguageType.Japanese, new[] { "飾り棚" } }
            }
        },

        /*
         * Hint Levels (WineGlassRoomData hintByLevel — 레벨 1~5마다 문장 1개)
         */

        /*
         * Step 1
         */

        {
            HintLanguageDataKey.HintWine_Step1_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "바닥에 적힌 알파벳, 그냥 장식은 아닐 거야. 주변을 더 둘러봐." } },
                { LanguageType.English,  new[] { "The alphabet letters on the floor aren't just decoration. Look around some more." } },
                { LanguageType.Japanese, new[] { "床に書かれたアルファベット、ただの飾りじゃないはず。周りをもっと見てみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_Step1_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "바닥 알파벳 중에 뭘 봐야 하는지 알려주는 게 있어. 방과 방 사이 통로를 살펴봐." } },
                { LanguageType.English,  new[] { "Something tells you which floor letters to look at. Check the corridor between the rooms." } },
                { LanguageType.Japanese, new[] { "床のアルファベットのどれを見ればいいか教えてくれるものがある。部屋と部屋の間の通路を調べてみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_Step1_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "통로 벽에 붙은 종이가 바닥 알파벳 중 어떤 걸 봐야 하는지 알려줘. 찾아봐." } },
                { LanguageType.English,  new[] { "The paper on the corridor wall tells you which floor letters to look at. Find it." } },
                { LanguageType.Japanese, new[] { "通路の壁に貼られた紙が、床のアルファベットのどれを見ればいいか教えてくれる。探してみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_Step1_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "통로 벽 종이를 가까이서 봐. 그려진 얼룩 그림이 바닥 알파벳 중 어디를 볼지 알려줘." } },
                { LanguageType.English,  new[] { "Take a close look at the paper on the corridor wall. The stains drawn on it show which floor letters to look at." } },
                { LanguageType.Japanese, new[] { "通路の壁の紙を近くで見て。描かれたシミの絵が、床のアルファベットのどこを見ればいいか教えてくれる。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_Step1_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "통로 벽 종이의 얼룩 그림을 확대해서 보고, 같은 색 바닥 얼룩이 있는 자리의 알파벳을 확인해." } },
                { LanguageType.English,  new[] { "Zoom in on the stain drawing on the corridor wall paper, then check the letter under each floor stain of the same color." } },
                { LanguageType.Japanese, new[] { "通路の壁の紙のシミの絵を拡大して見て、同じ色の床のシミがある場所のアルファベットを確認して。" } }
            }
        },

        /*
         * Step 2
         */

        {
            HintLanguageDataKey.HintWine_Step2_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "적어둔 메모를 어디에 쓸 수 있을지 생각해봐." } },
                { LanguageType.English,  new[] { "Think about where you could use the memo you wrote." } },
                { LanguageType.Japanese, new[] { "書いたメモをどこで使えるか考えてみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_Step2_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "글자가 잔뜩 적힌 가구가 다른 방에 있어." } },
                { LanguageType.English,  new[] { "There's a piece of furniture covered in letters in another room." } },
                { LanguageType.Japanese, new[] { "文字がたくさん書かれた家具が別の部屋にある。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_Step2_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "책장을 가까이서 봐. 책 제목이 눈에 띌 거야." } },
                { LanguageType.English,  new[] { "Look closely at the bookshelf. The book titles will catch your eye." } },
                { LanguageType.Japanese, new[] { "本棚を近くで見て。本のタイトルが目に入るはず。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_Step2_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "메모에 적은 글자로 시작하는 책을 찾아봐. 그 책에 숫자가 있어." } },
                { LanguageType.English,  new[] { "Find the books that start with the letters on your memo. Those books have numbers." } },
                { LanguageType.Japanese, new[] { "メモに書いた文字で始まる本を探してみて。その本に数字がある。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_Step2_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "책장을 확대해서 메모의 각 글자로 시작하는 제목의 책을 찾고, 그 책에 적힌 숫자를 색깔별로 기억해둬." } },
                { LanguageType.English,  new[] { "Zoom in on the bookshelf, find the book whose title starts with each letter on your memo, and remember each book's number by color." } },
                { LanguageType.Japanese, new[] { "本棚を拡大して、メモのそれぞれの文字で始まるタイトルの本を探して、その本に書かれた数字を色ごとに覚えておいて。" } }
            }
        },

        /*
         * Step 3
         */

        {
            HintLanguageDataKey.HintWine_Step3_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "알아낸 숫자를 쓸 만한 곳이 있을 거야." } },
                { LanguageType.English,  new[] { "There must be somewhere to use the numbers you found." } },
                { LanguageType.Japanese, new[] { "見つけた数字を使える場所があるはず。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_Step3_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "와인렉의 빈 칸들을 살펴봐." } },
                { LanguageType.English,  new[] { "Check the empty slots on the wine rack." } },
                { LanguageType.Japanese, new[] { "ワインラックの空いているマスを調べてみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_Step3_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "와인렉 칸마다 번호가 있어. 책에서 본 숫자랑 연결해봐." } },
                { LanguageType.English,  new[] { "Every slot on the wine rack has a number. Connect them to the numbers from the books." } },
                { LanguageType.Japanese, new[] { "ワインラックのマスにはそれぞれ番号がある。本で見た数字とつなげてみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_Step3_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "병마다 해당하는 색이 있어. 그 색의 글자로 찾은 책 숫자 칸에 병을 꽂아봐." } },
                { LanguageType.English,  new[] { "Each bottle has its own color. Put it in the slot matching the number of the book you found with that color's letter." } },
                { LanguageType.Japanese, new[] { "ボトルにはそれぞれ色がある。その色の文字で見つけた本の数字のマスにボトルを差してみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_Step3_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "병 하나를 들고 그 병 색에 맞는 메모 글자를 떠올린 다음, 그 글자 책의 숫자랑 같은 번호의 와인렉 칸에 꽂아. 네 병 다 맞추면 돼." } },
                { LanguageType.English,  new[] { "Pick up a bottle, recall the memo letter for its color, then put it in the wine rack slot with the same number as that letter's book. Do it for all four bottles." } },
                { LanguageType.Japanese, new[] { "ボトルを一本持って、その色に合うメモの文字を思い出して、その文字の本の数字と同じ番号のワインラックのマスに差して。四本全部合わせればいい。" } }
            }
        },

        /*
         * Step 4
         */

        {
            HintLanguageDataKey.HintWine_Step4_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "방 안에 방금 달라진 곳이 있어." } },
                { LanguageType.English,  new[] { "Something in the room just changed." } },
                { LanguageType.Japanese, new[] { "部屋の中で今、何かが変わった。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_Step4_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "잠겨 있던 장식장을 다시 봐." } },
                { LanguageType.English,  new[] { "Take another look at the display cabinet that was locked." } },
                { LanguageType.Japanese, new[] { "鍵がかかっていた飾り棚をもう一度見て。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_Step4_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "장식장 안에 작은 물건이 있어." } },
                { LanguageType.English,  new[] { "There's something small inside the display cabinet." } },
                { LanguageType.Japanese, new[] { "飾り棚の中に小さな物がある。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_Step4_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "장식장 안의 열쇠를 꺼내서 쓸 곳을 찾아봐." } },
                { LanguageType.English,  new[] { "Take the key out of the display cabinet and find where to use it." } },
                { LanguageType.Japanese, new[] { "飾り棚の中の鍵を取り出して、使う場所を探してみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_Step4_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "열린 장식장에서 열쇠를 꺼내 상자에 넣으면 탈출구가 열려." } },
                { LanguageType.English,  new[] { "Take the key from the open display cabinet and put it in the box to open the way out." } },
                { LanguageType.Japanese, new[] { "開いた飾り棚から鍵を取り出して箱に入れれば、出口が開くよ。" } }
            }
        },

        /*
         * Progress Notes (WineHintBridge — 진행 구간별, T0 레벨1~2 / T1 레벨3 / T2 레벨4~5, 후보 중 랜덤)
         */

        /*
         * StainViewed
         */

        {
            HintLanguageDataKey.HintWine_NoteStainViewed_T0,
            new()
            {
                { LanguageType.Korean,   new[] { "그림을 봤으면 이제 바닥 알파벳을 볼 차례야.", "그림 속 색깔을 기억해두고 바닥 알파벳을 다시 둘러봐." } },
                { LanguageType.English,  new[] { "Now that you've seen the drawing, it's time to look at the alphabet letters on the floor.", "Remember the colors in the drawing and look over the floor letters again." } },
                { LanguageType.Japanese, new[] { "絵を見たなら、次は床のアルファベットを見る番だ。", "絵の中の色を覚えて、床のアルファベットをもう一度見て回って。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_NoteStainViewed_T1,
            new()
            {
                { LanguageType.Korean,   new[] { "그림 속 얼룩이랑 같은 색의 얼룩이 바닥에 있어. 얼룩이 있는 자리의 알파벳을 하나씩 가까이서 봐.", "바닥 얼룩을 그림 속 색깔이랑 하나씩 맞춰봐. 얼룩마다 알파벳이 하나씩 숨어 있어." } },
                { LanguageType.English,  new[] { "There are stains on the floor in the same colors as the drawing. Take a close look at the letter under each stain, one by one.", "Match the floor stains to the colors in the drawing one by one. Each stain hides a letter." } },
                { LanguageType.Japanese, new[] { "絵の中のシミと同じ色のシミが床にある。シミがある場所のアルファベットを一つずつ近くで見て。", "床のシミを絵の中の色と一つずつ合わせてみて。シミごとにアルファベットが一つ隠れてる。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_NoteStainViewed_T2,
            new()
            {
                { LanguageType.Korean,   new[] { "그림이랑 같은 색의 바닥 얼룩을 찾아서, 얼룩이 있는 자리의 바닥 알파벳을 확인해.", "바닥 얼룩마다 알파벳이 숨어 있어. 그림 속 색깔이랑 같은 얼룩을 하나씩 찾아서 그 자리 알파벳을 확인해봐." } },
                { LanguageType.English,  new[] { "Find the floor stains in the same colors as the drawing, and check the floor letter where each stain is.", "Every floor stain hides a letter. Find each stain matching a color in the drawing and check the letter at that spot." } },
                { LanguageType.Japanese, new[] { "絵と同じ色の床のシミを探して、シミがある場所の床のアルファベットを確認して。", "床のシミごとにアルファベットが隠れてる。絵の中の色と同じシミを一つずつ探して、その場所のアルファベットを確認してみて。" } }
            }
        },

        /*
         * MemoGuide
         */

        {
            HintLanguageDataKey.HintWine_NoteMemoGuide_T0,
            new()
            {
                { LanguageType.Korean,   new[] { "바닥에서 찾은 알파벳을 어딘가에 적어둘 수 있을 거야.", "알아낸 알파벳을 기록해둘 곳을 찾아봐." } },
                { LanguageType.English,  new[] { "You could write down the letters you found on the floor somewhere.", "Find a place to record the letters you figured out." } },
                { LanguageType.Japanese, new[] { "床で見つけたアルファベットをどこかに書いておけるはず。", "わかったアルファベットを記録できる場所を探してみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_NoteMemoGuide_T1,
            new()
            {
                { LanguageType.Korean,   new[] { "액자 아래 종이에 바닥에서 찾은 알파벳을 적어봐.", "찾은 알파벳은 액자 아래 붙은 종이에 적을 수 있어." } },
                { LanguageType.English,  new[] { "Write the letters you found on the floor on the paper under the frame.", "You can write the letters you found on the paper under the frame." } },
                { LanguageType.Japanese, new[] { "額縁の下の紙に、床で見つけたアルファベットを書いてみて。", "見つけたアルファベットは額縁の下に貼られた紙に書けるよ。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_NoteMemoGuide_T2,
            new()
            {
                { LanguageType.Korean,   new[] { "액자 아래 종이를 가까이서 보고, 칸 색깔이랑 같은 색 얼룩 자리의 바닥 알파벳을 하나씩 적어.", "액자 아래 종이의 색깔 칸마다, 같은 색 바닥 얼룩 자리에서 찾은 알파벳을 적어 넣어." } },
                { LanguageType.English,  new[] { "Look closely at the paper under the frame and write in, one by one, the floor letter under the stain matching each box's color.", "For each colored box on the paper under the frame, write the letter you found at the floor stain of the same color." } },
                { LanguageType.Japanese, new[] { "額縁の下の紙を近くで見て、欄の色と同じ色のシミがある場所の床のアルファベットを一つずつ書いて。", "額縁の下の紙の色の欄ごとに、同じ色の床のシミの場所で見つけたアルファベットを書き込んで。" } }
            }
        },

        /*
         * MemoPartial
         */

        {
            HintLanguageDataKey.HintWine_NoteMemoPartial_T0,
            new()
            {
                { LanguageType.Korean,   new[] { "잘하고 있어. 메모에 적은 알파벳 중에 아직 안 맞는 게 있어.", "거의 다 왔어. 메모에 적은 알파벳을 다시 한번 확인해봐." } },
                { LanguageType.English,  new[] { "You're doing well. Some of the letters on your memo aren't right yet.", "You're almost there. Check the letters on your memo one more time." } },
                { LanguageType.Japanese, new[] { "いい調子。メモに書いたアルファベットの中に、まだ合ってないのがある。", "もう少しだよ。メモに書いたアルファベットをもう一度確認してみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_NoteMemoPartial_T1,
            new()
            {
                { LanguageType.Korean,   new[] { "메모 일부는 맞았어. 나머지 색깔의 얼룩을 다시 찾아가서 그 자리 알파벳을 확인해봐.", "아직 틀린 칸이 남아 있어. 그 칸 색깔이랑 같은 바닥 얼룩 자리의 알파벳을 다시 봐." } },
                { LanguageType.English,  new[] { "Part of your memo is right. Go back to the stains of the other colors and check the letters there.", "There are still wrong boxes. Look again at the letter at the floor stain matching that box's color." } },
                { LanguageType.Japanese, new[] { "メモの一部は合ってる。残りの色のシミをもう一度探しに行って、その場所のアルファベットを確認してみて。", "まだ間違った欄が残ってる。その欄の色と同じ床のシミの場所のアルファベットをもう一度見て。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_NoteMemoPartial_T2,
            new()
            {
                { LanguageType.Korean,   new[] { "메모 칸 색깔이랑 바닥 얼룩 색깔이 정확히 같은지 확인하고, 그 얼룩 자리의 알파벳으로 틀린 칸을 고쳐 적어.", "틀린 칸은 색깔을 헷갈렸을 가능성이 커. 벽 종이 그림이랑 바닥 얼룩 색을 다시 비교해서 그 칸 알파벳을 고쳐." } },
                { LanguageType.English,  new[] { "Make sure the memo box colors exactly match the floor stain colors, then fix the wrong boxes using the letter at that stain.", "The wrong boxes are probably mixed-up colors. Compare the wall paper drawing with the floor stain colors again and fix those letters." } },
                { LanguageType.Japanese, new[] { "メモの欄の色と床のシミの色が本当に同じか確認して、そのシミの場所のアルファベットで間違った欄を書き直して。", "間違った欄はたぶん色を取り違えてる。壁の紙の絵と床のシミの色をもう一度比べて、その欄のアルファベットを直して。" } }
            }
        },

        /*
         * ShelfViewed
         */

        {
            HintLanguageDataKey.HintWine_NoteShelfViewed_T0,
            new()
            {
                { LanguageType.Korean,   new[] { "책장에서 메모 글자랑 관련된 걸 찾아봐.", "책 제목들을 메모랑 비교해봐." } },
                { LanguageType.English,  new[] { "Look for something on the bookshelf related to the letters on your memo.", "Compare the book titles with your memo." } },
                { LanguageType.Japanese, new[] { "本棚でメモの文字に関係あるものを探してみて。", "本のタイトルをメモと比べてみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_NoteShelfViewed_T1,
            new()
            {
                { LanguageType.Korean,   new[] { "메모 글자로 시작하는 책에 숫자가 적혀 있어.", "메모의 각 글자로 시작하는 책을 찾고 그 숫자를 확인해봐." } },
                { LanguageType.English,  new[] { "The books that start with the letters on your memo have numbers written on them.", "Find the book that starts with each letter on your memo and check its number." } },
                { LanguageType.Japanese, new[] { "メモの文字で始まる本に数字が書いてある。", "メモのそれぞれの文字で始まる本を探して、その数字を確認してみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_NoteShelfViewed_T2,
            new()
            {
                { LanguageType.Korean,   new[] { "메모의 색깔별 글자로 시작하는 책을 하나씩 찾아서, 그 책의 숫자를 색깔별로 기억해둬.", "책 네 권의 숫자를 색깔이랑 같이 기억한 다음 와인렉으로 가봐." } },
                { LanguageType.English,  new[] { "Find the books starting with each colored letter on your memo one by one, and remember each book's number by color.", "Remember the four books' numbers along with their colors, then head to the wine rack." } },
                { LanguageType.Japanese, new[] { "メモの色ごとの文字で始まる本を一冊ずつ探して、その本の数字を色ごとに覚えておいて。", "四冊の本の数字を色と一緒に覚えたら、ワインラックへ行ってみて。" } }
            }
        },

        /*
         * WrongColor
         */

        {
            HintLanguageDataKey.HintWine_NoteWrongColor_T0,
            new()
            {
                { LanguageType.Korean,   new[] { "칸은 잘 찾았는데 병이 바뀐 것 같아.", "그 칸에 들어갈 병은 따로 있어." } },
                { LanguageType.English,  new[] { "You found the right slot, but it looks like the bottle got swapped.", "A different bottle belongs in that slot." } },
                { LanguageType.Japanese, new[] { "マスは合ってるけど、ボトルが入れ替わってるみたい。", "そのマスに入るボトルは別にある。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_NoteWrongColor_T1,
            new()
            {
                { LanguageType.Korean,   new[] { "병마다 해당하는 색이 있어. 그 색 글자의 숫자 칸에 꽂아야 해.", "숫자 칸은 맞게 찾았어. 그 칸에 맞는 색의 병으로 바꿔 꽂아봐." } },
                { LanguageType.English,  new[] { "Each bottle has its own color. It goes in the number slot for that color's letter.", "You found the right number slot. Swap in the bottle with the color that fits it." } },
                { LanguageType.Japanese, new[] { "ボトルにはそれぞれ色がある。その色の文字の数字のマスに差さなきゃ。", "数字のマスは合ってる。そのマスに合う色のボトルに差し替えてみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_NoteWrongColor_T2,
            new()
            {
                { LanguageType.Korean,   new[] { "꽂은 병의 색을 메모 칸 색이랑 맞춰봐. 그 색 글자로 찾은 책의 숫자 칸이 그 병 자리야.", "병을 빼서, 병 색이랑 같은 메모 칸의 글자를 확인하고 그 글자 책의 숫자 칸에 다시 꽂아." } },
                { LanguageType.English,  new[] { "Match the color of the bottle you placed with the memo box colors. The number slot of the book you found with that color's letter is where it goes.", "Pull the bottle out, check the letter in the memo box of the same color, and put it in the number slot of that letter's book." } },
                { LanguageType.Japanese, new[] { "差したボトルの色をメモの欄の色と合わせてみて。その色の文字で見つけた本の数字のマスがそのボトルの場所だよ。", "ボトルを抜いて、ボトルと同じ色のメモの欄の文字を確認して、その文字の本の数字のマスに差し直して。" } }
            }
        },

        /*
         * WrongNumber
         */

        {
            HintLanguageDataKey.HintWine_NoteWrongNumber_T0,
            new()
            {
                { LanguageType.Korean,   new[] { "그 칸은 아닌 것 같아. 칸을 고르는 기준이 있어.", "아무 칸에나 꽂으면 안 돼. 어디서 숫자를 봤는지 떠올려봐." } },
                { LanguageType.English,  new[] { "I don't think it's that slot. There's a rule for picking slots.", "You can't just put it in any slot. Think back to where you saw numbers." } },
                { LanguageType.Japanese, new[] { "そのマスじゃないみたい。マスを選ぶ決まりがある。", "どのマスでもいいわけじゃない。どこで数字を見たか思い出してみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_NoteWrongNumber_T1,
            new()
            {
                { LanguageType.Korean,   new[] { "와인렉 칸 번호는 책에서 본 숫자랑 맞춰야 해.", "책장에서 메모 글자로 시작하는 책을 다시 보고 그 숫자를 확인해." } },
                { LanguageType.English,  new[] { "The wine rack slot numbers have to match the numbers you saw in the books.", "Look at the books starting with your memo letters on the bookshelf again and check their numbers." } },
                { LanguageType.Japanese, new[] { "ワインラックのマスの番号は、本で見た数字と合わせなきゃ。", "本棚でメモの文字で始まる本をもう一度見て、その数字を確認して。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_NoteWrongNumber_T2,
            new()
            {
                { LanguageType.Korean,   new[] { "병 색에 맞는 메모 글자로 시작하는 책을 찾고, 그 책 숫자랑 같은 번호 칸에 꽂아.", "꽂은 병을 빼고, 책장에서 그 병 색 글자의 책 숫자를 확인한 다음 같은 번호 칸에 꽂아." } },
                { LanguageType.English,  new[] { "Find the book starting with the memo letter for the bottle's color, and put the bottle in the slot with that book's number.", "Pull the bottle out, check the number of the book for that bottle's color letter on the shelf, then put it in the slot with the same number." } },
                { LanguageType.Japanese, new[] { "ボトルの色に合うメモの文字で始まる本を探して、その本の数字と同じ番号のマスに差して。", "差したボトルを抜いて、本棚でそのボトルの色の文字の本の数字を確認してから、同じ番号のマスに差して。" } }
            }
        },

        /*
         * RackPartial
         */

        {
            HintLanguageDataKey.HintWine_NoteRackPartial_T0,
            new()
            {
                { LanguageType.Korean,   new[] { "좋아, 제대로 꽂고 있어. 남은 병도 같은 방법으로 해봐.", "잘 가고 있어. 나머지 병 자리도 찾아봐." } },
                { LanguageType.English,  new[] { "Nice, you're placing them right. Do the same for the remaining bottles.", "You're on the right track. Find the spots for the other bottles too." } },
                { LanguageType.Japanese, new[] { "いいね、ちゃんと差せてる。残りのボトルも同じやり方でやってみて。", "順調だよ。残りのボトルの場所も探してみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_NoteRackPartial_T1,
            new()
            {
                { LanguageType.Korean,   new[] { "맞게 꽂은 병이 있어. 남은 병도 색깔 → 글자 → 책 숫자 순서로 자리를 찾아봐.", "같은 방법으로 남은 병의 칸을 찾으면 돼." } },
                { LanguageType.English,  new[] { "Some bottles are in the right place. Find spots for the rest in the order color → letter → book number.", "Use the same method to find the slots for the remaining bottles." } },
                { LanguageType.Japanese, new[] { "正しく差せたボトルがある。残りのボトルも色 → 文字 → 本の数字の順で場所を探してみて。", "同じやり方で残りのボトルのマスを探せばいい。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_NoteRackPartial_T2,
            new()
            {
                { LanguageType.Korean,   new[] { "남은 병마다 색에 맞는 메모 글자를 확인하고, 그 글자 책의 숫자 칸에 꽂아.", "아직 자리를 못 찾은 병을 들고, 그 색 글자의 책 숫자랑 같은 번호 칸에 꽂아봐." } },
                { LanguageType.English,  new[] { "For each remaining bottle, check the memo letter for its color and put it in the number slot of that letter's book.", "Pick up a bottle that hasn't found its spot yet and put it in the slot with the same number as that color letter's book." } },
                { LanguageType.Japanese, new[] { "残りのボトルごとに色に合うメモの文字を確認して、その文字の本の数字のマスに差して。", "まだ場所が決まってないボトルを持って、その色の文字の本の数字と同じ番号のマスに差してみて。" } }
            }
        },

        /*
         * HasKey
         */

        {
            HintLanguageDataKey.HintWine_NoteHasKey_T0,
            new()
            {
                { LanguageType.Korean,   new[] { "손에 넣은 걸 쓸 곳이 있을 거야.", "그 열쇠가 맞는 곳을 찾아봐." } },
                { LanguageType.English,  new[] { "There must be somewhere to use what you got.", "Find where that key fits." } },
                { LanguageType.Japanese, new[] { "手に入れたものを使う場所があるはず。", "その鍵が合う場所を探してみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_NoteHasKey_T1,
            new()
            {
                { LanguageType.Korean,   new[] { "열쇠 구멍이 있는 걸 찾아봐.", "방 안의 상자를 살펴봐." } },
                { LanguageType.English,  new[] { "Look for something with a keyhole.", "Check the box in the room." } },
                { LanguageType.Japanese, new[] { "鍵穴があるものを探してみて。", "部屋の中の箱を調べてみて。" } }
            }
        },

        {
            HintLanguageDataKey.HintWine_NoteHasKey_T2,
            new()
            {
                { LanguageType.Korean,   new[] { "열쇠를 들고 상자로 가서 넣어봐.", "상자에 열쇠를 넣으면 나갈 수 있을 거야." } },
                { LanguageType.English,  new[] { "Take the key to the box and put it in.", "Put the key in the box and you should be able to get out." } },
                { LanguageType.Japanese, new[] { "鍵を持って箱のところへ行って入れてみて。", "箱に鍵を入れれば外に出られるはず。" } }
            }
        }
    };
}