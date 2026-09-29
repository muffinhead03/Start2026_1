using UnityEngine;

// 확대 조사 중인 오브젝트가 E키(Interact)를 가로채기 위한 인터페이스.
// 조사 중에는 레이가 오브젝트를 맞히든 말든 E 입력이 항상 이 오브젝트로 전달됩니다.
// (레이가 빗나가서 들고 있던 물건이 떨어지거나, 조사가 안 끝나는 문제 방지)
public interface IInspectInteractHandler
{
    void OnInteractWhileInspecting();
}

// 현재 조사 중인 오브젝트 1개를 들고 있는 전역 슬롯.
// Player_Interaction.Interact() 가 맨 먼저 확인합니다.
public static class InspectInput
{
    static IInspectInteractHandler current;

    public static IInspectInteractHandler Current
    {
        get
        {
            // 파괴된 MonoBehaviour 참조가 남아 있으면 비워줌
            if (current is Object obj && obj == null) current = null;
            return current;
        }
    }

    public static void Begin(IInspectInteractHandler handler) => current = handler;

    public static void End(IInspectInteractHandler handler)
    {
        if (current == handler) current = null;
    }

    // Domain Reload 끈 상태로 플레이해도 이전 플레이의 값이 남지 않도록 초기화
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatic() => current = null;
}