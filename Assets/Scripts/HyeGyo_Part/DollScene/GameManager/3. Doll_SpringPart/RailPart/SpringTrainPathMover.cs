using System.Collections;
using UnityEngine;


public class SpringTrainPathMover : MonoBehaviour
{
    // =========================================================
    // 이동 대상
    // =========================================================

    [Header("이동할 기차")]
    [SerializeField]
    private Transform train;


    // =========================================================
    // 이동 경로
    // =========================================================

    [Header("기차 이동 경로")]
    [SerializeField]
    private Transform[] waypoints;


    // =========================================================
    // 이동 설정
    // =========================================================

    [Header("이동 설정")]

    [SerializeField]
    private float moveSpeed = 2f;

    [SerializeField]
    private float arriveDistance = 0.01f;


    // =========================================================
    // 현재 상태
    // =========================================================

    public bool IsMoving
    {
        get;
        private set;
    }


    public bool IsMoveCompleted
    {
        get;
        private set;
    }


    // =========================================================
    // 기차 이동
    //
    // 외부 Manager에서
    //
    // yield return trainMover.MoveAlongPath();
    //
    // 형태로 호출
    // =========================================================

    public IEnumerator MoveAlongPath()
    {
        if (IsMoving)
        {
            yield break;
        }


        if (IsMoveCompleted)
        {
            yield break;
        }


        if (train == null)
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] Train이 연결되지 않았습니다."
            );

            yield break;
        }


        if (waypoints == null ||
            waypoints.Length == 0)
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] Waypoint가 없습니다."
            );

            yield break;
        }


        IsMoving =
            true;


        // =====================================================
        // WayPoint 순서대로 이동
        // =====================================================

        for (int i = 0; i < waypoints.Length; i++)
        {
            Transform targetPoint =
                waypoints[i];


            if (targetPoint == null)
            {
                continue;
            }


            // -------------------------------------------------
            // 해당 Point까지 이동
            // -------------------------------------------------

            while (
                Vector3.Distance(
                    train.position,
                    targetPoint.position
                ) > arriveDistance
            )
            {
                train.position =
                    Vector3.MoveTowards(
                        train.position,
                        targetPoint.position,
                        moveSpeed * Time.deltaTime
                    );


                yield return null;
            }


            // -------------------------------------------------
            // 최종 위치 정확하게 보정
            // -------------------------------------------------

            train.position =
                targetPoint.position;
        }


        // =====================================================
        // 이동 완료
        // =====================================================

        IsMoving =
            false;


        IsMoveCompleted =
            true;


        Debug.Log(
            "[SpringTrainPathMover] 기차 이동 완료"
        );
    }
}