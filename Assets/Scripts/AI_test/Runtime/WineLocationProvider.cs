using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 와인씬의 플레이어 위치, 각 퍼즐 영역의 위치, 구역(방) 정보를 HintManager(ILocationAwareProvider)에 제공하는 컴포넌트.
///
/// 스텝 완료·최근 행동 기록은 WineHintBridge가 담당하고(10월 리디자인), 이 컴포넌트는 "위치 정보만" 제공하는
/// 독립 컴포넌트임. 코드에 오브젝트 이름이 박혀 있지 않아서, 퍼즐이 바뀌면 Inspector의 Tracked Objects만 다시 등록하면 됨.
/// 현재 등록할 이름 (WineGlassRoomData의 relatedObjectName):
///   HintStain → Hint_Stain(통로 벽 종이) / BookShelf → Room_3/BookShelfView / wine → 와인렉 / key1 → chestbox (1)
///   + 1단계 위치 문장용(WineHintBridge가 진행 상황에 따라 바꿔 씀): WineStains → 바닥 얼룩 하나 / Memo → Note_Alphabet
///
/// 구역(방) 판단 — 바닥 타일 묶음 기준:
///  - Zones에 "표시 이름 + 바닥 부모(Floor_Room1 / Floor_Corridor 등)"를 등록하면, 시작할 때 그 아래 Renderer 영역을 모아둠
///  - 플레이어 발밑(XZ)이 어느 구역 타일 위에 있는지로 현재 구역을 판단 (타일 하나하나로 검사 — 방 모양 그대로)
///  - 어느 타일에도 안 걸리는 순간(문턱 등)은 직전 구역 유지
///  - 대상 오브젝트의 구역: Tracked Objects의 Zone Override가 있으면 그 값, 없으면 가장 가까운 타일 기준
///    (벽 종이처럼 경계 벽에 붙은 물건은 양쪽 타일 거리가 비슷해서 Zone Override로 직접 지정하는 게 안전)
///  - 구역이 하나도 등록 안 되면 구역 정보는 null → HintEngine이 기존 거리 문장으로 동작
/// </summary>
public class WineLocationProvider : MonoBehaviour, ILocationAwareProvider, IZoneLookupProvider, ILandmarkProvider
{
    [Header("힌트 매니저 연결")]
    [SerializeField] HintManager hintManager;

    [Header("플레이어 (거리·구역 계산 기준점)")]
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

        [Tooltip("(선택) 이 물건이 속한 구역 — Zones의 Display Name과 똑같이 적기 (예: \"통로\"). " +
                 "비우면 가장 가까운 바닥 타일 기준으로 자동 판단. " +
                 "벽 종이처럼 두 구역 경계 벽에 붙은 물건은 '플레이어가 서서 보는 쪽' 구역으로 직접 적어주세요.")]
        public string zoneOverride;

