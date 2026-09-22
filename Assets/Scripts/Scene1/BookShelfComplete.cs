using System.Collections;
using UnityEngine;

public class BookShelfComplete : MonoBehaviour
{
    [Header("연결")]
    public PuzzleSolver puzzleSolver;
    public HintManager hintManager;
    public Animator bookshelfAnimator;
    public GameObject hiddenKey;

    [Header("Sound")]
    public Play_Audio audioPlayer;
    public AudioClip slideClip;

    // Object_Puzzle의 UnlockEvent에 연결
    public void OnBookshelfSolved()
    {
        Debug.Log("[BookShelfComplete] 정답! 책장이 밀려나며 열쇠 발견");

        if (audioPlayer != null && slideClip != null)
            audioPlayer.PlayAudio(slideClip);

        if (bookshelfAnimator != null)
        {
            bookshelfAnimator.SetTrigger("Slide");
            StartCoroutine(StopSoundWhenSlideEnds());   // ← 추가
        }
        if (hiddenKey != null) hiddenKey.SetActive(true);

        if (hintManager != null)
        {
            if (!hintManager.currentPlayerState.completedSteps.Contains(5))
                hintManager.currentPlayerState.completedSteps.Add(5);

            if (!hintManager.currentPlayerState.completedSteps.Contains(6))
                hintManager.currentPlayerState.completedSteps.Add(6);

            if (!hintManager.currentPlayerState.foundClues.Contains("clue_bookshelf_order"))
                hintManager.currentPlayerState.foundClues.Add("clue_bookshelf_order");
        }
    }

    // ← 추가: 슬라이드 애니메이션이 끝나면 소리 정지
    IEnumerator StopSoundWhenSlideEnds()
    {
        // 트리거 직전 상태(Idle)를 기억하고, 다른 상태로 넘어갈 때까지 대기
        int idleHash = bookshelfAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        float timeout = 3f;

        while (bookshelfAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == idleHash)
        {
            timeout -= Time.deltaTime;
            if (timeout <= 0f) yield break;   // 상태가 안 바뀌면 포기
            yield return null;
        }

        // 슬라이드 애니메이션이 끝까지 재생될 때까지 대기
        int slideHash = bookshelfAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        while (bookshelfAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == slideHash &&
               bookshelfAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        // 소리 정지
        if (audioPlayer != null)
        {
            AudioSource src = audioPlayer.GetComponent<AudioSource>();
            if (src != null) src.Stop();
        }
    }
}