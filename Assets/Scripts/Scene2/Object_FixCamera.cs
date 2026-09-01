using UnityEngine;
using UnityEngine.Events;

public class Object_FixCamera : MonoBehaviour
{
    [Header("Fix Position")]
    public Transform pivot;
    public Vector3 targetPos;

    [Header("Optional Camera Point")]
    public bool useCameraPoint;
    public Transform cameraPoint;

    [Header("Fix Event")]
    public UnityEvent Fixed;
    public UnityEvent UnFixed;

    public Scene_UI_Manager SceneUI;

    Vector3 targetRot;

    [Header("Player")]
    public Player_FixCamera player;


    void Start()
    {
        targetRot = (-1) * targetPos;
    }

    public void FixCamera()
    {
        SceneUI.SetActiveCursor(false);
        Fixed?.Invoke();

    if (useCameraPoint && cameraPoint != null)
            {
                // 직접 지정한 Camera Point 사용
                player.FixCamera(cameraPoint.position,cameraPoint.forward,this.gameObject);
            }
    else
            {
                // 기존 Pivot + TargetPos 방식
                if(pivot!=null) player.FixCamera(pivot.position + targetPos,targetRot,this.gameObject);
                else player.FixCamera(transform.position + targetPos,targetRot,this.gameObject);
            }
    }

    public void UnFixCamera()
    {
        SceneUI.SetActiveCursor(true);
        UnFixed?.Invoke();

        player.UnFixCamera();
    }
}
