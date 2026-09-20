using UnityEngine;

// 와인 라벨 4개 다 조사되면 책장이 옆으로 밀리며 숨겨진 종이 등장
// WineLabelManager.onAllLabelsInspected 에 연결
public class ShelfSlideReveal : MonoBehaviour
{
    [Header("연결")]
    public Animator shelfAnimator;
    public GameObject hiddenPaper;

    [Header("Sound")]
    public Play_Audio audioPlayer;
    public AudioClip slideClip;

    public void OnReveal()
    {
        Debug.Log("[ShelfSlideReveal] 책장이 밀려나며 종이 발견");

        if (audioPlayer != null && slideClip != null)
            audioPlayer.PlayAudio(slideClip);

        if (shelfAnimator != null) shelfAnimator.SetTrigger("Slide");
        if (hiddenPaper != null) hiddenPaper.SetActive(true);
    }
}