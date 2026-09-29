using UnityEngine;

/// <summary>
/// 플레이어 위치, 힌트 대상 오브젝트 위치, 현재 머무는 구역 정보를 HintManager에 제공하는 인터페이스.
/// IPuzzleDataProvider / IPossessionProvider와 같은 패턴 — HintManager는 이 인터페이스만 참조하고,
/// 씬마다 다른 위치/구역 추적 구현(Transform 좌표, 트리거 콜라이더 등)은 각 씬의 XxxHintBridge가 담당함.
/// (해석: 구역 시스템(트리거 콜라이더 등)이 아직 없는 씬은 GetCurrentZoneName()/GetCurrentZoneStaySeconds()만
///  null/0으로 반환하면 되고, 그래도 플레이어·오브젝트 위치 기반 거리 힌트는 정상 동작함 —
///  전부 다 구현 안 해도 부분적으로만 채워도 안전하게 동작하도록 설계됨)
/// </summary>
public interface ILocationAwareProvider
{
    /// <summary>플레이어의 현재 월드 좌표. 모르면 null.</summary>
    Vector3? GetPlayerPosition();

    /// <summary>주어진 objectName에 해당하는 오브젝트의 현재 월드 좌표. 못 찾으면 null.</summary>
    Vector3? GetObjectPosition(string objectName);

    /// <summary>플레이어가 현재 머무는 구역 이름. 구역 시스템 없으면 null.</summary>
    string GetCurrentZoneName();

    /// <summary>그 구역에 머문 시간(초). 구역 시스템 없으면 0.</summary>
    float GetCurrentZoneStaySeconds();
}