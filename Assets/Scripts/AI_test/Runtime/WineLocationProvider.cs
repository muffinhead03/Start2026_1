using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 와인씬의 플레이어 위치와 각 퍼즐 영역의 위치를 HintManager(ILocationAwareProvider)에 제공하는 컴포넌트.
///
/// 기존 힌트 보고 스크립트들(BookShelfComplete, FloorStainManager, WineGlass, WineRackLabel 등)은
/// completedSteps/foundClues/AddLastAction을 직접 hintManager에 보고하는 구조를 그대로 유지함 —
/// 이 컴포넌트는 그 구조와 완전히 무관하게, "위치 정보만" 별도로 붙는 독립 컴포넌트임.
/// 즉 기존 8개 스크립트는 한 줄도 안 건드려도 됨.
///
/// 구역(방별 체류시간) 추적은 트리거 콜라이더가 아직 없어서 지금은 비워둠(null/0) —
/// 나중에 Room_1/2/3에 트리거 콜라이더를 달면 그때 GetCurrentZoneName()/GetCurrentZoneStaySeconds()만
/// 채우면 되고, 그 전까지는 이 부분만 빠진 채로 나머지(플레이어-오브젝트 거리)는 정상 동작함.
/// </summary>
public class WineLocationProvider : MonoBehaviour, ILocationAwareProvider
{
    [Header("힌트 매니저 연결")]
    [SerializeField] HintManager hintManager;

    [Header("플레이어 (거리 계산 기준점)")]
    [SerializeField] Transform player;

    [System.Serializable]
    public class NamedTransform
    {
        [Tooltip("WineGlassRoomData.cs의 relatedObjectName과 정확히 똑같은 문자열이어야 매칭됨. " +
                 "예: \"WineStains\", \"StainCrossPoints\", \"WineRackLabels\", \"PuzzleBookShelf\"")]
        public string objectName;

        [Tooltip("이 영역을 대표하는 Transform. 개별 오브젝트 하나여도 되고, " +
                 "여러 개를 묶는 빈 부모 오브젝트(예: \"1.WineStains\" 그룹)의 Transform이어도 됨.")]
        public Transform transform;
    }

    [Header("퍼즐 영역 위치 등록 (relatedObjectName ↔ Transform)")]
    [Tooltip("WineGlassRoomData.cs의 각 스텝에 적어둔 relatedObjectName 문자열마다 하나씩 등록. " +
             "예: WineStains → \"1.WineStains\" 부모 오브젝트, StainCrossPoints → \"2.StainCrossPoints\" 부모, " +
             "WineRackLabels → \"3.WineRackLabels\" 부모, PuzzleBookShelf → \"5_6.PuzzleBookShelf\" 부모")]
    [SerializeField] List<NamedTransform> trackedObjects = new List<NamedTransform>();

    void Start()
    {
        if (hintManager == null)
            hintManager = FindFirstObjectByType<HintManager>();

        if (hintManager != null)
        {
            hintManager.LocationProvider = this;
            Debug.Log("[WineLocationProvider] HintManager.LocationProvider로 등록됨");
        }
        else
        {
            Debug.LogError("[WineLocationProvider] HintManager를 찾을 수 없습니다.");
        }
    }

    /// <summary>플레이어의 현재 월드 좌표. player Transform이 비어있으면 null.</summary>
    public Vector3? GetPlayerPosition()
        => player != null ? player.position : (Vector3?)null;

    /// <summary>
    /// trackedObjects 목록에서 objectName이 일치하는 항목을 찾아 그 Transform의 위치를 반환.
    /// 못 찾으면(등록 안 했거나 오타면) null — 이 경우 HintEngine은 proximityNote를 그냥 비움.
    /// </summary>
    public Vector3? GetObjectPosition(string objectName)
    {
        foreach (var entry in trackedObjects)
            if (entry.objectName == objectName && entry.transform != null)
                return entry.transform.position;
        return null;
    }

    // 구역(방별 체류시간) 시스템 — 아직 트리거 콜라이더가 없어서 항상 비움.
    // 나중에 Room_1/2/3 트리거 콜라이더 + 체류시간 타이머를 추가하면 이 두 메서드만 채우면 됨.
    public string GetCurrentZoneName() => null;
    public float GetCurrentZoneStaySeconds() => 0f;
}