using UnityEngine;
using UnityEngine.Events;

public class Object_FixCamera : MonoBehaviour
{
    [Header("Fix Position")]
    public Transform pivot;
    public Vector3 targetPos;

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

        player.FixCamera(pivot.position + targetPos, targetRot, this.gameObject);
    }

    public void UnFixCamera()
    {
        SceneUI.SetActiveCursor(true);
        UnFixed?.Invoke();

        player.UnFixCamera();
    }
}
