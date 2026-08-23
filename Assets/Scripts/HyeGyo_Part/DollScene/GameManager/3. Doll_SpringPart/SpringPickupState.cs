using System;
using UnityEngine;


public class SpringPickupState : MonoBehaviour
{
    // =========================================================
    // 태엽 표시 오브젝트
    // =========================================================

    [Header("태엽 Visual")]
    [Tooltip(
        "실제로 화면에 보이는 태엽 오브젝트입니다. " +
        "가능하면 이 스크립트가 붙은 오브젝트의 자식을 넣어주세요."
    )]
    [SerializeField]
    private GameObject springVisual;


    // =========================================================
    // 태엽 상호작용 Collider
    // =========================================================

    [Header("태엽 Collider - 3D 사용 시")]
    [SerializeField]
    private Collider springCollider3D;


    [Header("태엽 Collider - 2D 사용 시")]
    [SerializeField]
    private Collider2D springCollider2D;


    // =========================================================
    // 현재 상태
    // =========================================================

    public bool IsSpringRevealed
    {
        get;
        private set;
    }


    public bool IsSpringCollected
    {
        get;
        private set;
    }


    // =========================================================
    // Event
    //
    // SpringProgressManager가 이 Event를 구독함
    // =========================================================

    public event Action OnSpringCollected;


    // =========================================================
    // Unity
    // =========================================================

    private void Awake()
    {
        InitializeSpring();
    }


    // =========================================================
    // 초기화
    // =========================================================

    private void InitializeSpring()
    {
        IsSpringRevealed =
            false;


        IsSpringCollected =
            false;


        SetSpringVisible(
            false
        );
    }


    // =========================================================
    // 태엽 공개
    //
    // SpringTrainSequenceManager에서
    // 기차 연출이 끝났을 때 호출
    // =========================================================

    public void RevealSpring()
    {
        // 이미 먹은 태엽이면 다시 등장시키지 않음
        if (IsSpringCollected)
        {
            return;
        }


        // 이미 공개된 상태
        if (IsSpringRevealed)
        {
            return;
        }


        IsSpringRevealed =
            true;


        SetSpringVisible(
            true
        );


        Debug.Log(
            "[SpringPickup] 태엽 공개"
        );
    }


    // =========================================================
    // 태엽 획득
    //
    // 플레이어가 태엽을 클릭하거나
    // 상호작용했을 때 호출
    // =========================================================

    public void CollectSpring()
    {
        // 아직 기차가 밀려나지 않아
        // 태엽이 공개되지 않은 상태
        if (!IsSpringRevealed)
        {
            Debug.LogWarning(
                "[SpringPickup] 아직 태엽을 획득할 수 없습니다."
            );

            return;
        }


        // 이미 획득
        if (IsSpringCollected)
        {
            return;
        }


        IsSpringCollected =
            true;


        SetSpringVisible(
            false
        );


        Debug.Log(
            "[SpringPickup] 태엽 획득 완료"
        );


        // SpringProgressManager에게
        // 태엽 획득 사실 전달
        OnSpringCollected?.Invoke();
    }


    // =========================================================
    // 태엽 표시 / 숨김
    // =========================================================

    private void SetSpringVisible(
        bool visible
    )
    {
        if (springVisual != null)
        {
            springVisual.SetActive(
                visible
            );
        }


        if (springCollider3D != null)
        {
            springCollider3D.enabled =
                visible;
        }


        if (springCollider2D != null)
        {
            springCollider2D.enabled =
                visible;
        }
    }
}