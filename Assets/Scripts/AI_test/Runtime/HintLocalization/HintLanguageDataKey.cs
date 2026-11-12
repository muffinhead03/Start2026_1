/// <summary>
/// 힌트 시스템 번역 키 (팀 LanguageDataKey와 같은 방식 — const string + nameof).
/// 팀 LanguageDataKey/LanguageData는 아이템 이름 등 팀 공용이라 건드리지 않고, 힌트 문장은 여기서 따로 관리함
/// (같은 파일 끝에 동시에 추가하면 머지 충돌이 나서 분리).
///
/// 이름 규칙: Hint{영역}_{대상}
///  - HintUI_     : 힌트 패널 고정 문구 (LocalizedHintUIText)
///  - HintObs_    : 관찰 문장 틀 (HintEngine — 키 값은 Core의 ObservationPhraseKeys와 같게 연결)
///  - HintAction_ : 최근 행동 라벨 (DefaultActionLabelProvider)
///  - HintWine_   : 와인씬 구역·랜드마크 이름(씬 Inspector에 키를 적음) / 단계별 힌트 / 진행 문장
/// </summary>
public static class HintLanguageDataKey
{
    /*
     * Hint UI
     */

    public const string HintUI_RadioNoise =
        nameof(HintUI_RadioNoise);

    public const string HintUI_Greeting =
        nameof(HintUI_Greeting);

    public const string HintUI_Connecting =
        nameof(HintUI_Connecting);

    public const string HintUI_NoHintsLeft =
        nameof(HintUI_NoHintsLeft);

    public const string HintUI_AllCluesFound =
        nameof(HintUI_AllCluesFound);

    public const string HintUI_NoResponse =
        nameof(HintUI_NoResponse);

    public const string HintUI_FallbackTag =
        nameof(HintUI_FallbackTag);


    /*
     * Observation Phrases (Core가 정한 키를 그대로 씀)
     */

    public const string HintObs_GenericAction       = ObservationPhraseKeys.GenericAction;
    public const string HintObs_ProximityVeryClose  = ObservationPhraseKeys.ProximityVeryClose;
    public const string HintObs_ProximityNearby     = ObservationPhraseKeys.ProximityNearby;
    public const string HintObs_ProximityFar        = ObservationPhraseKeys.ProximityFar;
    public const string HintObs_ZoneHere            = ObservationPhraseKeys.ZoneHere;
    public const string HintObs_Landmark            = ObservationPhraseKeys.Landmark;
    public const string HintObs_ZoneOther           = ObservationPhraseKeys.ZoneOther;
    public const string HintObs_JoinActionPlace     = ObservationPhraseKeys.JoinActionPlace;
    public const string HintObs_JoinPlace           = ObservationPhraseKeys.JoinPlace;
    public const string HintObs_JoinActionProximity = ObservationPhraseKeys.JoinActionProximity;
    public const string HintObs_JoinSingle          = ObservationPhraseKeys.JoinSingle;
    public const string HintObs_SentenceGap         = ObservationPhraseKeys.SentenceGap;


    /*
     * Action Labels — Wine Scene
     */

    public const string HintAction_WineViewHintStain   = nameof(HintAction_WineViewHintStain);
    public const string HintAction_WineViewFloorStain  = nameof(HintAction_WineViewFloorStain);
    public const string HintAction_WineMemoSolved      = nameof(HintAction_WineMemoSolved);
    public const string HintAction_WineMemoPartial     = nameof(HintAction_WineMemoPartial);
    public const string HintAction_WineMemoWritten     = nameof(HintAction_WineMemoWritten);
    public const string HintAction_WineMemoOpened      = nameof(HintAction_WineMemoOpened);
    public const string HintAction_WineViewBookshelf   = nameof(HintAction_WineViewBookshelf);
    public const string HintAction_WineBottleGrabbed   = nameof(HintAction_WineBottleGrabbed);
    public const string HintAction_WineBottlePlaced    = nameof(HintAction_WineBottlePlaced);
    public const string HintAction_WineBottleRemoved   = nameof(HintAction_WineBottleRemoved);
    public const string HintAction_WineCabinetLocked   = nameof(HintAction_WineCabinetLocked);
    public const string HintAction_WineRackSolved      = nameof(HintAction_WineRackSolved);
    public const string HintAction_WineKeyGrabbed      = nameof(HintAction_WineKeyGrabbed);


