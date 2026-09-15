using System.Collections.Generic;

public static class LanguageData
{
    private static readonly Dictionary<
        string,
        Dictionary<LanguageType, string>
    > Data = new()
    {
        /*
         * Doll Scene
         */

        {
            LanguageDataKey.DollScene_Item_DollLeg,
            new()
            {
                {
                    LanguageType.Korean,
                    "인형다리"
                },
                {
                    LanguageType.English,
                    "Doll's Leg"
                }
            }
        },

        {
            LanguageDataKey.DollScene_Item_TeddyBear,
            new()
            {
                {
                    LanguageType.Korean,
                    "멀쩡한 곰인형"
                },
                {
                    LanguageType.English,
                    "Teddy Bear"
                }
            }
        },

        {
            LanguageDataKey.DollScene_Item_TeddyBearTornHead,
            new()
            {
                {
                    LanguageType.Korean,
                    "곰인형 머리"
                },
                {
                    LanguageType.English,
                    "Teddy Bear's Torn Head"
                }
            }
        },

        {
            LanguageDataKey.DollScene_Item_BeheadedTeddyBear,
            new()
            {
                {
                    LanguageType.Korean,
                    "곰인형 몸통"
                },
                {
                    LanguageType.English,
                    "Beheaded Teddy Bear"
                }
            }
        },

        {
            LanguageDataKey.DollScene_Item_Coin,
            new()
            {
                {
                    LanguageType.Korean,
                    "동전"
                },
                {
                    LanguageType.English,
                    "Mysterious Coin"
                }
            }
        },

        {
            LanguageDataKey.DollScene_Item_DollArm,
            new()
            {
                {
                    LanguageType.Korean,
                    "인형 팔"
                },
                {
                    LanguageType.English,
                    "Doll's Arm"
                }
            }
        },

        {
            LanguageDataKey.DollScene_Item_EscapeKey,
            new()
            {
                {
                    LanguageType.Korean,
                    "탈출 열쇠"
                },
                {
                    LanguageType.English,
                    "Escape Key"
                }
            }
        },

        {
            LanguageDataKey.DollScene_Item_DollSpring,
            new()
            {
                {
                    LanguageType.Korean,
                    "태엽"
                },
                {
                    LanguageType.English,
                    "Doll's Spring"
                }
            }
        },


        /*
         * Wine Scene
         */

        {
            LanguageDataKey.WineScene_Item_EscapeKey,
            new()
            {
                {
                    LanguageType.Korean,
                    "탈출 열쇠"
                },
                {
                    LanguageType.English,
                    "Escape Key"
                }
            }
        },

        {
            LanguageDataKey.WineScene_Item_Book,
            new()
            {
                {
                    LanguageType.Korean,
                    "책"
                },
                {
                    LanguageType.English,
                    "Book"
                }
            }
        },


        /*
         * Organ Scene
         */

        {
            LanguageDataKey.OrganScene_Item_Pipe,
            new()
            {
                {
                    LanguageType.Korean,
                    "파이프"
                },
                {
                    LanguageType.English,
                    "Pipe"
                }
            }
        },

        {
            LanguageDataKey.OrganScene_Item_VinylRecord,
            new()
            {
                {
                    LanguageType.Korean,
                    "LP판"
                },
                {
                    LanguageType.English,
                    "Vinyl Record"
                }
            }
        },

        {
            LanguageDataKey.OrganScene_Item_SheetMusic,
            new()
            {
                {
                    LanguageType.Korean,
                    "악보 조각"
                },
                {
                    LanguageType.English,
                    "Sheet Music"
                }
            }
        },

        {
            LanguageDataKey.OrganScene_Item_Book,
            new()
            {
                {
                    LanguageType.Korean,
                    "책"
                },
                {
                    LanguageType.English,
                    "Book"
                }
            }
        }
    };


    /*
     * ==============================
     * Localization Get
     * ==============================
     */

    public static string Get(
        string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return key;
        }

        // 등록된 Localization Key인지 확인
        if (!Data.TryGetValue(
                key,
                out Dictionary<
                    LanguageType,
                    string
                > languageData))
        {
            // 등록되지 않은 값이면
            // 기존 문자열을 그대로 반환
            return key;
        }

        // 현재 선택된 언어
        LanguageType currentLanguage =
            LanguageCurrentStatus.CurrentLanguage;

        // 현재 언어 데이터가 있다면 반환
        if (languageData.TryGetValue(
                currentLanguage,
                out string localizedText))
        {
            return localizedText;
        }

        // 현재 언어 데이터가 없으면
        // Korean을 기본값으로 사용
        if (languageData.TryGetValue(
                LanguageType.Korean,
                out string koreanText))
        {
            return koreanText;
        }

        // 모든 데이터가 없다면 Key 그대로 반환
        return key;
    }
}