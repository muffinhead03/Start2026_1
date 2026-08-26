using System.Collections;
using UnityEngine;

public class RetroGameManager : MonoBehaviour
{
    [Header("Doll Scene Game Manager")]
    [SerializeField]
    private DollScene_GameManager gameManager;


    [Header("Game State")]
    [SerializeField]
    private bool isPlaying = false;

    [SerializeField]
    private bool isCoinInserted = false;


    [Header("Reward State")]
    [SerializeField]
    private bool isArmRewardUnlocked = false;


    // =========================================================
    // 인형 팔 배출 설정
    // =========================================================

    [Header("인형 팔 배출")]

    [Tooltip("실제로 배출될 인형 팔 오브젝트")]
    [SerializeField]
    private GameObject armRewardObject;

    [Tooltip("인형 팔 배출구 DollArmComeOut")]
    [SerializeField]
    private Transform armComeOutPoint;

    [Tooltip("동전 투입 후 팔 배출까지 기다리는 시간")]
    [SerializeField]
    private float armRewardDelay = 1f;

    [Tooltip("인형 팔이 기계 밖으로 나오는 시간")]
    [SerializeField]
    private float armComeOutDuration = 1f;

    [Tooltip("월드 -Z 방향으로 배출되는 거리")]
    [SerializeField]
    private float armComeOutDistance = 0.4f;

    [Tooltip("바닥에 떨어져 안정됐다고 판단할 속도")]
    [SerializeField]
    private float settledVelocity = 0.08f;


    private bool isArmComingOut = false;


    public bool IsPlaying => isPlaying;
    public bool IsCoinInserted => isCoinInserted;
    public bool IsArmRewardUnlocked => isArmRewardUnlocked;