    /*
     * Action Labels — Organ Scene
     */

    public const string HintAction_OrganStep02 = nameof(HintAction_OrganStep02);
    public const string HintAction_OrganStep03 = nameof(HintAction_OrganStep03);
    public const string HintAction_OrganStep04 = nameof(HintAction_OrganStep04);
    public const string HintAction_OrganStep05 = nameof(HintAction_OrganStep05);
    public const string HintAction_OrganStep06 = nameof(HintAction_OrganStep06);
    public const string HintAction_OrganStep08 = nameof(HintAction_OrganStep08);
    public const string HintAction_OrganStep09 = nameof(HintAction_OrganStep09);
    public const string HintAction_OrganStep10 = nameof(HintAction_OrganStep10);
    public const string HintAction_OrganStep11 = nameof(HintAction_OrganStep11);
    public const string HintAction_OrganStep12 = nameof(HintAction_OrganStep12);
    public const string HintAction_OrganStep13 = nameof(HintAction_OrganStep13);
    public const string HintAction_OrganStep14 = nameof(HintAction_OrganStep14);
    public const string HintAction_OrganStep15 = nameof(HintAction_OrganStep15);
    public const string HintAction_OrganStep16 = nameof(HintAction_OrganStep16);

    public const string HintAction_OrganHoldLP      = nameof(HintAction_OrganHoldLP);
    public const string HintAction_OrganHoldPipe    = nameof(HintAction_OrganHoldPipe);
    public const string HintAction_OrganHoldPaper   = nameof(HintAction_OrganHoldPaper);
    public const string HintAction_OrganHoldPicture = nameof(HintAction_OrganHoldPicture);
    public const string HintAction_OrganHoldAny     = nameof(HintAction_OrganHoldAny);


    /*
     * Action Labels — Doll Scene
     */

    // DollHintBridge가 스텝 완료 시 "doll_step_NN" → DefaultActionLabelProvider가 아래 키로 바꿈 (9단계 기준)
    public const string HintAction_DollBookViewed  = nameof(HintAction_DollBookViewed);   // 1 그림책 봄
    public const string HintAction_DollLockOpened  = nameof(HintAction_DollLockOpened);   // 2 자물쇠 열림
    public const string HintAction_DollLegFixed    = nameof(HintAction_DollLegFixed);     // 3 다리 교체
    public const string HintAction_DollCoinGot     = nameof(HintAction_DollCoinGot);      // 4 동전 얻음
    public const string HintAction_DollArmGot      = nameof(HintAction_DollArmGot);       // 5 오락기에서 팔 얻음
    public const string HintAction_DollArmFixed    = nameof(HintAction_DollArmFixed);     // 6 팔 교체
    public const string HintAction_DollSpringGot   = nameof(HintAction_DollSpringGot);    // 7 태엽 얻음
    public const string HintAction_DollSpringFixed = nameof(HintAction_DollSpringFixed);  // 8 태엽 장착

    // 곰 인형 던진 횟수 — {0}에 횟수
    public const string HintAction_DollBearThrowOnce = nameof(HintAction_DollBearThrowOnce);
    public const string HintAction_DollBearThrowFew  = nameof(HintAction_DollBearThrowFew);
    public const string HintAction_DollBearThrowMany = nameof(HintAction_DollBearThrowMany);


    /*
     * Wine Scene — 구역·랜드마크 이름 (WineLocationProvider Inspector에 이 키를 적음)
     */

