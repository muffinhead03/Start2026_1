using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using static UnityEngine.Rendering.DebugUI;

public class Object_Door : MonoBehaviour
{
    [Header("열리는 애니메이션")]
    public Animator animator;

    [Header("잠김")]
    public bool isLocked;

    [Header("사운드")]
    public AudioClip audio_open;
    public AudioClip audio_locked;

    private Play_Audio audio_player;

    void Start()
    {
        audio_player = GetComponent<Play_Audio>();
    }

    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("비밀번호 일치! 문이 열립니다.");
        OpenCloseDoor();
    }


    public void OpenCloseDoor()
    {
        if (animator == null) return;
        if (isLocked)
        {
            Object_KeyLocked keylocked = GetComponent<Object_KeyLocked>();
            if (keylocked != null)
            {
                keylocked.OnInteract();
                return;
            }

            audio_player?.PlayAudio(audio_locked);
            return;
        }

        if (animator.GetInteger("Open") == 0) OpenDoor();
        else CloseDoor();
    }

    public void OpenDoor()
    {
        if (animator.GetInteger("Open") == 1) return;

        animator.SetInteger("Open", 1);
        audio_player?.PlayAudio(audio_open);
    }

    public void CloseDoor()
    {
        if (animator.GetInteger("Open") == 0) return;

        animator.SetInteger("Open", 0);
        audio_player?.PlayAudio(audio_open);
    }
}
