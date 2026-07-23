using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

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
            audio_player?.PlayAudio(audio_locked);
            return;
        }

        int value = (animator.GetInteger("Open") == 0) ? 1 : 0;
        animator.SetInteger("Open", value);
        audio_player?.PlayAudio(audio_open);
    }
}
