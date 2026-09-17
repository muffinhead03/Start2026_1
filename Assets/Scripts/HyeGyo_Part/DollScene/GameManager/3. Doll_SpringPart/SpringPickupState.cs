using System;
using UnityEngine;


public class SpringPickupState : MonoBehaviour
{
    // =========================================================
    // 실제 획득할 태엽 오브젝트
    // =========================================================

    [Header("실제 태엽 Pickup 오브젝트")]
    [Tooltip(
        "Mesh / Collider / Event_On_Ray / Object_Grabbable이 " +
        "붙어 있는 실제 태엽 오브젝트 전체를 넣습니다."
    )]
    [SerializeField]
    private GameObject springPickupObject;


    // =========================================================
    // State
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
        IsSpringRevealed = false;
        IsSpringCollected = false;


        if (springPickupObject != null)
        {
            springPickupObject.SetActive(false);
        }
    }


    // =========================================================
    // 기차 연출 종료 → 태엽 전체 활성화
    // =========================================================

    public void RevealSpring()
    {
        if (IsSpringCollected)
        {
            return;
        }


        if (IsSpringRevealed)
        {
            return;
        }


        IsSpringRevealed = true;


        if (springPickupObject != null)
        {
            springPickupObject.SetActive(true);
        }


        Debug.Log(
            "[SpringPickup] 태엽 오브젝트 활성화"
        );
    }


    // =========================================================
    // 실제 획득
    // =========================================================

    public void CollectSpring()
    {
        if (!IsSpringRevealed)
        {
            Debug.LogWarning(
                "[SpringPickup] 아직 태엽을 획득할 수 없습니다."
            );

            return;
        }


        if (IsSpringCollected)
        {
            return;
        }


        IsSpringCollected = true;


        Debug.Log(
            "[SpringPickup] 태엽 획득 완료"
        );


        OnSpringCollected?.Invoke();


        // Object_Grabbable.OnGrab()이
        // 물체를 제거/비활성화한다면 여기서 굳이 끌 필요 없음.
        //
        // 필요하면 아래를 사용해도 됨.
        //
        // if (springPickupObject != null)
        // {
        //     springPickupObject.SetActive(false);
        // }
    }
}