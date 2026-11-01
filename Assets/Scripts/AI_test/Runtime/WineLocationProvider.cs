using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 와인씬의 플레이어 위치와 각 퍼즐 영역의 위치를 HintManager(ILocationAwareProvider)에 제공하는 컴포넌트.
///
/// 스텝 완료·최근 행동 기록은 WineHintBridge가 담당하고(10월 리디자인), 이 컴포넌트는 "위치 정보만" 제공하는
/// 독립 컴포넌트임. 코드에 오브젝트 이름이 박혀 있지 않아서, 퍼즐이 바뀌면 Inspector의 Tracked Objects만 다시 등록하면 됨.
/// 현재 등록할 이름 (WineGlassRoomData의 relatedObjectName):
///   HintStain → Room_1/Hint_Stain / BookShelf → Room_3/BookShelfView / wine → 와인렉 / key1 → chestbox (1)
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
                 "예: \"HintStain\", \"BookShelf\", \"wine\", \"key1\"")]
        public string objectName;

        [Tooltip("이 영역을 대표하는 Transform. 개별 오브젝트 하나여도 되고, " +
                 "여러 개를 묶는 빈 부모 오브젝트(예: 와인렉 전체 \"WineReck\")의 Transform이어도 됨.")]
        public Transform transform;
    }

    [Header("퍼즐 영역 위치 등록 (relatedObjectName ↔ Transform)")]
    [Tooltip("WineGlassRoomData.cs의 각 스텝에 적어둔 relatedObjectName 문자열마다 하나씩 등록. " +
             "예: HintStain → Room_1/Hint_Stain, BookShelf → Room_3/BookShelfView, " +
             "wine → WineReck(와인렉), key1 → chestbox (1)(상자)")]
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