    public const string HintWine_ZoneRoom1    = nameof(HintWine_ZoneRoom1);
    public const string HintWine_ZoneRoom2    = nameof(HintWine_ZoneRoom2);
    public const string HintWine_ZoneRoom3    = nameof(HintWine_ZoneRoom3);
    public const string HintWine_ZoneCorridor = nameof(HintWine_ZoneCorridor);

    public const string HintWine_LandmarkWallPaper = nameof(HintWine_LandmarkWallPaper);
    public const string HintWine_LandmarkBookshelf = nameof(HintWine_LandmarkBookshelf);
    public const string HintWine_LandmarkWineRack  = nameof(HintWine_LandmarkWineRack);
    public const string HintWine_LandmarkBox       = nameof(HintWine_LandmarkBox);
    public const string HintWine_LandmarkMemo      = nameof(HintWine_LandmarkMemo);
    public const string HintWine_LandmarkCabinet   = nameof(HintWine_LandmarkCabinet);

    /*
     * Wine Scene — 단계별 힌트 (WineGlassRoomData hintByLevel, 레벨 1~5)
     */

    public const string HintWine_Step1_Level1 = nameof(HintWine_Step1_Level1);
    public const string HintWine_Step1_Level2 = nameof(HintWine_Step1_Level2);
    public const string HintWine_Step1_Level3 = nameof(HintWine_Step1_Level3);
    public const string HintWine_Step1_Level4 = nameof(HintWine_Step1_Level4);
    public const string HintWine_Step1_Level5 = nameof(HintWine_Step1_Level5);
    public const string HintWine_Step2_Level1 = nameof(HintWine_Step2_Level1);
    public const string HintWine_Step2_Level2 = nameof(HintWine_Step2_Level2);
    public const string HintWine_Step2_Level3 = nameof(HintWine_Step2_Level3);
    public const string HintWine_Step2_Level4 = nameof(HintWine_Step2_Level4);
    public const string HintWine_Step2_Level5 = nameof(HintWine_Step2_Level5);
    public const string HintWine_Step3_Level1 = nameof(HintWine_Step3_Level1);
    public const string HintWine_Step3_Level2 = nameof(HintWine_Step3_Level2);
    public const string HintWine_Step3_Level3 = nameof(HintWine_Step3_Level3);
    public const string HintWine_Step3_Level4 = nameof(HintWine_Step3_Level4);
    public const string HintWine_Step3_Level5 = nameof(HintWine_Step3_Level5);
    public const string HintWine_Step4_Level1 = nameof(HintWine_Step4_Level1);
    public const string HintWine_Step4_Level2 = nameof(HintWine_Step4_Level2);
    public const string HintWine_Step4_Level3 = nameof(HintWine_Step4_Level3);
    public const string HintWine_Step4_Level4 = nameof(HintWine_Step4_Level4);
    public const string HintWine_Step4_Level5 = nameof(HintWine_Step4_Level5);


    /*
     * Wine Scene — 진행 문장 (WineHintBridge, T0 레벨1~2 / T1 레벨3 / T2 레벨4~5)
     */

