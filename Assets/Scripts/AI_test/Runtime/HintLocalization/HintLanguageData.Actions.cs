using System.Collections.Generic;

/// <summary>
/// 최근 행동 라벨 (DefaultActionLabelProvider가 action id 접두사 → 키로 연결).
/// 관찰 문장 틀 안에 들어가므로 라벨 안에는 쉼표(, 、)를 넣지 않음. 영어는 소문자로 시작.
/// 모든 레벨에 붙는 문장이라 풀이 방법을 알려주는 단어(알파벳·숫자 등)는 넣지 않음.
/// </summary>
public static partial class HintLanguageData
{
    private static readonly Dictionary<string, Dictionary<LanguageType, string[]>> ActionData = new()
    {
        /*
         * Wine Scene
         */

        {
            HintLanguageDataKey.HintAction_WineViewHintStain,
            new()
            {
                { LanguageType.Korean,   new[] { "통로 벽의 종이를 자세히 봤지", "벽에 붙은 종이를 한참 들여다보고 있었네" } },
                { LanguageType.English,  new[] { "you took a close look at the paper on the corridor wall", "you were staring at the paper on the wall for a while" } },
                { LanguageType.Japanese, new[] { "通路の壁の紙をじっくり見てたね", "壁に貼られた紙をしばらく見つめてたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_WineViewFloorStain,
            new()
            {
                { LanguageType.Korean,   new[] { "바닥 얼룩을 들여다보고 있었지", "바닥에 쭈그려 앉아 얼룩을 보고 있었네" } },
                { LanguageType.English,  new[] { "you were looking at the stains on the floor", "you were crouched down looking at the floor stains" } },
                { LanguageType.Japanese, new[] { "床のシミを覗き込んでたね", "床にしゃがんでシミを見てたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_WineMemoSolved,
            new()
            {
                { LanguageType.Korean,   new[] { "종이에 글자를 전부 맞게 적었네", "메모를 제대로 다 채웠구나" } },
                { LanguageType.English,  new[] { "you wrote everything on the paper correctly", "you filled in the memo just right" } },
                { LanguageType.Japanese, new[] { "紙に全部正しく書き込んだね", "メモをちゃんと全部埋めたんだね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_WineMemoPartial,
            new()
            {
                { LanguageType.Korean,   new[] { "종이에 몇 칸은 맞게 적었네", "메모를 조금씩 맞춰 가고 있었지" } },
                { LanguageType.English,  new[] { "you got some of the boxes on the paper right", "you were slowly getting the memo right" } },
                { LanguageType.Japanese, new[] { "紙のいくつかの欄は正しく書けたね", "メモを少しずつ合わせていってたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_WineMemoWritten,
            new()
            {
                { LanguageType.Korean,   new[] { "종이에 뭔가 적고 있었지", "메모 종이를 고쳐 쓰고 있었네" } },
                { LanguageType.English,  new[] { "you were writing something on the paper", "you were rewriting the memo" } },
                { LanguageType.Japanese, new[] { "紙に何か書いてたね", "メモを書き直してたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_WineMemoOpened,
            new()
            {
                { LanguageType.Korean,   new[] { "메모 종이를 들여다보기만 했지", "종이를 펼쳐보고 그냥 내려놨네" } },
                { LanguageType.English,  new[] { "you just looked at the memo", "you opened the paper and put it right back" } },
                { LanguageType.Japanese, new[] { "メモを見ただけだったね", "紙を広げてそのまま置いたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_WineViewBookshelf,
            new()
            {
                { LanguageType.Korean,   new[] { "책장을 한참 들여다보고 있었네", "책등을 쭉 훑어보고 있었지" } },
                { LanguageType.English,  new[] { "you were studying the bookshelf for a while", "you were scanning the book spines" } },
                { LanguageType.Japanese, new[] { "本棚をしばらく眺めてたね", "本の背表紙をざっと見てたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_WineBottleGrabbed,
            new()
            {
                { LanguageType.Korean,   new[] { "병을 집어 들었네", "와인병 하나를 손에 쥐고 있었지" } },
                { LanguageType.English,  new[] { "you picked up a bottle", "you were holding a wine bottle" } },
                { LanguageType.Japanese, new[] { "ボトルを手に取ったね", "ワインボトルを一本握ってたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_WineBottlePlaced,
            new()
            {
                { LanguageType.Korean,   new[] { "병을 렉에 꽂아보고 있었지", "와인렉 칸에 병을 넣어봤네" } },
                { LanguageType.English,  new[] { "you were trying bottles in the rack", "you put a bottle into a slot on the wine rack" } },
                { LanguageType.Japanese, new[] { "ボトルをラックに差してみてたね", "ワインラックの棚にボトルを入れてみたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_WineBottleRemoved,
            new()
            {
                { LanguageType.Korean,   new[] { "꽂았던 병을 다시 빼고 있었네", "렉에서 병을 도로 꺼냈지" } },
                { LanguageType.English,  new[] { "you were pulling a bottle back out", "you took a bottle back out of the rack" } },
                { LanguageType.Japanese, new[] { "差したボトルをまた抜いてたね", "ラックからボトルを取り出したね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_WineCabinetLocked,
            new()
            {
                { LanguageType.Korean,   new[] { "장식장 문을 열어보려고 했지", "잠긴 장식장 문을 덜컹거려 봤네" } },
                { LanguageType.English,  new[] { "you tried to open the cabinet door", "you rattled the locked cabinet door" } },
                { LanguageType.Japanese, new[] { "飾り棚の扉を開けようとしてたね", "鍵のかかった飾り棚の扉をガタガタさせたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_WineRackSolved,
            new()
            {
                { LanguageType.Korean,   new[] { "와인렉을 다 채웠지", "장식장 문이 열리는 소리가 났네" } },
                { LanguageType.English,  new[] { "you filled up the wine rack", "I heard the cabinet door open" } },
                { LanguageType.Japanese, new[] { "ワインラックを全部埋めたね", "飾り棚の扉が開く音がしたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_WineKeyGrabbed,
            new()
            {
                { LanguageType.Korean,   new[] { "장식장에서 뭔가 꺼냈지", "작은 열쇠를 손에 넣었네" } },
                { LanguageType.English,  new[] { "you took something out of the cabinet", "you got your hands on a small key" } },
                { LanguageType.Japanese, new[] { "飾り棚から何か取り出したね", "小さな鍵を手に入れたね" } }
            }
        },

        /*
         * Organ Scene
         */

        {
            HintLanguageDataKey.HintAction_OrganStep02,
            new()
            {
                { LanguageType.Korean,   new[] { "거실에서 파이프를 하나 찾아냈지", "파이프 하나를 손에 넣었네" } },
                { LanguageType.English,  new[] { "you found a pipe in the living room", "you got hold of a pipe" } },
                { LanguageType.Japanese, new[] { "リビングでパイプを一本見つけたね", "パイプを一本手に入れたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganStep03,
            new()
            {
                { LanguageType.Korean,   new[] { "옆방에서 파이프를 또 찾아냈지", "파이프를 하나 더 챙겼네" } },
                { LanguageType.English,  new[] { "you found another pipe in the next room", "you grabbed one more pipe" } },
                { LanguageType.Japanese, new[] { "隣の部屋でまたパイプを見つけたね", "パイプをもう一本手に入れたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganStep04,
            new()
            {
                { LanguageType.Korean,   new[] { "오르골 소리를 듣고 있었지", "오르골 멜로디에 귀 기울이고 있었네" } },
                { LanguageType.English,  new[] { "you were listening to the music box", "you were tuned in to the music box melody" } },
                { LanguageType.Japanese, new[] { "オルゴールの音を聴いてたね", "オルゴールのメロディーに耳を傾けてたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganStep05,
            new()
            {
                { LanguageType.Korean,   new[] { "잠긴 방에서 파이프를 꺼내왔지", "비밀번호 방을 열어냈네" } },
                { LanguageType.English,  new[] { "you brought a pipe out of the locked room", "you got the code room open" } },
                { LanguageType.Japanese, new[] { "鍵のかかった部屋からパイプを持ってきたね", "暗証番号の部屋を開けたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganStep06,
            new()
            {
                { LanguageType.Korean,   new[] { "LP를 틀어서 파이프를 찾아냈지", "턴테이블을 돌려보고 있었네" } },
                { LanguageType.English,  new[] { "you played a vinyl record and found a pipe", "you were spinning the turntable" } },
                { LanguageType.Japanese, new[] { "レコードをかけてパイプを見つけたね", "ターンテーブルを回してたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganStep08,
            new()
            {
                { LanguageType.Korean,   new[] { "오르간에 파이프를 끼워 넣었지", "오르간을 고치고 있었네" } },
                { LanguageType.English,  new[] { "you fitted the pipes into the organ", "you were fixing the organ" } },
                { LanguageType.Japanese, new[] { "オルガンにパイプをはめ込んだね", "オルガンを直してたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganStep09,
            new()
            {
                { LanguageType.Korean,   new[] { "거실 바닥에서 종이 조각을 주웠지", "바닥에 떨어진 악보 조각을 챙겼네" } },
                { LanguageType.English,  new[] { "you picked up a scrap of paper from the living room floor", "you grabbed the sheet music scrap off the floor" } },
                { LanguageType.Japanese, new[] { "リビングの床で紙切れを拾ったね", "床に落ちてた楽譜の切れ端を拾ったね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganStep10,
            new()
            {
                { LanguageType.Korean,   new[] { "좁은 통로로 기어 들어갔지", "숨겨진 길을 찾아냈네" } },
                { LanguageType.English,  new[] { "you crawled into the narrow passage", "you found the hidden way" } },
                { LanguageType.Japanese, new[] { "狭い通路に這って入ったね", "隠された道を見つけたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganStep11,
            new()
            {
                { LanguageType.Korean,   new[] { "메트로놈 박자를 맞추고 있었지", "캐비닛 안의 악보 조각을 꺼냈네" } },
                { LanguageType.English,  new[] { "you were setting the metronome's beat", "you took the sheet music scrap out of the cabinet" } },
                { LanguageType.Japanese, new[] { "メトロノームのテンポを合わせてたね", "キャビネットの中の楽譜の切れ端を取り出したね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganStep12,
            new()
            {
                { LanguageType.Korean,   new[] { "책장을 만지작거리고 있었지", "책장 속 책들을 움직이고 있었네" } },
                { LanguageType.English,  new[] { "you were fiddling with the bookshelf", "you were moving the books around on the shelf" } },
                { LanguageType.Japanese, new[] { "本棚をいじってたね", "本棚の本を動かしてたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganStep13,
            new()
            {
                { LanguageType.Korean,   new[] { "액자 뒤를 뒤져보고 있었지", "가족사진 액자를 살펴보고 있었네" } },
                { LanguageType.English,  new[] { "you were searching behind the frame", "you were looking over the family photo frame" } },
                { LanguageType.Japanese, new[] { "額縁の裏を探ってたね", "家族写真の額縁を調べてたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganStep14,
            new()
            {
                { LanguageType.Korean,   new[] { "책상 위를 뒤지고 있었지", "촛불 켜진 책상 위를 살펴보고 있었네" } },
                { LanguageType.English,  new[] { "you were searching the desk", "you were looking over the candlelit desk" } },
                { LanguageType.Japanese, new[] { "机の上を探ってたね", "ろうそくの灯った机の上を調べてたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganStep15,
            new()
            {
                { LanguageType.Korean,   new[] { "악보 조각을 맞추고 있었지", "찢어진 악보를 이어 붙이고 있었네" } },
                { LanguageType.English,  new[] { "you were piecing the sheet music together", "you were putting the torn sheet music back together" } },
                { LanguageType.Japanese, new[] { "楽譜の切れ端を合わせてたね", "破れた楽譜をつなぎ合わせてたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganStep16,
            new()
            {
                { LanguageType.Korean,   new[] { "오르간 건반을 누르고 있었지", "오르간을 연주하고 있었네" } },
                { LanguageType.English,  new[] { "you were pressing the organ keys", "you were playing the organ" } },
                { LanguageType.Japanese, new[] { "オルガンの鍵盤を押してたね", "オルガンを弾いてたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganHoldLP,
            new()
            {
                { LanguageType.Korean,   new[] { "LP판을 집어 들었지", "LP 한 장을 손에 들고 있었네" } },
                { LanguageType.English,  new[] { "you picked up a vinyl record", "you were holding a vinyl record" } },
                { LanguageType.Japanese, new[] { "レコードを手に取ったね", "レコードを一枚手に持ってたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganHoldPipe,
            new()
            {
                { LanguageType.Korean,   new[] { "파이프를 손에 들고 있었지", "파이프를 챙겨 들고 있었네" } },
                { LanguageType.English,  new[] { "you were holding a pipe", "you were carrying a pipe" } },
                { LanguageType.Japanese, new[] { "パイプを手に持ってたね", "パイプを抱えてたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganHoldPaper,
            new()
            {
                { LanguageType.Korean,   new[] { "악보 조각을 들여다보고 있었지", "찢어진 악보를 손에 들고 있었네" } },
                { LanguageType.English,  new[] { "you were looking at a sheet music scrap", "you were holding the torn sheet music" } },
                { LanguageType.Japanese, new[] { "楽譜の切れ端を眺めてたね", "破れた楽譜を手に持ってたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganHoldPicture,
            new()
            {
                { LanguageType.Korean,   new[] { "그림을 들고 살펴보고 있었지", "그림을 손에 들고 있었네" } },
                { LanguageType.English,  new[] { "you were holding a painting and studying it", "you had a painting in your hands" } },
                { LanguageType.Japanese, new[] { "絵を持って調べてたね", "絵を手に持ってたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_OrganHoldAny,
            new()
            {
                { LanguageType.Korean,   new[] { "뭔가를 집어 들었지", "손에 뭔가를 들고 있었네" } },
                { LanguageType.English,  new[] { "you picked something up", "you were holding something" } },
                { LanguageType.Japanese, new[] { "何かを手に取ったね", "何かを手に持ってたね" } }
            }
        },

        /*
         * Doll Scene
         */

                {
            HintLanguageDataKey.HintAction_DollBookViewed,
            new()
            {
                { LanguageType.Korean,   new[] { "마트료시카 그림책을 펼쳐봤지", "시소 그림이 있는 책을 넘겨봤네" } },
                { LanguageType.English,  new[] { "you opened the matryoshka picture book", "you flipped through the book with the seesaw pictures" } },
                { LanguageType.Japanese, new[] { "マトリョーシカの絵本を開いてみたね", "シーソーの絵の本をめくってみたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_DollLockOpened,
            new()
            {
                { LanguageType.Korean,   new[] { "자물쇠 번호를 맞춰 열었지", "마트료시카 순서를 알아냈네" } },
                { LanguageType.English,  new[] { "you cracked the lock code", "you figured out the matryoshka order" } },
                { LanguageType.Japanese, new[] { "ダイヤル錠の番号を合わせて開けたね", "マトリョーシカの順番を見抜いたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_DollLegFixed,
            new()
            {
                { LanguageType.Korean,   new[] { "인형에 다리를 붙여줬지", "부러진 다리를 갈아 끼웠네" } },
                { LanguageType.English,  new[] { "you attached a leg to the doll", "you swapped out the broken leg" } },
                { LanguageType.Japanese, new[] { "人形に脚を付けてあげたね", "折れた脚を付け替えたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_DollCoinGot,
            new()
            {
                { LanguageType.Korean,   new[] { "곰 인형한테서 동전을 얻어냈지", "곰 인형이 동전을 떨어뜨렸네" } },
                { LanguageType.English,  new[] { "you got a coin out of the teddy bear", "the teddy bear dropped a coin" } },
                { LanguageType.Japanese, new[] { "クマのぬいぐるみからコインを手に入れたね", "クマのぬいぐるみがコインを落としたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_DollArmGot,
            new()
            {
                { LanguageType.Korean,   new[] { "오락기에 동전을 넣어서 팔을 얻었지", "오락기에서 팔이 나왔네" } },
                { LanguageType.English,  new[] { "you put a coin in the arcade machine and got an arm", "an arm came out of the arcade machine" } },
                { LanguageType.Japanese, new[] { "ゲーム機にコインを入れて腕を手に入れたね", "ゲーム機から腕が出てきたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_DollArmFixed,
            new()
            {
                { LanguageType.Korean,   new[] { "인형에 팔을 붙여줬지", "인형 팔을 갈아 끼웠네" } },
                { LanguageType.English,  new[] { "you attached an arm to the doll", "you swapped in the doll's arm" } },
                { LanguageType.Japanese, new[] { "人形に腕を付けてあげたね", "人形の腕を付け替えたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_DollSpringGot,
            new()
            {
                { LanguageType.Korean,   new[] { "기차 선로를 맞춰서 태엽을 얻었지", "태엽을 손에 넣었네" } },
                { LanguageType.English,  new[] { "you fixed the train tracks and got the spring", "you got hold of the spring" } },
                { LanguageType.Japanese, new[] { "線路をつないでぜんまいを手に入れたね", "ぜんまいを手に入れたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_DollSpringFixed,
            new()
            {
                { LanguageType.Korean,   new[] { "인형에 태엽을 끼워 넣었지", "인형 등에 태엽을 달아줬네" } },
                { LanguageType.English,  new[] { "you fitted the spring into the doll", "you put the spring on the doll's back" } },
                { LanguageType.Japanese, new[] { "人形にぜんまいを差し込んだね", "人形の背中にぜんまいを付けてあげたね" } }
            }
        },


        {
            HintLanguageDataKey.HintAction_DollBearThrowOnce,
            new()
            {
                { LanguageType.Korean,   new[] { "곰 인형을 한 번 던져봤지", "곰 인형을 한 번 집어던졌네" } },
                { LanguageType.English,  new[] { "you threw the teddy bear once", "you tossed the teddy bear once" } },
                { LanguageType.Japanese, new[] { "クマのぬいぐるみを一回投げてみたね", "クマのぬいぐるみを一回放り投げたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_DollBearThrowFew,
            new()
            {
                { LanguageType.Korean,   new[] { "곰 인형을 벌써 {0}번 던졌지", "곰 인형을 {0}번째 던지고 있었네" } },
                { LanguageType.English,  new[] { "you've already thrown the teddy bear {0} times", "you've tossed the teddy bear {0} times now" } },
                { LanguageType.Japanese, new[] { "クマのぬいぐるみをもう{0}回投げたね", "クマのぬいぐるみ、これで{0}回投げたね" } }
            }
        },

        {
            HintLanguageDataKey.HintAction_DollBearThrowMany,
            new()
            {
                { LanguageType.Korean,   new[] { "곰 인형을 벌써 {0}번이나 던졌지", "곰 인형을 {0}번이나 집어던졌네" } },
                { LanguageType.English,  new[] { "you've thrown the teddy bear {0} times already", "you've hurled the teddy bear {0} times" } },
                { LanguageType.Japanese, new[] { "クマのぬいぐるみをもう{0}回も投げたね", "クマのぬいぐるみを{0}回も放り投げたね" } }
            }
        }
    };
}