    // =========================================================
    // 초기화
    // =========================================================

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<DollScene_GameManager>();
        }


        if (gameManager == null)
        {
            Debug.LogError(
                "[RetroGame] DollScene_GameManager를 찾을 수 없습니다.",
                this
            );
        }


        // 아직 보상이 등장하지 않았다면 숨김
        if (armRewardObject != null &&
            !isArmRewardUnlocked)
        {
            armRewardObject.SetActive(false);
        }


        Debug.Log(
            "[RetroGame] RetroGameManager 초기화 완료"
        );
    }


    // =========================================================
    // 게임 시작
    // =========================================================

    public void StartRetroGame()
    {
        if (isPlaying)
        {
            Debug.Log(
                "[RetroGame] 이미 게임이 진행 중입니다."
            );

            return;
        }


        isPlaying = true;


        Debug.Log(
            "[RetroGame] 게임 시작 상태 저장"
        );
    }


    // =========================================================
    // 동전 투입
    // =========================================================

    public void SetCoinInserted(bool inserted)
    {
        isCoinInserted = inserted;


        Debug.Log(
            $"[RetroGame] 동전 투입 상태 : {isCoinInserted}"
        );


        if (!isCoinInserted)
        {
            return;
        }


        // 이미 등장했거나 현재 배출 중이면 중복 실행 방지
        if (isArmRewardUnlocked ||
            isArmComingOut)
        {
            return;
        }


        StartCoroutine(
            ArmRewardSequence()
        );
    }


    // =========================================================
    // 인형 팔 배출
    //
    // 동전 투입 완료
    // ↓
    // 1초 대기
    // ↓
    // DollArmComeOut 중앙에서 팔 등장
    // ↓
    // 월드 -Z 방향으로 1초간 배출
    // ↓
    // 완전히 나온 후 Collider + Gravity ON
    // ↓
    // 바닥으로 툭 떨어짐
    // ↓
    // 안정된 후 획득 가능
    // =========================================================

    private IEnumerator ArmRewardSequence()
    {
        isArmComingOut = true;


        // -----------------------------------------------------
        // 1. 동전 투입 후 대기
        // -----------------------------------------------------

        Debug.Log(
            $"[RetroGame] 인형 팔 등장까지 {armRewardDelay}초 대기"
        );


        yield return new WaitForSeconds(
            armRewardDelay
        );


        // -----------------------------------------------------
        // 필수 오브젝트 확인
        // -----------------------------------------------------

        if (armRewardObject == null)
        {
            Debug.LogError(
                "[RetroGame] Arm Reward Object가 연결되지 않았습니다.",
                this
            );

            isArmComingOut = false;
            yield break;
        }


        if (armComeOutPoint == null)
        {
            Debug.LogError(
                "[RetroGame] DollArmComeOut이 연결되지 않았습니다.",
                this
            );

            isArmComingOut = false;
            yield break;
        }


        // -----------------------------------------------------
        // 2. DollArmComeOut Collider의 중앙점
        // -----------------------------------------------------

        Vector3 startPosition =
            GetComeOutCenter();


        // -----------------------------------------------------
        // 3. 무조건 월드 -Z 방향
        //
        // Vector3.back = (0, 0, -1)
        //
        // DollArmComeOut의 Rotation과 관계없이
        // 항상 씬의 -Z 방향으로 이동합니다.
        // -----------------------------------------------------

        Vector3 comeOutDirection =
            Vector3.back;


        Vector3 endPosition =
            startPosition +
            comeOutDirection * armComeOutDistance;


        Debug.Log(
            $"[RetroGame] 팔 시작 위치 = {startPosition}, " +
            $"끝 위치 = {endPosition}, " +
            $"World -Z = {comeOutDirection}"
        );


        // -----------------------------------------------------
        // 4. 팔 위치 준비
        // -----------------------------------------------------

        armRewardObject.transform.SetParent(
            null,
            true
        );


        armRewardObject.transform.position =
            startPosition;


        /*
         * 팔의 방향은 DollArmComeOut 방향을 사용합니다.
         *
         * 이동 방향은 이 Rotation과 관계없이
         * 무조건 World -Z입니다.
         */
        armRewardObject.transform.rotation =
            armComeOutPoint.rotation;


        // -----------------------------------------------------
        // 5. 활성화하기 전에 Grab / Collider를 먼저 차단
        // -----------------------------------------------------

        SetArmGrabEnabled(false);

        SetArmColliders(false);


        // -----------------------------------------------------
        // 6. 기존 Rigidbody가 있으면 물리 정지
        // -----------------------------------------------------

        Rigidbody armBody =
            armRewardObject.GetComponent<Rigidbody>();


        if (armBody != null)
        {
            armBody.linearVelocity =
                Vector3.zero;

            armBody.angularVelocity =
                Vector3.zero;

            armBody.useGravity =
                false;

            armBody.isKinematic =
                true;
        }


        // 이제 화면에 등장
        armRewardObject.SetActive(true);


        Debug.Log(
            $"[RetroGame] 팔 활성화 = " +
            $"{armRewardObject.activeInHierarchy}"
        );


        // -----------------------------------------------------
        // 7. 월드 -Z 방향으로 1초 동안 배출
        // -----------------------------------------------------

        Debug.Log(
            "[RetroGame] 인형 팔 World -Z 방향 배출 시작"
        );


        float elapsed = 0f;


        while (elapsed < armComeOutDuration)
        {
            if (armRewardObject == null)
            {
                isArmComingOut = false;
                yield break;
            }


            elapsed +=
                Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    elapsed /
                    Mathf.Max(
                        armComeOutDuration,
                        0.0001f
                    )
                );


            // 부드럽게 배출
            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );


            armRewardObject.transform.position =
                Vector3.Lerp(
                    startPosition,
                    endPosition,
                    smoothT
                );


            yield return null;
        }


        // 정확한 마지막 위치
        armRewardObject.transform.position =
            endPosition;


        Debug.Log(
            "[RetroGame] 인형 팔 완전히 배출됨"
        );


        // -----------------------------------------------------
        // 8. 완전히 기계 밖으로 나온 다음 Collider ON
        // -----------------------------------------------------

        SetArmColliders(true);


        // -----------------------------------------------------
        // 9. 팔에 Rigidbody 확인
        //
        // 게임기 본체에는 Rigidbody 필요 없음
        // -----------------------------------------------------

        armBody =
            armRewardObject.GetComponent<Rigidbody>();


        if (armBody == null)
        {
            armBody =
                armRewardObject.AddComponent<Rigidbody>();


            Debug.Log(
                "[RetroGame] 인형 팔에 Rigidbody 자동 추가"
            );
        }


        // -----------------------------------------------------
        // 10. 이제 바닥으로 툭 떨어뜨림
        // -----------------------------------------------------

        armBody.linearVelocity =
            Vector3.zero;

        armBody.angularVelocity =
            Vector3.zero;

        armBody.isKinematic =
            false;

        armBody.useGravity =
            true;


        Debug.Log(
            "[RetroGame] 인형 팔 낙하 시작"
        );


        // 보상 등장 상태 저장
        UnlockArmReward();


        // -----------------------------------------------------
        // 11. 바닥에 떨어져 안정될 때까지 기다림
        // -----------------------------------------------------

        yield return StartCoroutine(
            WaitUntilArmSettled(
                armBody
            )
        );


        // -----------------------------------------------------
        // 12. 떨어진 다음에만 잡을 수 있게 함
        // -----------------------------------------------------

        SetArmGrabEnabled(true);


        Debug.Log(
            "[RetroGame] 인형 팔 착지 완료 → 획득 가능"
        );


        isArmComingOut = false;
    }


    // =========================================================
    // DollArmComeOut 중앙 위치
    // =========================================================

    private Vector3 GetComeOutCenter()
    {
        Collider col =
            armComeOutPoint.GetComponent<Collider>();


        /*
         * DollArmComeOut에 BoxCollider가 있으면
         * 그 Collider의 실제 월드 중앙 위치에서 시작합니다.
         */
        if (col != null)
        {
            return col.bounds.center;
        }


        return armComeOutPoint.position;
    }


    // =========================================================
    // 인형 팔 Collider
    // =========================================================

    private void SetArmColliders(
        bool enabled)
    {
        if (armRewardObject == null)
        {
            return;
        }


        Collider[] colliders =
            armRewardObject.GetComponentsInChildren<Collider>(
                true
            );


        foreach (Collider col in colliders)
        {
            if (col != null)
            {
                col.enabled =
                    enabled;
            }
        }
    }


    // =========================================================
    // 인형 팔 잡기 기능
    // =========================================================

    private void SetArmGrabEnabled(
        bool enabled)
    {
        if (armRewardObject == null)
        {
            return;
        }


        Object_Grabbable grabbable =
            armRewardObject.GetComponent<Object_Grabbable>();


        if (grabbable != null)
        {
            grabbable.enabled =
                enabled;
        }


        Event_On_Ray rayEvent =
            armRewardObject.GetComponent<Event_On_Ray>();


        if (rayEvent != null)
        {
            rayEvent.enabled =
                enabled;
        }
    }


    // =========================================================
    // 바닥에 떨어져 안정되는 것 확인
    // =========================================================

    private IEnumerator WaitUntilArmSettled(
        Rigidbody armBody)
    {
        if (armBody == null)
        {
            yield break;
        }


        /*
         * 중력이 켜진 직후 순간적으로 속도가 0이므로
         * 바로 착지했다고 판단하지 않도록 잠시 기다립니다.
         */
        yield return new WaitForSeconds(
            0.3f
        );


        float stableTime = 0f;
        float maximumWait = 5f;
        float elapsed = 0f;


        while (elapsed < maximumWait)
        {
            if (armBody == null)
            {
                yield break;
            }


            elapsed +=
                Time.deltaTime;


            if (
                armBody.linearVelocity.sqrMagnitude
                <
                settledVelocity *
                settledVelocity
            )
            {
                stableTime +=
                    Time.deltaTime;


                /*
                 * 0.3초 동안 거의 움직이지 않으면
                 * 바닥에 떨어졌다고 판단
                 */
                if (stableTime >= 0.3f)
                {
                    yield break;
                }
            }
            else
            {
                stableTime =
                    0f;
            }


            yield return null;
        }


        /*
         * 물체가 어딘가에 걸려 계속 흔들리더라도
         * 5초 후에는 잡을 수 있도록 합니다.
         */
        Debug.LogWarning(
            "[RetroGame] 인형 팔 착지 확인 시간 초과 → Grab 활성화"
        );
    }


    // =========================================================
    // 인형 팔 보상 상태
    // =========================================================

    public void UnlockArmReward()
    {
        if (isArmRewardUnlocked)
        {
            return;
        }


        isArmRewardUnlocked =
            true;


        Debug.Log(
            "[RetroGame] 인형 팔 보상 등장 상태 저장"
        );
    }


    // =========================================================
    // 게임 승리
    // =========================================================

    public void GameWin()
    {
        if (!isPlaying)
        {
            Debug.LogWarning(
                "[RetroGame] 게임 중이 아닙니다."
            );

            return;
        }


        Debug.Log(
            "[RetroGame] 게임 승리"
        );


        EndGame();
    }


    // =========================================================
    // 게임 패배
    // =========================================================

    public void GameLose()
    {
        if (!isPlaying)
        {
            Debug.LogWarning(
                "[RetroGame] 게임 중이 아닙니다."
            );

            return;
        }


        Debug.Log(
            "[RetroGame] 게임 패배"
        );


        EndGame();
    }


    // =========================================================
    // 게임 종료
    // =========================================================

    public void EndGame()
    {
        if (!isPlaying)
        {
            return;
        }


        isPlaying =
            false;


        Debug.Log(
            "[RetroGame] 게임 종료 상태 저장"
        );
    }
}