    public const string HintWine_NoteStainViewed_T0 = nameof(HintWine_NoteStainViewed_T0);
    public const string HintWine_NoteStainViewed_T1 = nameof(HintWine_NoteStainViewed_T1);
    public const string HintWine_NoteStainViewed_T2 = nameof(HintWine_NoteStainViewed_T2);
    public const string HintWine_NoteMemoGuide_T0 = nameof(HintWine_NoteMemoGuide_T0);
    public const string HintWine_NoteMemoGuide_T1 = nameof(HintWine_NoteMemoGuide_T1);
    public const string HintWine_NoteMemoGuide_T2 = nameof(HintWine_NoteMemoGuide_T2);
    public const string HintWine_NoteMemoPartial_T0 = nameof(HintWine_NoteMemoPartial_T0);
    public const string HintWine_NoteMemoPartial_T1 = nameof(HintWine_NoteMemoPartial_T1);
    public const string HintWine_NoteMemoPartial_T2 = nameof(HintWine_NoteMemoPartial_T2);
    public const string HintWine_NoteShelfViewed_T0 = nameof(HintWine_NoteShelfViewed_T0);
    public const string HintWine_NoteShelfViewed_T1 = nameof(HintWine_NoteShelfViewed_T1);
    public const string HintWine_NoteShelfViewed_T2 = nameof(HintWine_NoteShelfViewed_T2);
    public const string HintWine_NoteWrongColor_T0 = nameof(HintWine_NoteWrongColor_T0);
    public const string HintWine_NoteWrongColor_T1 = nameof(HintWine_NoteWrongColor_T1);
    public const string HintWine_NoteWrongColor_T2 = nameof(HintWine_NoteWrongColor_T2);
    public const string HintWine_NoteWrongNumber_T0 = nameof(HintWine_NoteWrongNumber_T0);
    public const string HintWine_NoteWrongNumber_T1 = nameof(HintWine_NoteWrongNumber_T1);
    public const string HintWine_NoteWrongNumber_T2 = nameof(HintWine_NoteWrongNumber_T2);
    public const string HintWine_NoteRackPartial_T0 = nameof(HintWine_NoteRackPartial_T0);
    public const string HintWine_NoteRackPartial_T1 = nameof(HintWine_NoteRackPartial_T1);
    public const string HintWine_NoteRackPartial_T2 = nameof(HintWine_NoteRackPartial_T2);
    public const string HintWine_NoteHasKey_T0 = nameof(HintWine_NoteHasKey_T0);
    public const string HintWine_NoteHasKey_T1 = nameof(HintWine_NoteHasKey_T1);
    public const string HintWine_NoteHasKey_T2 = nameof(HintWine_NoteHasKey_T2);


    /*
     * Doll Scene — 단계별 힌트 (DollRoomData.hintByLevel, 9단계 × 레벨 1~5)
     */

