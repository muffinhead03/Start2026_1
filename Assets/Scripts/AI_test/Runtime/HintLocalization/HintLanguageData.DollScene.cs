using System.Collections.Generic;

/// <summary>
/// 인형씬 번역 데이터 — 단계별 힌트(DollRoomData.hintByLevel) / 진행 문장(DollHintBridge).
/// LLM 힌트의 [Hint direction]과 LLM이 없을 때의 고정 문구(폴백) 둘 다 이 문장을 씀.
/// 모든 문장은 관찰 문장과 같은 반말. 자물쇠 정답 숫자·곰 인형 정답 횟수는 어떤 문장에도 넣지 않음.
///
/// 2026-11 인형씬 기획 변경: 기존 1단계(마트료시카 → 자물쇠)를 둘로 나눔
///  1 그림책(시소 그림)으로 마트료시카 크기별 무게 알아내기 / 2 서 있는 순서대로 무게를 자물쇠에 입력
///  → 나머지는 한 칸씩 밀려서 9단계. 5단계(오락기)는 게임 없이 동전 넣고 화면 연출 후 팔이 나오는 흐름으로 수정.
/// </summary>
public static partial class HintLanguageData
{
    private static readonly Dictionary<string, Dictionary<LanguageType, string[]>> DollSceneData = new()
    {
        /*
         * Steps — DollRoomData.hintByLevel (레벨 1~5)
         */

        // ── Step 1 : 그림책 보기 ──
        {
            HintLanguageDataKey.HintDoll_Step1_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "바닥에 떨어진 책을 잘 살펴보는 게 좋겠어." } },
                { LanguageType.English,  new[] { "You should take a good look at the book lying on the floor." } },
                { LanguageType.Japanese, new[] { "床に落ちてる本をよく見てみたほうがいいよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step1_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "책에 나온 그림이 어떤 숫자를 알려주고 있을지도 몰라." } },
                { LanguageType.English,  new[] { "The pictures in the book might be telling you some numbers." } },
                { LanguageType.Japanese, new[] { "本の絵が何かの数字を教えてくれてるかもしれない。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step1_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "책을 보고 마트료시카의 무게를 알아내봐." } },
                { LanguageType.English,  new[] { "Use the book to work out how much each matryoshka weighs." } },
                { LanguageType.Japanese, new[] { "本を見て、マトリョーシカの重さを突き止めてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step1_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "책에 나온 그림을 보면 자물쇠 번호를 알 수 있어." } },
                { LanguageType.English,  new[] { "The pictures in the book will give you the lock code." } },
                { LanguageType.Japanese, new[] { "本の絵を見れば、ダイヤル錠の番号がわかるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step1_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "책에 나온 마트료시카 인형 각각의 무게로 자물쇠를 풀어." } },
                { LanguageType.English,  new[] { "Open the lock using the weight of each matryoshka doll shown in the book." } },
                { LanguageType.Japanese, new[] { "本に出てくるマトリョーシカ人形それぞれの重さで、ダイヤル錠を開けて。" } }
            }
        },

        // ── Step 2 : 자물쇠 ──
        {
            HintLanguageDataKey.HintDoll_Step2_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "책에 나온 인형이 방에도 있어. 그 인형들을 잘 살펴보면 자물쇠를 풀 수 있어." } },
                { LanguageType.English,  new[] { "The dolls from the book are in this room too. Look at them closely and you can open the lock." } },
                { LanguageType.Japanese, new[] { "本に出てきた人形はこの部屋にもいるよ。その人形たちをよく見れば、ダイヤル錠を開けられる。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step2_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "마트료시카들이 서 있는 순서대로 자물쇠 번호를 입력해봐." } },
                { LanguageType.English,  new[] { "Enter the lock code in the order the matryoshkas are standing." } },
                { LanguageType.Japanese, new[] { "マトリョーシカが並んでる順番どおりに、ダイヤル錠の番号を入れてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step2_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "마트료시카들이 서 있는 순서대로 책에 나온 숫자들을 자물쇠에 입력해봐." } },
                { LanguageType.English,  new[] { "In the order the matryoshkas are standing, enter the numbers from the book into the lock." } },
                { LanguageType.Japanese, new[] { "マトリョーシカが並んでる順番どおりに、本に出てきた数字をダイヤル錠に入れてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step2_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "마트료시카들이 서 있는 순서대로 책에 나온 인형의 무게를 자물쇠에 입력해." } },
                { LanguageType.English,  new[] { "In the order the matryoshkas are standing, enter each doll's weight from the book into the lock." } },
                { LanguageType.Japanese, new[] { "マトリョーシカが並んでる順番どおりに、本に出てきた人形の重さをダイヤル錠に入れて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step2_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "마트료시카들이 서 있는 순서대로 각 인형에 필요한 무게 추의 개수를 자물쇠에 입력해." } },
                { LanguageType.English,  new[] { "In the order the matryoshkas are standing, enter how many weight blocks each doll needs into the lock." } },
                { LanguageType.Japanese, new[] { "マトリョーシカが並んでる順番どおりに、それぞれの人形に必要な重りの数をダイヤル錠に入れて。" } }
            }
        },

        // ── Step 3 : 다리 ──
        {
            HintLanguageDataKey.HintDoll_Step3_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "목마 구역을 다시 살펴보는 게 좋겠어." } },
                { LanguageType.English,  new[] { "Take another look around the rocking horse area." } },
                { LanguageType.Japanese, new[] { "木馬のエリアをもう一度見てみたほうがいいよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step3_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "목마들 사이에 뭔가 떨어져 있을지도 몰라." } },
                { LanguageType.English,  new[] { "Something might have fallen between the rocking horses." } },
                { LanguageType.Japanese, new[] { "木馬の間に何か落ちてるかもしれない。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step3_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "목마 구역 바닥을 확인해봐." } },
                { LanguageType.English,  new[] { "Check the floor in the rocking horse area." } },
                { LanguageType.Japanese, new[] { "木馬エリアの床を確認してみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step3_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "두 번째와 세 번째 목마 사이를 살펴보면 다리를 찾을 수 있어." } },
                { LanguageType.English,  new[] { "Look between the second and third rocking horses and you'll find the leg." } },
                { LanguageType.Japanese, new[] { "二つ目と三つ目の木馬の間を見れば、脚が見つかるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step3_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "자물쇠를 연 목마 구역으로 가서 두 번째와 세 번째 목마 사이 바닥을 확인해. 다리를 찾으면 인형에게 가져가 교체해." } },
                { LanguageType.English,  new[] { "Go to the rocking horse area you unlocked and check the floor between the second and third horses. Once you find the leg, bring it to the doll and swap it in." } },
                { LanguageType.Japanese, new[] { "鍵を開けた木馬エリアに行って、二つ目と三つ目の木馬の間の床を確認して。脚を見つけたら人形に持っていって付け替えて。" } }
            }
        },

        // ── Step 4 : 곰 인형 ──
        {
            HintLanguageDataKey.HintDoll_Step4_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "그 곰 인형, 뭔가 반응이 있을 것 같아." } },
                { LanguageType.English,  new[] { "That teddy bear seems like it might react to something." } },
                { LanguageType.Japanese, new[] { "あのクマのぬいぐるみ、何か反応がありそう。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step4_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "곰 인형이 뭔가 말을 하는 것 같아." } },
                { LanguageType.English,  new[] { "The teddy bear seems to be saying something." } },
                { LanguageType.Japanese, new[] { "クマのぬいぐるみが何かしゃべってるみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step4_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "곰 인형을 집어서 던져봐." } },
                { LanguageType.English,  new[] { "Pick up the teddy bear and throw it." } },
                { LanguageType.Japanese, new[] { "クマのぬいぐるみを持ち上げて投げてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step4_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "곰 인형을 여러 번 던지면 동전이 나올 거야." } },
                { LanguageType.English,  new[] { "Throw the teddy bear several times and a coin will come out." } },
                { LanguageType.Japanese, new[] { "クマのぬいぐるみを何回も投げれば、コインが出てくるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step4_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "말하는 곰 인형을 집어서 벽이나 바닥에 반복해서 던져봐. 몇 번 던지면 동전이 떨어져." } },
                { LanguageType.English,  new[] { "Pick up the talking teddy bear and keep throwing it at the wall or floor. After a few throws, a coin drops out." } },
                { LanguageType.Japanese, new[] { "しゃべるクマのぬいぐるみを持って、壁か床に何度も投げてみて。何回か投げるとコインが落ちるよ。" } }
            }
        },

        // ── Step 5 : 오락기 ──
        {
            HintLanguageDataKey.HintDoll_Step5_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "방금 얻은 걸 쓸 곳이 있을 것 같아." } },
                { LanguageType.English,  new[] { "There's probably somewhere to use what you just got." } },
                { LanguageType.Japanese, new[] { "さっき手に入れたもの、使える場所がありそう。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step5_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "오락기가 동전을 필요로 하는 것 같아." } },
                { LanguageType.English,  new[] { "The arcade machine seems to need a coin." } },
                { LanguageType.Japanese, new[] { "ゲーム機がコインを必要としてるみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step5_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "오락기에 동전을 넣어봐." } },
                { LanguageType.English,  new[] { "Put the coin into the arcade machine." } },
                { LanguageType.Japanese, new[] { "ゲーム機にコインを入れてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step5_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "동전을 넣고 화면이 꺼질 때까지 기다리면 팔을 얻을 수 있어." } },
                { LanguageType.English,  new[] { "Insert the coin and wait until the screen goes off. You'll get an arm." } },
                { LanguageType.Japanese, new[] { "コインを入れて画面が消えるまで待てば、腕が手に入るよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step5_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "동전을 오락기 투입구에 넣고 화면 연출이 끝날 때까지 기다려. 그러면 인형의 팔이 나와." } },
                { LanguageType.English,  new[] { "Put the coin in the arcade slot and wait for the screen to finish. Then the doll's arm comes out." } },
                { LanguageType.Japanese, new[] { "コインをゲーム機の投入口に入れて、画面の演出が終わるまで待って。そうすると人形の腕が出てくるよ。" } }
            }
        },

        // ── Step 6 : 팔 교체 ──
        {
            HintLanguageDataKey.HintDoll_Step6_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "지금까지 모은 걸 쓸 차례인 것 같아." } },
                { LanguageType.English,  new[] { "Looks like it's time to use what you've collected." } },
                { LanguageType.Japanese, new[] { "今まで集めたものを使う番みたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step6_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "인형에게 다시 가볼 필요가 있어." } },
                { LanguageType.English,  new[] { "You need to go back to the doll." } },
                { LanguageType.Japanese, new[] { "もう一度人形のところに行ってみる必要があるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step6_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "얻은 팔을 인형에게 가져가봐." } },
                { LanguageType.English,  new[] { "Bring the arm you got to the doll." } },
                { LanguageType.Japanese, new[] { "手に入れた腕を人形に持っていってみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step6_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "인형에게 다가가 팔을 장착해봐." } },
                { LanguageType.English,  new[] { "Go up to the doll and attach the arm." } },
                { LanguageType.Japanese, new[] { "人形に近づいて腕を付けてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step6_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "오락기에서 얻은 팔을 인형에게 가져가 상호작용해서 장착해." } },
                { LanguageType.English,  new[] { "Take the arm from the arcade machine to the doll and interact with it to attach the arm." } },
                { LanguageType.Japanese, new[] { "ゲーム機で手に入れた腕を人形に持っていって、調べて付けて。" } }
            }
        },

        // ── Step 7 : 기차 선로 ──
        {
            HintLanguageDataKey.HintDoll_Step7_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "기차 트랙 쪽에 뭔가 있을 것 같아." } },
                { LanguageType.English,  new[] { "There seems to be something over by the train tracks." } },
                { LanguageType.Japanese, new[] { "線路のほうに何かありそう。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step7_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "버튼들이 선로 방향을 바꾸는 것 같아." } },
                { LanguageType.English,  new[] { "The buttons seem to change the direction of the tracks." } },
                { LanguageType.Japanese, new[] { "ボタンで線路の向きが変わるみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step7_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "화남·기쁨·슬픔 버튼으로 선로 방향을 맞춰봐." } },
                { LanguageType.English,  new[] { "Use the angry, happy, and sad buttons to line up the tracks." } },
                { LanguageType.Japanese, new[] { "怒り・喜び・悲しみのボタンで線路の向きを合わせてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step7_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "버튼을 눌러 선로를 맞추면 검은 기차가 밀려나면서 태엽이 나와." } },
                { LanguageType.English,  new[] { "Line up the tracks with the buttons and the black train gets pushed out, releasing the spring." } },
                { LanguageType.Japanese, new[] { "ボタンで線路を合わせると、黒い汽車が押し出されてぜんまいが出てくるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step7_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "세 가지 감정 버튼을 눌러 선로 방향을 바꿔가며 검은 기차를 밀어내. 성공하면 태엽을 얻을 수 있어." } },
                { LanguageType.English,  new[] { "Press the three emotion buttons to switch the tracks and push the black train out. If it works, you'll get the spring." } },
                { LanguageType.Japanese, new[] { "三つの感情ボタンで線路の向きを変えながら、黒い汽車を押し出して。成功すればぜんまいが手に入るよ。" } }
            }
        },

        // ── Step 8 : 태엽 장착 ──
        {
            HintLanguageDataKey.HintDoll_Step8_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "인형에게 다시 가볼 때가 된 것 같아." } },
                { LanguageType.English,  new[] { "It might be time to visit the doll again." } },
                { LanguageType.Japanese, new[] { "そろそろ人形のところにまた行ってみる頃みたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step8_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "방금 얻은 걸 인형에 쓸 수 있을 거야." } },
                { LanguageType.English,  new[] { "You can probably use what you just got on the doll." } },
                { LanguageType.Japanese, new[] { "さっき手に入れたもの、人形に使えるはず。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step8_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "얻은 태엽을 인형에게 가져가봐." } },
                { LanguageType.English,  new[] { "Bring the spring you got to the doll." } },
                { LanguageType.Japanese, new[] { "手に入れたぜんまいを人形に持っていってみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step8_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "인형의 태엽 부분에 상호작용해봐." } },
                { LanguageType.English,  new[] { "Interact with the spot on the doll where the spring goes." } },
                { LanguageType.Japanese, new[] { "人形のぜんまいの部分を調べてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step8_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "기차에서 얻은 태엽을 인형에게 가져가 태엽 자리에 장착해." } },
                { LanguageType.English,  new[] { "Take the spring from the train to the doll and fit it into the spring slot." } },
                { LanguageType.Japanese, new[] { "汽車で手に入れたぜんまいを人形に持っていって、ぜんまいの場所に付けて。" } }
            }
        },

        // ── Step 9 : 탈출 ──
        {
            HintLanguageDataKey.HintDoll_Step9_Level1,
            new()
            {
                { LanguageType.Korean,   new[] { "인형이 이제 다 준비된 것 같아." } },
                { LanguageType.English,  new[] { "The doll looks just about ready now." } },
                { LanguageType.Japanese, new[] { "人形、もう準備ができたみたい。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step9_Level2,
            new()
            {
                { LanguageType.Korean,   new[] { "인형과 다시 상호작용해볼 차례야." } },
                { LanguageType.English,  new[] { "Time to interact with the doll again." } },
                { LanguageType.Japanese, new[] { "もう一度人形を調べてみる番だよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step9_Level3,
            new()
            {
                { LanguageType.Korean,   new[] { "인형의 태엽을 돌려봐." } },
                { LanguageType.English,  new[] { "Wind the doll's spring." } },
                { LanguageType.Japanese, new[] { "人形のぜんまいを巻いてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step9_Level4,
            new()
            {
                { LanguageType.Korean,   new[] { "태엽을 돌리면 수리가 끝나고 뭔가 떨어질 거야." } },
                { LanguageType.English,  new[] { "Wind it up and the repair will finish. Something will drop." } },
                { LanguageType.Japanese, new[] { "ぜんまいを巻けば修理が終わって、何かが落ちてくるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_Step9_Level5,
            new()
            {
                { LanguageType.Korean,   new[] { "인형과 다시 상호작용해서 태엽을 돌려. 수리가 끝나면 열쇠가 떨어지는데, 그 열쇠로 탈출해." } },
                { LanguageType.English,  new[] { "Interact with the doll again and wind the spring. When the repair is done a key drops. Use it to escape." } },
                { LanguageType.Japanese, new[] { "もう一度人形を調べてぜんまいを巻いて。修理が終わると鍵が落ちるから、その鍵で脱出して。" } }
            }
        },

        /*
         * Progress notes — DollHintBridge (T0 레벨1~2 / T1 레벨3 / T2 레벨4~5)
         */

        {
            HintLanguageDataKey.HintDoll_NoteBearEarly_T0,
            new()
            {
                { LanguageType.Korean,   new[] { "곰 인형이 던질 때마다 반응하고 있어. 멈추지 말고 계속 던져봐.", "곰 인형 목소리가 조금씩 달라지고 있어. 한 번 더 던져봐.", "곰 인형이 던져질 때마다 뭔가 변하는 것 같아. 계속 던져봐." } },
                { LanguageType.English,  new[] { "The teddy bear reacts every time you throw it. Don't stop, keep throwing.", "The teddy bear's voice is changing little by little. Throw it once more.", "Something seems to change every time the teddy bear gets thrown. Keep throwing." } },
                { LanguageType.Japanese, new[] { "クマのぬいぐるみ、投げるたびに反応してる。止めずに投げ続けてみて。", "クマのぬいぐるみの声が少しずつ変わってる。もう一回投げてみて。", "クマのぬいぐるみ、投げられるたびに何か変わってるみたい。投げ続けてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_NoteBearEarly_T1,
            new()
            {
                { LanguageType.Korean,   new[] { "곰 인형이 충격을 받을수록 약해지는 것 같아. 계속 세게 던져봐.", "곰 인형이 던질 때마다 조금씩 망가지고 있어. 멈추지 말고 계속 던져봐." } },
                { LanguageType.English,  new[] { "The teddy bear seems to get weaker the more it gets hit. Keep throwing it hard.", "The teddy bear is falling apart a little more with every throw. Don't stop, keep throwing." } },
                { LanguageType.Japanese, new[] { "クマのぬいぐるみ、投げるたびに弱ってるみたい。強く投げ続けてみて。", "クマのぬいぐるみ、投げるたびに少しずつ壊れてきてる。止めずに投げ続けてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_NoteBearEarly_T2,
            new()
            {
                { LanguageType.Korean,   new[] { "곰 인형을 계속 던지면 안에서 뭔가 떨어질 거야. 멈추지 마.", "곰 인형 안에 뭔가 들어 있어. 찢어질 때까지 계속 던져봐." } },
                { LanguageType.English,  new[] { "Keep throwing the teddy bear and something will fall out of it. Don't stop.", "There's something inside the teddy bear. Keep throwing it until it tears open." } },
                { LanguageType.Japanese, new[] { "クマのぬいぐるみを投げ続ければ、中から何か落ちてくるよ。止めないで。", "クマのぬいぐるみの中に何か入ってる。破れるまで投げ続けてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_NoteBearAlmost_T0,
            new()
            {
                { LanguageType.Korean,   new[] { "곰 인형이 이제 거의 버티지 못하는 것 같아. 조금만 더 던져봐.", "곰 인형 반응이 점점 약해지고 있어. 조금만 더 던져봐.", "곰 인형이 한계에 다다른 것 같아. 멈추지 말고 조금만 더." } },
                { LanguageType.English,  new[] { "The teddy bear can barely hold together now. Throw it a little more.", "The teddy bear's reactions are getting weaker. Just a few more throws.", "The teddy bear seems to be at its limit. Don't stop, just a bit more." } },
                { LanguageType.Japanese, new[] { "クマのぬいぐるみ、もうほとんど持ちこたえられないみたい。もう少しだけ投げてみて。", "クマのぬいぐるみの反応がだんだん弱くなってる。もう少しだけ投げてみて。", "クマのぬいぐるみ、限界みたい。止めずにもう少しだけ。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_NoteBearAlmost_T1,
            new()
            {
                { LanguageType.Korean,   new[] { "곰 인형이 곧 찢어질 것 같아. 조금만 더 세게 던져봐.", "곰 인형 솔기가 터지기 직전이야. 조금만 더 던져봐." } },
                { LanguageType.English,  new[] { "The teddy bear looks like it's about to tear. Throw it a little harder.", "The teddy bear's seams are about to burst. Throw it a little more." } },
                { LanguageType.Japanese, new[] { "クマのぬいぐるみ、もうすぐ破れそう。もう少し強く投げてみて。", "クマのぬいぐるみの縫い目が破れる寸前だよ。もう少し投げてみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_NoteBearAlmost_T2,
            new()
            {
                { LanguageType.Korean,   new[] { "곰 인형이 찢어지면 안에서 동전이 나올 거야. 조금만 더 던져봐.", "조금만 더 던지면 곰 인형이 찢어지면서 동전이 떨어질 거야." } },
                { LanguageType.English,  new[] { "When the teddy bear tears, a coin will come out. Throw it a little more.", "A few more throws and the teddy bear will tear open and drop a coin." } },
                { LanguageType.Japanese, new[] { "クマのぬいぐるみが破れたら、中からコインが出てくるよ。もう少し投げてみて。", "もう少し投げれば、クマのぬいぐるみが破れてコインが落ちるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_NoteBearBroken_T0,
            new()
            {
                { LanguageType.Korean,   new[] { "찢어진 곰 인형에서 동전이 떨어졌어. 바닥을 확인해봐." } },
                { LanguageType.English,  new[] { "A coin fell out of the torn teddy bear. Check the floor." } },
                { LanguageType.Japanese, new[] { "破れたクマのぬいぐるみからコインが落ちたよ。床を確認してみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_NoteBearBroken_T1,
            new()
            {
                { LanguageType.Korean,   new[] { "찢어진 곰 인형에서 동전이 떨어졌어. 근처 바닥에서 주워봐.", "곰 인형 안에서 동전이 나왔어. 바닥에 떨어진 동전을 주워봐.", "곰 인형이 찢어지면서 동전이 굴러 나왔어. 바닥을 보고 주워봐." } },
                { LanguageType.English,  new[] { "A coin fell out of the torn teddy bear. Pick it up from the floor nearby.", "A coin came out of the teddy bear. Pick up the coin on the floor.", "A coin rolled out when the teddy bear tore. Look at the floor and pick it up." } },
                { LanguageType.Japanese, new[] { "破れたクマのぬいぐるみからコインが落ちたよ。近くの床で拾ってみて。", "クマのぬいぐるみの中からコインが出てきたよ。床に落ちたコインを拾ってみて。", "クマのぬいぐるみが破れて、コインが転がり出たよ。床を見て拾ってみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_NoteBearBroken_T2,
            new()
            {
                { LanguageType.Korean,   new[] { "찢어진 곰 인형 바로 옆 바닥에 떨어진 동전을 주워. 그 동전은 오락기에 쓸 수 있어.", "곰 인형에서 나온 동전을 주워서 챙겨. 오락기에 넣을 수 있어." } },
                { LanguageType.English,  new[] { "Pick up the coin on the floor right next to the torn teddy bear. You can use it in the arcade machine.", "Grab the coin that came out of the teddy bear. It goes into the arcade machine." } },
                { LanguageType.Japanese, new[] { "破れたクマのぬいぐるみのすぐ横の床に落ちたコインを拾って。そのコインはゲーム機に使えるよ。", "クマのぬいぐるみから出てきたコインを拾っておいて。ゲーム機に入れられるよ。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_NoteDollMoving_T0,
            new()
            {
                { LanguageType.Korean,   new[] { "인형이 움직이기 시작했어. 잠시 지켜봐.", "인형이 뭔가 하려는 것 같아. 조금만 기다려봐." } },
                { LanguageType.English,  new[] { "The doll has started moving. Watch it for a moment.", "The doll seems to be about to do something. Wait a little." } },
                { LanguageType.Japanese, new[] { "人形が動き始めたよ。少し見守ってみて。", "人形が何かしようとしてるみたい。少し待ってみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_NoteKeyDropped_T0,
            new()
            {
                { LanguageType.Korean,   new[] { "인형 쪽에서 열쇠가 떨어졌어. 바닥을 확인해봐." } },
                { LanguageType.English,  new[] { "A key dropped from near the doll. Check the floor." } },
                { LanguageType.Japanese, new[] { "人形のほうから鍵が落ちたよ。床を確認してみて。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_NoteKeyDropped_T1,
            new()
            {
                { LanguageType.Korean,   new[] { "인형 근처 바닥에 떨어진 열쇠를 찾아봐.", "인형이 떨어뜨린 열쇠가 바닥 어딘가에 있을 거야." } },
                { LanguageType.English,  new[] { "Look for the key that fell on the floor near the doll.", "The key the doll dropped should be somewhere on the floor." } },
                { LanguageType.Japanese, new[] { "人形の近くの床に落ちた鍵を探してみて。", "人形が落とした鍵が、床のどこかにあるはず。" } }
            }
        },
        {
            HintLanguageDataKey.HintDoll_NoteKeyDropped_T2,
            new()
            {
                { LanguageType.Korean,   new[] { "인형이 떨어뜨린 열쇠를 주워서 탈출해.", "바닥에 떨어진 열쇠를 주워서 문을 열고 나가." } },
                { LanguageType.English,  new[] { "Pick up the key the doll dropped and escape.", "Pick up the key on the floor, open the door, and get out." } },
                { LanguageType.Japanese, new[] { "人形が落とした鍵を拾って脱出して。", "床に落ちた鍵を拾って、扉を開けて外に出て。" } }
            }
        }
    };
}