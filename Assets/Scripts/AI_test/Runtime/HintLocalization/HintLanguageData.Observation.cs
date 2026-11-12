using System.Collections.Generic;

/// <summary>
/// 관찰 문장 틀 (HintEngine이 IHintPhraseTable로 읽음 — 키는 Core의 ObservationPhraseKeys).
/// {0}, {1}은 자리표시자 — 언어마다 어순이 달라서 문장 틀째로 번역함.
/// 행동·위치 문장은 문장 중간에도 들어가므로 영어는 소문자로 시작 (문장 맨 앞이면 HintEngine이 대문자로 바꿈).
/// </summary>
public static partial class HintLanguageData
{
    private static readonly Dictionary<string, Dictionary<LanguageType, string[]>> ObservationData = new()
    {
        {
            HintLanguageDataKey.HintObs_GenericAction,
            new()
            {
                { LanguageType.Korean,   new[] { "방금 뭔가를 조사하고 있었지", "조금 전에 뭔가 살펴보고 있었네" } },
                { LanguageType.English,  new[] { "you were just checking something out", "you were looking at something a moment ago" } },
                { LanguageType.Japanese, new[] { "さっき何か調べてたね", "ちょっと前に何か見てたね" } }
            }
        },

        {
            HintLanguageDataKey.HintObs_ProximityVeryClose,
            new()
            {
                { LanguageType.Korean,   new[] { "바로 근처에 있네", "가까이 와 있구나" } },
                { LanguageType.English,  new[] { "it's right near you", "you're really close" } },
                { LanguageType.Japanese, new[] { "すぐ近くにいるね", "近くまで来てるね" } }
            }
        },

        {
            HintLanguageDataKey.HintObs_ProximityNearby,
            new()
            {
                { LanguageType.Korean,   new[] { "근처에 있는 것 같은데", "가까운 곳에 있나 보네" } },
                { LanguageType.English,  new[] { "you seem to be nearby", "looks like you're not far" } },
                { LanguageType.Japanese, new[] { "近くにいるみたいだけど", "近いところにいるみたいだね" } }
            }
        },

        {
            HintLanguageDataKey.HintObs_ProximityFar,
            new()
            {
                { LanguageType.Korean,   new[] { "좀 멀리 떨어진 곳에 있는 것 같은데", "다른 데 가 있나 보네" } },
                { LanguageType.English,  new[] { "you seem pretty far away", "looks like you're somewhere else" } },
                { LanguageType.Japanese, new[] { "ちょっと離れたところにいるみたいだけど", "別のところにいるみたいだね" } }
            }
        },

        {
            HintLanguageDataKey.HintObs_ZoneHere,
            new()
            {
                { LanguageType.Korean,   new[] { "{0}에 있구나", "{0}에 와 있네" } },
                { LanguageType.English,  new[] { "you're in {0}", "you've made it to {0}" } },
                { LanguageType.Japanese, new[] { "{0}にいるんだね", "{0}に来てるね" } }
            }
        },

        {
            HintLanguageDataKey.HintObs_Landmark,
            new()
            {
                { LanguageType.Korean,   new[] { "{0} 앞에 있네", "{0} 앞에 서 있구나" } },
                { LanguageType.English,  new[] { "you're in front of {0}", "you're standing by {0}" } },
                { LanguageType.Japanese, new[] { "{0}の前にいるね", "{0}の前に立ってるんだね" } }
            }
        },

        {
            HintLanguageDataKey.HintObs_ZoneOther,
            new()
            {
                { LanguageType.Korean,   new[] { "{0} 쪽도 한번 둘러봐.", "{0} 쪽을 살펴보는 게 좋겠어." } },
                { LanguageType.English,  new[] { "take a look around {0} too.", "you should check {0}." } },
                { LanguageType.Japanese, new[] { "{0}の方も見てみて。", "{0}の方を調べたほうがいいよ。" } }
            }
        },

        {
            HintLanguageDataKey.HintObs_JoinActionPlace,
            new()
            {
                { LanguageType.Korean,   new[] { "{0}, 지금은 {1}..." } },
                { LanguageType.English,  new[] { "{0}, and now {1}..." } },
                { LanguageType.Japanese, new[] { "{0}、今は{1}…" } }
            }
        },

        {
            HintLanguageDataKey.HintObs_JoinPlace,
            new()
            {
                { LanguageType.Korean,   new[] { "{0}..." } },
                { LanguageType.English,  new[] { "{0}..." } },
                { LanguageType.Japanese, new[] { "{0}…" } }
            }
        },

        {
            HintLanguageDataKey.HintObs_JoinActionProximity,
            new()
            {
                { LanguageType.Korean,   new[] { "{0}, 지금은 {1}." } },
                { LanguageType.English,  new[] { "{0}, and now {1}." } },
                { LanguageType.Japanese, new[] { "{0}、今は{1}。" } }
            }
        },

        {
            HintLanguageDataKey.HintObs_JoinSingle,
            new()
            {
                { LanguageType.Korean,   new[] { "{0}." } },
                { LanguageType.English,  new[] { "{0}." } },
                { LanguageType.Japanese, new[] { "{0}。" } }
            }
        },

        {
            HintLanguageDataKey.HintObs_SentenceGap,
            new()
            {
                { LanguageType.Korean,   new[] { " " } },
                { LanguageType.English,  new[] { " " } },
                { LanguageType.Japanese, new[] { "" } }
            }
        }
    };
}