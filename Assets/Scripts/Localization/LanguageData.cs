using System.Collections.Generic;

public static class LanguageData
{
    private static readonly Dictionary<
        string,
        Dictionary<LanguageType, string>
    > Data = new()
    {
        /*
         * Intro Scene
         */

        {
            LanguageDataKey.IntroScene_Text_Open,
            new()
            {
                {
                    LanguageType.Korean,
                    "연다"
                },
                {
                    LanguageType.English,
                    "Open"
                },
                {
                    LanguageType.Japanese,
                    "開ける"
                }
            }
        },

        {
            LanguageDataKey.IntroScene_Text_DoNotOpen,
            new()
            {
                {
                    LanguageType.Korean,
                    "열지 않는다"
                },
                {
                    LanguageType.English,
                    "Do not open"
                },
                {
                    LanguageType.Japanese,
                    "開けない"
                }
            }
        },

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
                },
                {
                    LanguageType.Japanese,
                    "人形の脚"
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
                },
                {
                    LanguageType.Japanese,
                    "無傷のクマのぬいぐるみ"
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
                },
                {
                    LanguageType.Japanese,
                    "クマのぬいぐるみの頭"
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
                },
                {
                    LanguageType.Japanese,
                    "クマのぬいぐるみの胴体"
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
                },
                {
                    LanguageType.Japanese,
                    "不思議なコイン"
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
                },
                {
                    LanguageType.Japanese,
                    "人形の腕"
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
                },
                {
                    LanguageType.Japanese,
                    "脱出の鍵"
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
                },
                {
                    LanguageType.Japanese,
                    "ぜんまい"
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
                },
                {
                    LanguageType.Japanese,
                    "脱出の鍵"
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
                },
                {
                    LanguageType.Japanese,
                    "本"
                }
            }
        },

        {
            LanguageDataKey.WineScene_Item_WineBottle_RubyRed,
            new()
            {
                {
                    LanguageType.Korean,
                    "루비색 와인병"
                },
                {
                    LanguageType.English,
                    "Ruby Red Wine Bottle"
                },
                {
                    LanguageType.Japanese,
                    "赤ワイン1"
                }
            }
        },

        {
            LanguageDataKey.WineScene_Item_WineBottle_TawnyBrown,
            new()
            {
                {
                    LanguageType.Korean,
                    "황갈색 와인병"
                },
                {
                    LanguageType.English,
                    "Tawny Brown Wine Bottle"
                },
                {
                    LanguageType.Japanese,
                    "赤ワイン2"
                }
            }
        },

        {
            LanguageDataKey.WineScene_Item_WineBottle_Burgundy,
            new()
            {
                {
                    LanguageType.Korean,
                    "버건디색 와인병"
                },
                {
                    LanguageType.English,
                    "Burgundy Wine Bottle"
                },
                {
                    LanguageType.Japanese,
                    "赤ワイン3"
                }
            }
        },

        {
            LanguageDataKey.WineScene_Item_WineBottle_Maroon,
            new()
            {
                {
                    LanguageType.Korean,
                    "적갈색 와인병"
                },
                {
                    LanguageType.English,
                    "Maroon Wine Bottle"
                },
                {
                    LanguageType.Japanese,
                    "赤ワイン4"
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
                },
                {
                    LanguageType.Japanese,
                    "オルガンのパイプ"
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
            LanguageDataKey.OrganScene_Item_VinylRecord_Green,
            new()
            {
                {
                    LanguageType.Korean,
                    "초록색 LP판"
                },
                {
                    LanguageType.English,
                    "Green Vinyl Record"
                },
                {
                    LanguageType.Japanese,
                    "緑色のレコード"
                }
            }
        },

        {
            LanguageDataKey.OrganScene_Item_VinylRecord_Red,
            new()
            {
                {
                    LanguageType.Korean,
                    "빨간색 LP판"
                },
                {
                    LanguageType.English,
                    "Red Vinyl Record"
                },
                {
                    LanguageType.Japanese,
                    "赤色のレコード"
                }
            }
        },

        {
            LanguageDataKey.OrganScene_Item_VinylRecord_Blue,
            new()
            {
                {
                    LanguageType.Korean,
                    "파란색 LP판"
                },
                {
                    LanguageType.English,
                    "Blue Vinyl Record"
                },
                {
                    LanguageType.Japanese,
                    "青色のレコード"
                }
            }
        },

        {
            LanguageDataKey.OrganScene_Item_VinylRecord_Yellow,
            new()
            {
                {
                    LanguageType.Korean,
                    "노란색 LP판"
                },
                {
                    LanguageType.English,
                    "Yellow Vinyl Record"
                },
                {
                    LanguageType.Japanese,
                    "黄色のレコード"
                }
            }
        },

        {
            LanguageDataKey.OrganScene_Item_VinylRecord_White,
            new()
            {
                {
                    LanguageType.Korean,
                    "흰색 LP판"
                },
                {
                    LanguageType.English,
                    "White Vinyl Record"
                },
                {
                    LanguageType.Japanese,
                    "白色のレコード"
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
                },
                {
                    LanguageType.Japanese,
                    "本"
                }
            }
        },

        /*
         * Setting Menu / Title Menu
         */

        {
            LanguageDataKey.Setting_Title,
            new()
            {
                {
                    LanguageType.Korean,
                    "설정"
                },
                {
                    LanguageType.English,
                    "Settings"
                },
                {
                    LanguageType.Japanese,
                    "設定"
                }
            }
        },

        {
            LanguageDataKey.Setting_HintLevel,
            new()
            {
                {
                    LanguageType.Korean,
                    "힌트 난이도"
                },
                {
                    LanguageType.English,
                    "Hint Level"
                },
                {
                    LanguageType.Japanese,
                    "ヒントレベル"
                }
            }
        },

        {
            LanguageDataKey.Setting_MouseSensitivity,
            new()
            {
                {
                    LanguageType.Korean,
                    "마우스 감도"
                },
                {
                    LanguageType.English,
                    "Mouse Sensitivity"
                },
                {
                    LanguageType.Japanese,
                    "マウス感度"
                }
            }
        },

        {
            LanguageDataKey.Setting_SoundEffects,
            new()
            {
                {
                    LanguageType.Korean,
                    "효과음"
                },
                {
                    LanguageType.English,
                    "Sound Effects"
                },
                {
                    LanguageType.Japanese,
                    "効果音"
                }
            }
        },

        {
            LanguageDataKey.Setting_Language,
            new()
            {
                {
                    LanguageType.Korean,
                    "언어"
                },
                {
                    LanguageType.English,
                    "Language"
                },
                {
                    LanguageType.Japanese,
                    "言語"
                }
            }
        },

        {
            LanguageDataKey.Setting_ReturnToTitle,
            new()
            {
                {
                    LanguageType.Korean,
                    "타이틀로 돌아가기"
                },
                {
                    LanguageType.English,
                    "Return to Title"
                },
                {
                    LanguageType.Japanese,
                    "タイトルに戻る"
                }
            }
        },

        {
            LanguageDataKey.Setting_ExitGame,
            new()
            {
                {
                    LanguageType.Korean,
                    "게임 종료"
                },
                {
                    LanguageType.English,
                    "Exit Game"
                },
                {
                    LanguageType.Japanese,
                    "ゲーム終了"
                }
            }
        },

        {
            LanguageDataKey.Setting_Close,
            new()
            {
                {
                    LanguageType.Korean,
                    "닫기(Q)"
                },
                {
                    LanguageType.English,
                    "Close(Q)"
                },
                {
                    LanguageType.Japanese,
                    "閉じる(Q)"
                }
            }
        },

        {
            LanguageDataKey.Setting_Hint_Easy,
            new()
            {
                {
                    LanguageType.Korean,
                    "쉬움"
                },
                {
                    LanguageType.English,
                    "Easy"
                },
                {
                    LanguageType.Japanese,
                    "やさしい"
                }
            }
        },

        {
            LanguageDataKey.Setting_Hint_Medium,
            new()
            {
                {
                    LanguageType.Korean,
                    "보통"
                },
                {
                    LanguageType.English,
                    "Medium"
                },
                {
                    LanguageType.Japanese,
                    "ふつう"
                }
            }
        },

        {
            LanguageDataKey.Setting_Hint_Hard,
            new()
            {
                {
                    LanguageType.Korean,
                    "어려움"
                },
                {
                    LanguageType.English,
                    "Hard"
                },
                {
                    LanguageType.Japanese,
                    "むずかしい"
                }
            }
        },

        {
            LanguageDataKey.Title_Continue,
            new()
            {
                {
                    LanguageType.Korean,
                    "이어하기"
                },
                {
                    LanguageType.English,
                    "Continue"
                },
                {
                    LanguageType.Japanese,
                    "つづきから"
                }
            }
        },

        {
            LanguageDataKey.Title_Start,
            new()
            {
                {
                    LanguageType.Korean,
                    "시작하기"
                },
                {
                    LanguageType.English,
                    "Start"
                },
                {
                    LanguageType.Japanese,
                    "はじめから"
                }
            }
        },

        {
            LanguageDataKey.Title_Setting,
            new()
            {
                {
                    LanguageType.Korean,
                    "설정"
                },
                {
                    LanguageType.English,
                    "Setting"
                },
                {
                    LanguageType.Japanese,
                    "設定"
                }
            }
        },

        {
            LanguageDataKey.Title_Exit,
            new()
            {
                {
                    LanguageType.Korean,
                    "종료"
                },
                {
                    LanguageType.English,
                    "Exit"
                },
                {
                    LanguageType.Japanese,
                    "終了"
                }
            }
        }
    };


    /*
     * Localization Get
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
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(
                $"[Localization] 등록되지 않은 키: {key}"
            );
#endif
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