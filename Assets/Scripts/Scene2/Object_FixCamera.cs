using UnityEngine;
using UnityEngine.Events;

public class Object_FixCamera : MonoBehaviour
{
    public Vector3 targetPos;

    public UnityEvent Fixed;
    public UnityEvent UnFixed;

    public Scene_UI_Manager SceneUI;

    Vector3 targetRot;

    public Player_FixCamera player;

    bool isFixing;

    void Start()
    {
        targetRot = (-1) * targetPos;
        isFixing = false;
    }

    public void OnFix()
    {
        if (isFixing) UnFixCamera();
        else FixCamera();
    }

    void FixCamera()
    {
        isFixing = true;

        SceneUI.SetActiveCursor(false);
        Fixed?.Invoke();

        player.FixCamera(transform.position + targetPos, targetRot);
    }

    void UnFixCamera()
    {
        isFixing = false;

        SceneUI.SetActiveCursor(true);
        UnFixed?.Invoke();

        player.UnFixCamera();
    }
}
