using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Object_Metro : MonoBehaviour
{
    public UnityEvent OnClick;
    public Transform pivot;

    [Header("빛 효과")]
    public MeshRenderer[] lights;
    public Material[] materials;

    [Header("애니메이션")]
    public Animator anim;

    [Header("해제 이벤트")]
    public UnityEvent UnlockEvent;

    int count;

    InputAction click;
    bool active;

    Play_Audio audio_player;

    void Start()
    {
        click = InputSystem.actions.FindAction("Click");
        click.Disable();

        active = false;

        audio_player = GetComponent<Play_Audio>();
    }

    void PressButton()
    {
        if (!active) return;

        OnClick?.Invoke();
        if (pivot.localRotation.eulerAngles.x >= 320 && pivot.localRotation.eulerAngles.x <= 330)
        {
            if (count < 0) count = 1;
            else count++;
        }
        else count--;

        TurnLight(count);

        Debug.Log(count);
        if (count == -3) GetComponent<Object_FixCamera>().UnFixCamera();
        else if (count == 3)
        {
            UnlockEvent?.Invoke();
            GetComponent<Object_FixCamera>().UnFixCamera();
        }
    }

    public void SetEnable(bool active)
    {
        this.active = active;

        if (active)
        {
            click.performed += ctx => PressButton();
            click.Enable();

            anim.SetInteger("On", 1);
            audio_player.PlayAudioLoop();
        }
        else
        {
            click.performed -= ctx => PressButton();
            click.Disable(); 
            
            ResetGame();

            anim.SetInteger("On", 0);
            audio_player.PauseAudio();
        }
    }

    void ResetGame()
    {
        count = 0;

        TurnLight(0);
    }

    void TurnLight(int count)
    {
        int i;
        for(i = 0; i < lights.Length && i < count; i++)
        {
            lights[i].material = materials[0];
        }

        for (; i < lights.Length; i++)
        {
            lights[i].material = materials[1];
        }
    }
}
