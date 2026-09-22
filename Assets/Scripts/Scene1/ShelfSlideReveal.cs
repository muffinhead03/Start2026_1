using System.Collections;
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

        if (shelfAnimator != null)
        {
            shelfAnimator.SetTrigger("Slide");
            StartCoroutine(StopSoundWhenSlideEnds());   // ← 추가
        }
        if (hiddenPaper != null) hiddenPaper.SetActive(true);
    }

    // ← 추가: 슬라이드 애니메이션이 끝나면 소리 정지
    IEnumerator StopSoundWhenSlideEnds()
    {
        // 트리거 직전 상태를 기억해 두고, 다른 상태로 넘어갈 때까지 대기
        int idleHash = shelfAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        float timeout = 3f;

        while (shelfAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == idleHash)
        {
            timeout -= Time.deltaTime;
            if (timeout <= 0f) yield break;   // 상태가 안 바뀌면 포기 (소리는 끝까지 재생)
            yield return null;
        }

        // 슬라이드 상태의 애니메이션이 끝날 때까지 대기
        int slideHash = shelfAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        while (shelfAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == slideHash &&
               shelfAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        // 소리 정지
        if (audioPlayer != null)
        {
            AudioSource src = audioPlayer.GetComponent<AudioSource>();
            if (src != null) src.Stop();
        }
    }
}