    public const string HintDoll_Step1_Level1 = nameof(HintDoll_Step1_Level1);
    public const string HintDoll_Step1_Level2 = nameof(HintDoll_Step1_Level2);
    public const string HintDoll_Step1_Level3 = nameof(HintDoll_Step1_Level3);
    public const string HintDoll_Step1_Level4 = nameof(HintDoll_Step1_Level4);
    public const string HintDoll_Step1_Level5 = nameof(HintDoll_Step1_Level5);
    public const string HintDoll_Step2_Level1 = nameof(HintDoll_Step2_Level1);
    public const string HintDoll_Step2_Level2 = nameof(HintDoll_Step2_Level2);
    public const string HintDoll_Step2_Level3 = nameof(HintDoll_Step2_Level3);
    public const string HintDoll_Step2_Level4 = nameof(HintDoll_Step2_Level4);
    public const string HintDoll_Step2_Level5 = nameof(HintDoll_Step2_Level5);
    public const string HintDoll_Step3_Level1 = nameof(HintDoll_Step3_Level1);
    public const string HintDoll_Step3_Level2 = nameof(HintDoll_Step3_Level2);
    public const string HintDoll_Step3_Level3 = nameof(HintDoll_Step3_Level3);
    public const string HintDoll_Step3_Level4 = nameof(HintDoll_Step3_Level4);
    public const string HintDoll_Step3_Level5 = nameof(HintDoll_Step3_Level5);
    public const string HintDoll_Step4_Level1 = nameof(HintDoll_Step4_Level1);
    public const string HintDoll_Step4_Level2 = nameof(HintDoll_Step4_Level2);
    public const string HintDoll_Step4_Level3 = nameof(HintDoll_Step4_Level3);
    public const string HintDoll_Step4_Level4 = nameof(HintDoll_Step4_Level4);
    public const string HintDoll_Step4_Level5 = nameof(HintDoll_Step4_Level5);
    public const string HintDoll_Step5_Level1 = nameof(HintDoll_Step5_Level1);
    public const string HintDoll_Step5_Level2 = nameof(HintDoll_Step5_Level2);
    public const string HintDoll_Step5_Level3 = nameof(HintDoll_Step5_Level3);
    public const string HintDoll_Step5_Level4 = nameof(HintDoll_Step5_Level4);
    public const string HintDoll_Step5_Level5 = nameof(HintDoll_Step5_Level5);
    public const string HintDoll_Step6_Level1 = nameof(HintDoll_Step6_Level1);
    public const string HintDoll_Step6_Level2 = nameof(HintDoll_Step6_Level2);
    public const string HintDoll_Step6_Level3 = nameof(HintDoll_Step6_Level3);
    public const string HintDoll_Step6_Level4 = nameof(HintDoll_Step6_Level4);
    public const string HintDoll_Step6_Level5 = nameof(HintDoll_Step6_Level5);
    public const string HintDoll_Step7_Level1 = nameof(HintDoll_Step7_Level1);
    public const string HintDoll_Step7_Level2 = nameof(HintDoll_Step7_Level2);
    public const string HintDoll_Step7_Level3 = nameof(HintDoll_Step7_Level3);
    public const string HintDoll_Step7_Level4 = nameof(HintDoll_Step7_Level4);
    public const string HintDoll_Step7_Level5 = nameof(HintDoll_Step7_Level5);
    public const string HintDoll_Step8_Level1 = nameof(HintDoll_Step8_Level1);
    public const string HintDoll_Step8_Level2 = nameof(HintDoll_Step8_Level2);
    public const string HintDoll_Step8_Level3 = nameof(HintDoll_Step8_Level3);
    public const string HintDoll_Step8_Level4 = nameof(HintDoll_Step8_Level4);
    public const string HintDoll_Step8_Level5 = nameof(HintDoll_Step8_Level5);
    public const string HintDoll_Step9_Level1 = nameof(HintDoll_Step9_Level1);
    public const string HintDoll_Step9_Level2 = nameof(HintDoll_Step9_Level2);
    public const string HintDoll_Step9_Level3 = nameof(HintDoll_Step9_Level3);
    public const string HintDoll_Step9_Level4 = nameof(HintDoll_Step9_Level4);
    public const string HintDoll_Step9_Level5 = nameof(HintDoll_Step9_Level5);

    /*
     * Doll Scene — 진행 문장 (DollHintBridge, T0 레벨1~2 / T1 레벨3 / T2 레벨4~5)
     */

    public const string HintDoll_NoteBearEarly_T0 = nameof(HintDoll_NoteBearEarly_T0);
    public const string HintDoll_NoteBearEarly_T1 = nameof(HintDoll_NoteBearEarly_T1);
    public const string HintDoll_NoteBearEarly_T2 = nameof(HintDoll_NoteBearEarly_T2);
    public const string HintDoll_NoteBearAlmost_T0 = nameof(HintDoll_NoteBearAlmost_T0);
    public const string HintDoll_NoteBearAlmost_T1 = nameof(HintDoll_NoteBearAlmost_T1);
    public const string HintDoll_NoteBearAlmost_T2 = nameof(HintDoll_NoteBearAlmost_T2);
    public const string HintDoll_NoteBearBroken_T0 = nameof(HintDoll_NoteBearBroken_T0);
    public const string HintDoll_NoteBearBroken_T1 = nameof(HintDoll_NoteBearBroken_T1);
    public const string HintDoll_NoteBearBroken_T2 = nameof(HintDoll_NoteBearBroken_T2);
    public const string HintDoll_NoteDollMoving_T0 = nameof(HintDoll_NoteDollMoving_T0);
    public const string HintDoll_NoteKeyDropped_T0 = nameof(HintDoll_NoteKeyDropped_T0);
    public const string HintDoll_NoteKeyDropped_T1 = nameof(HintDoll_NoteKeyDropped_T1);
    public const string HintDoll_NoteKeyDropped_T2 = nameof(HintDoll_NoteKeyDropped_T2);
}