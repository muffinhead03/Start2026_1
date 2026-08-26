using System.Collections;
using UnityEngine;

public class DollKeyDropManager : MonoBehaviour
{
    [Header("Doll Pose")]
    [SerializeField]
    private DollPoseMoveManager poseMoveManager;


    [Header("Change Doll")]
    [SerializeField]
    private DollScene_ChangeDoll changeDoll;


    [Header("Key")]
    [SerializeField]
    private GameObject keyObject;

    [SerializeField]
    private CapsuleCollider keyCollider;

    [SerializeField]
    private Rigidbody keyRigidbody;

    [SerializeField]
    private Object_Grabbable keyGrabbable;

    [SerializeField]
    private Event_On_Ray keyRayEvent;


    [Header("착지")]
    [SerializeField]
    private float settledVelocity = 0.08f;

    [SerializeField]
    private float maximumSettleWait = 5f;


    private bool repairCompleted = false;
    private bool dropStarted = false;


    private void Start()
    {
        if (poseMoveManager == null)
        {
            poseMoveManager =
                FindFirstObjectByType<DollPoseMoveManager>();
        }

        if (changeDoll == null)
        {
            changeDoll =
                FindFirstObjectByType<DollScene_ChangeDoll>();
        }


        if (keyObject == null)
        {
            Debug.LogError(
                "[DollKeyDrop] Key Object가 없습니다.",
                this
            );

            return;
        }


        // 자동으로 컴포넌트 찾기
        if (keyCollider == null)
        {
            keyCollider =
                keyObject.GetComponentInChildren<CapsuleCollider>(
                    true
                );
        }


        if (keyRigidbody == null)
        {
            keyRigidbody =
                keyObject.GetComponent<Rigidbody>();
        }


        if (keyGrabbable == null)
        {
            keyGrabbable =
                keyObject.GetComponentInChildren<Object_Grabbable>(
                    true
                );
        }


        if (keyRayEvent == null)
        {
            keyRayEvent =
                keyObject.GetComponentInChildren<Event_On_Ray>(
                    true
                );
        }


        LockKey();
    }


    private void OnEnable()
    {
        if (poseMoveManager != null)
        {
            poseMoveManager.OnFinalPoseCompleted +=
                OnDollPoseCompleted;
        }
    }


    private void OnDisable()
    {
        if (poseMoveManager != null)
        {
            poseMoveManager.OnFinalPoseCompleted -=
                OnDollPoseCompleted;
        }
    }


    // =========================================================
    // DollMoveManager에서 호출
    // =========================================================

    public void SetRepairCompleted(
        bool completed)
    {
        if (!completed)
            return;


        if (repairCompleted)
            return;


        repairCompleted =
            true;


        Debug.Log(
            "[DollKeyDrop] 전체 수리 완료 전달받음"
        );


        // 먼저 인형 자세 복구
        if (poseMoveManager != null)
        {
            poseMoveManager.MoveToFinalPose();
        }
        else
        {
            // Pose Manager가 없으면 바로 Key Drop
            StartKeyDrop();
        }
    }


    // =========================================================
    // 초기 Key 상태
    // =========================================================

    private void LockKey()
    {
        if (keyCollider != null)
        {
            keyCollider.enabled =
                false;
        }


        if (keyRigidbody != null)
        {
            keyRigidbody.linearVelocity =
                Vector3.zero;

            keyRigidbody.angularVelocity =
                Vector3.zero;

            keyRigidbody.useGravity =
                false;

            keyRigidbody.isKinematic =
                true;
        }


        if (keyGrabbable != null)
        {
            keyGrabbable.enabled =
                false;
        }


        if (keyRayEvent != null)
        {
            keyRayEvent.enabled =
                false;
        }


        Debug.Log(
            "[DollKeyDrop] Key 초기 잠금"
        );
    }


    // =========================================================
    // 인형 Final Pose 이동 완료
    // =========================================================

    private void OnDollPoseCompleted()
    {
        if (!repairCompleted)
            return;


        StartKeyDrop();
    }


    // =========================================================
    // Key Drop 시작
    // =========================================================

    private void StartKeyDrop()
    {
        if (dropStarted)
            return;


        dropStarted =
            true;


        Debug.Log(
            "[DollKeyDrop] 인형 움직임 완료 → Key Drop 시작"
        );


        // 인형 손에서 분리
        keyObject.transform.SetParent(
            null,
            true
        );


        // Collider 활성화
        if (keyCollider != null)
        {
            keyCollider.enabled =
                true;
        }


        // Rigidbody가 없다면 추가
        if (keyRigidbody == null)
        {
            keyRigidbody =
                keyObject.AddComponent<Rigidbody>();
        }


        keyRigidbody.linearVelocity =
            Vector3.zero;

        keyRigidbody.angularVelocity =
            Vector3.zero;


        // 중력 활성화
        keyRigidbody.isKinematic =
            false;

        keyRigidbody.useGravity =
            true;


        StartCoroutine(
            WaitUntilKeySettled()
        );
    }


    // =========================================================
    // Key 착지 확인
    // =========================================================

    private IEnumerator WaitUntilKeySettled()
    {
        yield return new WaitForSeconds(
            0.3f
        );


        float stableTime = 0f;
        float elapsed = 0f;


        while (elapsed < maximumSettleWait)
        {
            if (keyRigidbody == null)
                yield break;


            elapsed +=
                Time.deltaTime;


            if (
                keyRigidbody.linearVelocity.sqrMagnitude
                <
                settledVelocity *
                settledVelocity
            )
            {
                stableTime +=
                    Time.deltaTime;


                if (stableTime >= 0.3f)
                {
                    break;
                }
            }
            else
            {
                stableTime =
                    0f;
            }


            yield return null;
        }


        KeyDropCompleted();
    }


    // =========================================================
    // Key 착지 완료
    // =========================================================

    private void KeyDropCompleted()
    {
        // 이제 Key 획득 가능
        if (keyGrabbable != null)
        {
            keyGrabbable.enabled =
                true;
        }


        if (keyRayEvent != null)
        {
            keyRayEvent.enabled =
                true;
        }


        Debug.Log(
            "[DollKeyDrop] Key 착지 완료 → 획득 가능"
        );


        // =============================================
        // ChangeDoll에게 완료 전달
        // =============================================

        if (changeDoll != null)
        {
            changeDoll.ReportExitKeyDropped();
        }
    }
}