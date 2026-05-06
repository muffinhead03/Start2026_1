using UnityEngine;

public interface IRayInteractable
{
    // 화면에 띄울 상호작용 텍스트 (예: "열쇠 줍기 (E)")
    string GetInteractPrompt();
    // 상호작용 시 실행될 로직
    void OnRayEnter();
    void OnRayStay();
    void OnRayExit();
    void OnRayClick();
}
