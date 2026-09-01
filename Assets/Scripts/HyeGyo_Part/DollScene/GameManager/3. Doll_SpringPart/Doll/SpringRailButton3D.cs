using UnityEngine;


public class SpringRailButton3D : MonoBehaviour
{
    // =========================================================
    // 버튼 종류
    // =========================================================

    public enum ButtonColor
    {
        Red,
        Yellow,
        Green
    }


    // =========================================================
    // 설정
    // =========================================================

    [Header("선로 회전 Controller")]
    [SerializeField]
    private SpringRailRotationController rotationController;


    [Header("이 버튼의 색상")]
    [SerializeField]
    private ButtonColor buttonColor;


    // =========================================================
    // 플레이어 상호작용 시 호출
    // =========================================================

    public void Interact()
    {
        if (rotationController == null)
        {
            Debug.LogWarning(
                "[SpringRailButton3D] RotationController가 연결되지 않았습니다.",
                this
            );

            return;
        }


        switch (buttonColor)
        {
            case ButtonColor.Red:

                rotationController.PressRed();

                break;


            case ButtonColor.Yellow:

                rotationController.PressYellow();

                break;


            case ButtonColor.Green:

                rotationController.PressGreen();

                break;
        }
    }


    // =========================================================
    // 마우스 테스트용
    //
    // Collider가 붙어 있으면
    // Play Mode에서 클릭하여 테스트 가능
    //
    // 나중에 플레이어 상호작용 시스템을 연결하면
    // 삭제해도 됩니다.
    // =========================================================

    private void OnMouseDown()
    {
        Interact();
    }
}