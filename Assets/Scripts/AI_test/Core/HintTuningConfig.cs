using UnityEngine;

/// <summary>
/// HintEngine.Calculate()의 점수 계산에 쓰이는 임계값들을 모아둔 튜닝 설정.
/// 원래 HintEngine 안에 하드코딩돼 있던 숫자들을 뽑아낸 것 — 데모/테스트마다
/// 코드 수정 없이 Inspector에서 바로 조절하기 위함.
/// 기본값은 원래 하드코딩돼 있던 값과 동일하다.
/// </summary>
[System.Serializable]
public class HintTuningConfig
{
    [Header("체류시간 히스토리 점수 (CalcHintHistoryScore, 가중치 35%)")]
    public float staySecondsMidThreshold  = 120f;  // 이 초 이상 체류하면 +staySecondsMidBonus
    public float staySecondsMidBonus      = 1.5f;
    public float staySecondsHighThreshold = 300f;  // 이 초 이상이면 Mid 대신 +staySecondsHighBonus
    public float staySecondsHighBonus     = 3f;

    [Header("정체시간 점수 (CalcStagnationScore, 가중치 20%)")]
    public float stagnationStaySecondsDivisor = 120f; // staySeconds ÷ 이 값 = 정체 점수 기여분 (원래 staySeconds/60*0.5와 동일)

    [Header("레벨 컷라인 (총점 → 힌트레벨 1~5)")]
    public float level2Cutline = 2f;
    public float level3Cutline = 3f;
    public float level4Cutline = 4f;
    public float level5Cutline = 5f;
}