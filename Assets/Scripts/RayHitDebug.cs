using UnityEngine;

/// <summary>
/// [임시 디버그용] 화면 가운데 레이가 지금 무엇에 맞고 있는지 Console에 찍어줌.
/// Player_Interaction과 같은 방식(Camera.main 화면 중앙, 같은 거리)으로 쏨.
/// 원인 찾으면 컴포넌트 지우고 파일도 삭제하기 — 커밋하지 말 것.
/// </summary>
public class RayHitDebug : MonoBehaviour
{
    [Tooltip("Player_Interaction의 Dist와 같은 값으로 맞추기")]
    public float dist = 3f;

    string last = "";

    void Update()
    {
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
        string now;

        if (Physics.Raycast(ray, out RaycastHit hit, dist))
        {
            var col = hit.collider;
            bool hasEvent = col.GetComponent<Event_On_Ray>() != null;
            now = $"맞은 콜라이더: {GetPath(col.transform)} | 레이어: {LayerMask.LayerToName(col.gameObject.layer)} " +
                  $"| 거리: {hit.distance:F2} | Event_On_Ray: {(hasEvent ? "있음" : "없음")}";
            Debug.DrawLine(ray.origin, hit.point, hasEvent ? Color.green : Color.red);
        }
        else
        {
            now = $"아무것도 안 맞음 (거리 {dist} 안)";
        }

        if (now != last)
        {
            Debug.Log("[RayHitDebug] " + now);
            last = now;
        }
    }

    static string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null) { t = t.parent; path = t.name + "/" + path; }
        return path;
    }
}