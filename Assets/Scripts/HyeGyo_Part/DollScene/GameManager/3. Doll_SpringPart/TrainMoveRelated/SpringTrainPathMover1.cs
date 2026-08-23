using System.Collections;
using UnityEngine;


public class SpringTrainPathMover : MonoBehaviour
{
    // =========================================================
    // 이동 대상
    // =========================================================

    [Header("이동할 기차")]
    [SerializeField]
    private Transform movingObject;


    // =========================================================
    // WayPoint
    // =========================================================

    [Header("이동 경로")]
    [SerializeField]
    private Transform[] waypoints;


    // =========================================================
    // 이동 설정
    // =========================================================

    [Header("이동 속도")]
    [SerializeField]
    private float moveSpeed = 2f;


    [Header("도착 판정 거리")]
    [SerializeField]
    private float arriveDistance = 0.01f;


    // =========================================================
    // 상태
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
    // Unity
    // =========================================================

    private void Awake()
    {
        if (movingObject == null)
        {
            movingObject =
                transform;
        }
    }


    // =========================================================
    // 경로 이동
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


        if (movingObject == null)
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] 이동할 기차가 없습니다."
            );

            yield break;
        }


        if (waypoints == null ||
            waypoints.Length == 0)
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] WayPoint가 없습니다."
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
            Transform target =
                waypoints[i];


            if (target == null)
            {
                continue;
            }


            while (
                Vector3.Distance(
                    movingObject.position,
                    target.position
                ) > arriveDistance
            )
            {
                movingObject.position =
                    Vector3.MoveTowards(
                        movingObject.position,
                        target.position,
                        moveSpeed * Time.deltaTime
                    );


                yield return null;
            }


            movingObject.position =
                target.position;
        }


        // =====================================================
        // 완료
        // =====================================================

        IsMoving =
            false;


        IsMoveCompleted =
            true;


        Debug.Log(
            "[SpringTrainPathMover] 이동 완료 : " +
            movingObject.name
        );
    }
}