        [Tooltip("(선택) 랜드마크 표시 이름 — 플레이어가 이 물건 바로 앞에 서 있으면 관찰 문장이 \"{이름} 앞에 있네\"로 나옴. " +
                 "예: 책장 / 와인렉 / 장식장 / 상자 / 액자 아래 종이 / 벽 종이. 비우면 랜드마크로 안 씀")]
        public string landmarkName;
    }

    [Header("퍼즐 영역 위치 등록 (relatedObjectName ↔ Transform)")]
    [Tooltip("WineGlassRoomData.cs의 각 스텝에 적어둔 relatedObjectName 문자열마다 하나씩 등록. " +
             "예: HintStain → Room_1/Hint_Stain, BookShelf → Room_3/BookShelfView, " +
             "wine → WineReck(와인렉), key1 → chestbox (1)(상자)")]
    [SerializeField] List<NamedTransform> trackedObjects = new List<NamedTransform>();

    [System.Serializable]
    public class FloorZone
    {
        [Tooltip("힌트 문장에 그대로 나오는 이름. 예: \"첫 번째 방\", \"통로\" — \"{이름}에 있구나\"로 만들어짐")]
        public string displayName;

        [Tooltip("이 구역 바닥 타일들의 부모 (Floor_Room1 / Floor_Room2 / Floor_Room3 / Floor_Corridor). " +
                 "아래 켜져 있는 Renderer를 전부 모음")]
        public Transform floorRoot;
    }

    [Header("구역 (바닥 타일 묶음)")]
    [SerializeField] List<FloorZone> zones = new List<FloorZone>();

    [Tooltip("타일 경계 이음새에서 판단이 끊기지 않게 주는 여유(미터)")]
    [SerializeField] float tileEdgeTolerance = 0.05f;

    [Tooltip("구역 판단 주기(초)")]
    [SerializeField] float zoneCheckInterval = 0.25f;

    [Header("랜드마크 (\"책장 앞에 있네\")")]
    [Tooltip("플레이어와 랜드마크 Transform 사이 수평 거리(미터)가 이 값 이하면 '바로 앞'으로 판단")]
    [SerializeField] float landmarkDistance = 2f;

    [Header("디버그")]
    [SerializeField] bool verboseLog = true;

    // zones와 같은 순서 — 각 구역 타일들의 월드 Bounds (시작 시 1회 수집, 바닥은 안 움직인다고 가정)
    readonly List<List<Bounds>> zoneTiles = new List<List<Bounds>>();

    string currentZone;
    float zoneEnterTime;
    float nextZoneCheck;

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

        CollectZoneTiles();
        UpdateCurrentZone(force: true);
    }

    void Update()
    {
        if (Time.time < nextZoneCheck) return;
        nextZoneCheck = Time.time + zoneCheckInterval;
        UpdateCurrentZone(force: false);
    }

    // =========================================================
    // 구역 타일 수집 / 판단
    // =========================================================

    void CollectZoneTiles()
    {
        zoneTiles.Clear();
        foreach (var zone in zones)
        {
            var tiles = new List<Bounds>();
            if (zone.floorRoot != null)
            {
                foreach (var r in zone.floorRoot.GetComponentsInChildren<Renderer>(false))
                    tiles.Add(r.bounds);
            }
            zoneTiles.Add(tiles);

            if (string.IsNullOrEmpty(zone.displayName))
                Debug.LogWarning("[WineLocationProvider] Display Name이 빈 구역이 있음 — 힌트 문장에 이름이 안 나옵니다.");
            if (tiles.Count == 0)
                Debug.LogWarning($"[WineLocationProvider] 구역 '{zone.displayName}': 바닥 타일 Renderer가 0개 — Floor Root 연결을 확인하세요.");
            else
                Log($"구역 '{zone.displayName}': 타일 {tiles.Count}개");
        }
    }

    void UpdateCurrentZone(bool force)
    {
        if (player == null || zoneTiles.Count == 0) return;

        string zone = ZoneContaining(player.position);
        if (zone == null) return;                 // 문턱 등 — 직전 구역 유지
        if (!force && zone == currentZone) return;

        if (zone != currentZone)
        {
            Log($"구역 변경: {currentZone ?? "(없음)"} → {zone}");
            currentZone = zone;
            zoneEnterTime = Time.time;
        }
    }

    /// <summary>XZ 평면에서 이 위치를 덮는 타일이 있는 구역 이름. 없으면 null.</summary>
    string ZoneContaining(Vector3 pos)
    {
        float t = tileEdgeTolerance;
        for (int i = 0; i < zoneTiles.Count; i++)
        {
            foreach (var b in zoneTiles[i])
            {
                if (pos.x >= b.min.x - t && pos.x <= b.max.x + t &&
                    pos.z >= b.min.z - t && pos.z <= b.max.z + t)
                    return zones[i].displayName;
            }
        }
        return null;
    }

    /// <summary>XZ 평면에서 이 위치와 가장 가까운 타일의 구역 이름. 타일이 하나도 없으면 null.</summary>
    string NearestZone(Vector3 pos)
    {
        string best = null;
        float bestSqr = float.MaxValue;
        for (int i = 0; i < zoneTiles.Count; i++)
        {
            foreach (var b in zoneTiles[i])
            {
                float dx = Mathf.Max(b.min.x - pos.x, 0f, pos.x - b.max.x);
                float dz = Mathf.Max(b.min.z - pos.z, 0f, pos.z - b.max.z);
                float sqr = dx * dx + dz * dz;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = zones[i].displayName;
                }
            }
        }
        return best;
    }

    NamedTransform FindTracked(string objectName)
    {
        foreach (var entry in trackedObjects)
            if (entry.objectName == objectName)
                return entry;
        return null;
    }

    // =========================================================
    // ILocationAwareProvider
    // =========================================================

    /// <summary>플레이어의 현재 월드 좌표. player Transform이 비어있으면 null.</summary>
    public Vector3? GetPlayerPosition()
        => player != null ? player.position : (Vector3?)null;

    /// <summary>
    /// trackedObjects 목록에서 objectName이 일치하는 항목을 찾아 그 Transform의 위치를 반환.
    /// 못 찾으면(등록 안 했거나 오타면) null — 이 경우 HintEngine은 proximityNote를 그냥 비움.
    /// </summary>
    public Vector3? GetObjectPosition(string objectName)
    {
        var entry = FindTracked(objectName);
        return entry != null && entry.transform != null ? entry.transform.position : (Vector3?)null;
    }

    /// <summary>플레이어가 지금 있는 구역 이름. 구역 미등록이거나 아직 어느 타일에도 안 올라갔으면 null.</summary>
    public string GetCurrentZoneName() => currentZone;

    public float GetCurrentZoneStaySeconds()
        => currentZone != null ? Time.time - zoneEnterTime : 0f;

    // =========================================================
    // ILandmarkProvider
    // =========================================================

    /// <summary>landmarkName이 있는 Tracked Objects 중 수평 거리 landmarkDistance 안에서 가장 가까운 것의 이름. 없으면 null.</summary>
    public string GetNearbyLandmarkName()
    {
        if (player == null) return null;

        string best = null;
        float bestSqr = landmarkDistance * landmarkDistance;
        Vector3 p = player.position;

        foreach (var entry in trackedObjects)
        {
            if (string.IsNullOrEmpty(entry.landmarkName) || entry.transform == null) continue;
            Vector3 o = entry.transform.position;
            float dx = o.x - p.x, dz = o.z - p.z;
            float sqr = dx * dx + dz * dz;
            if (sqr <= bestSqr)
            {
                bestSqr = sqr;
                best = entry.landmarkName;
            }
        }
        return best;
    }

    // =========================================================
    // IZoneLookupProvider
    // =========================================================

    /// <summary>대상 오브젝트의 구역. Zone Override 우선, 없으면 덮는 타일 → 가장 가까운 타일 순.</summary>
    public string GetObjectZoneName(string objectName)
    {
        var entry = FindTracked(objectName);
        if (entry == null) return null;
        if (!string.IsNullOrEmpty(entry.zoneOverride)) return entry.zoneOverride;
        if (entry.transform == null) return null;

        Vector3 pos = entry.transform.position;
        return ZoneContaining(pos) ?? NearestZone(pos);
    }

    // =========================================================
    // 디버그
    // =========================================================

    void Log(string msg)
    {
        if (verboseLog) Debug.Log($"[WineLocationProvider] {msg}");
    }

    // 이 오브젝트를 선택하면 씬 뷰에 구역별 타일 영역을 색깔별로 표시
    static readonly Color[] GizmoColors =
    {
        new Color(1f, 0.85f, 0.2f), new Color(0.3f, 0.7f, 1f),
        new Color(1f, 0.35f, 0.35f), new Color(0.4f, 1f, 0.5f), new Color(0.8f, 0.5f, 1f),
    };

    void OnDrawGizmosSelected()
    {
        for (int i = 0; i < zones.Count; i++)
        {
            if (zones[i].floorRoot == null) continue;
            Gizmos.color = GizmoColors[i % GizmoColors.Length];
            foreach (var r in zones[i].floorRoot.GetComponentsInChildren<Renderer>(false))
            {
                var b = r.bounds;
                Gizmos.DrawWireCube(new Vector3(b.center.x, b.max.y + 0.02f, b.center.z),
                                    new Vector3(b.size.x, 0.01f, b.size.z));
            }
        }
    }
}