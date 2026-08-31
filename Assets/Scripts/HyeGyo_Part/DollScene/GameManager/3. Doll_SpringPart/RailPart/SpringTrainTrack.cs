using UnityEngine;


public enum SpringTrainDirection
{
    Clockwise = 1,
    CounterClockwise = -1
}


public class SpringTrainTrack : MonoBehaviour
{
    // =========================================================
    // Point
    // =========================================================

    private const int MaxPointCount =
        101;


    [Header("사용할 마지막 Point Index")]

    [Tooltip(
        "0부터 이 번호까지 사용합니다.\n" +
        "예: 30이면 Point 0 ~ Point 30 사용"
    )]

    [Range(1, 90)]
    [SerializeField]
    private int lastPointIndex =
        20;


    [Header("기차 이동 Point 0 ~ 90")]

    [Tooltip(
        "각 Transform의 Position과 Rotation을 " +
        "기차 경로로 사용합니다."
    )]

    [SerializeField]
    private Transform[] points =
        new Transform[MaxPointCount];


    // =========================================================
    // Runtime Cache
    // =========================================================

    private float[] cumulativeDistances =
        new float[MaxPointCount];


    private float totalLength;


    private bool isCacheValid;


    // =========================================================
    // 외부 접근
    // =========================================================

    public int LastPointIndex =>
        lastPointIndex;


    public float TotalLength
    {
        get
        {
            EnsureCache();

            return totalLength;
        }
    }


    // =========================================================
    // Unity
    // =========================================================

    private void Awake()
    {
        RebuildCache();
    }


    // =========================================================
    // Cache
    // =========================================================

    public bool RebuildCache()
    {
        EnsurePointArraySize();


        isCacheValid =
            false;


        totalLength =
            0f;


        if (
            lastPointIndex < 1 ||
            lastPointIndex >= MaxPointCount
        )
        {
            return false;
        }


        // -----------------------------------------------------
        // Null 검사
        // -----------------------------------------------------

        for (
            int i = 0;
            i <= lastPointIndex;
            i++
        )
        {
            if (points[i] != null)
            {
                continue;
            }


            Debug.LogWarning(
                "[SpringTrainTrack] " +
                $"Point {i}가 비어 있습니다.",
                this
            );


            return false;
        }


        // -----------------------------------------------------
        // 누적 거리 생성
        // -----------------------------------------------------

        cumulativeDistances[0] =
            0f;


        for (
            int i = 1;
            i <= lastPointIndex;
            i++
        )
        {
            float segmentLength =
                Vector3.Distance(
                    points[i - 1].position,
                    points[i].position
                );


            totalLength +=
                segmentLength;


            cumulativeDistances[i] =
                totalLength;
        }


        if (totalLength <= 0.0001f)
        {
            Debug.LogWarning(
                "[SpringTrainTrack] Path 길이가 너무 짧습니다.",
                this
            );


            return false;
        }


        isCacheValid =
            true;


        return true;
    }


    private void EnsureCache()
    {
        if (isCacheValid)
        {
            return;
        }


        RebuildCache();
    }


    // =========================================================
    // 지정 Path Distance의 Pose
    //
    // Position + Rotation 둘 다 계산
    // =========================================================

    public Pose EvaluatePose(
        float distance
    )
    {
        EnsureCache();


        if (!isCacheValid)
        {
            return new Pose(
                transform.position,
                transform.rotation
            );
        }


        // =====================================================
        // Point 0 이전
        //
        // Rear Car가 Point 0보다 뒤에 있을 수 있으므로
        // 첫 Segment 방향으로 연장
        // =====================================================

        if (distance <= 0f)
        {
            Transform first =
                points[0];


            Transform second =
                points[1];


            Vector3 direction =
                second.position -
                first.position;


            if (
                direction.sqrMagnitude >
                0.000001f
            )
            {
                direction.Normalize();
            }
            else
            {
                direction =
                    first.forward;
            }


            Vector3 position =
                first.position +
                direction *
                distance;


            return new Pose(
                position,
                first.rotation
            );
        }


        // =====================================================
        // 마지막 Point 이후
        //
        // Front Car가 마지막 Point보다 앞에 있을 수 있으므로
        // 마지막 Segment 방향으로 연장
        // =====================================================

        if (distance >= totalLength)
        {
            Transform previous =
                points[
                    lastPointIndex - 1
                ];


            Transform last =
                points[
                    lastPointIndex
                ];


            Vector3 direction =
                last.position -
                previous.position;


            if (
                direction.sqrMagnitude >
                0.000001f
            )
            {
                direction.Normalize();
            }
            else
            {
                direction =
                    last.forward;
            }


            float extraDistance =
                distance -
                totalLength;


            Vector3 position =
                last.position +
                direction *
                extraDistance;


            return new Pose(
                position,
                last.rotation
            );
        }


        // =====================================================
        // 일반 Segment
        // =====================================================

        for (
            int i = 0;
            i < lastPointIndex;
            i++
        )
        {
            float segmentStart =
                cumulativeDistances[i];


            float segmentEnd =
                cumulativeDistances[i + 1];


            if (distance > segmentEnd)
            {
                continue;
            }


            float segmentLength =
                segmentEnd -
                segmentStart;


            float t =
                0f;


            if (segmentLength > 0.0001f)
            {
                t =
                    Mathf.Clamp01(
                        (
                            distance -
                            segmentStart
                        ) /
                        segmentLength
                    );
            }


            Transform start =
                points[i];


            Transform end =
                points[i + 1];


            Vector3 position =
                Vector3.Lerp(
                    start.position,
                    end.position,
                    t
                );


            Quaternion rotation =
                Quaternion.Slerp(
                    start.rotation,
                    end.rotation,
                    t
                );


            return new Pose(
                position,
                rotation
            );
        }


        Transform finalPoint =
            points[lastPointIndex];


        return new Pose(
            finalPoint.position,
            finalPoint.rotation
        );
    }


    // =========================================================
    // Point Index → Path Distance
    // =========================================================

    public float GetDistanceAtPoint(
        int pointIndex
    )
    {
        EnsureCache();


        if (!isCacheValid)
        {
            return 0f;
        }


        pointIndex =
            Mathf.Clamp(
                pointIndex,
                0,
                lastPointIndex
            );


        return cumulativeDistances[
            pointIndex
        ];
    }


    // =========================================================
    // Point 접근
    // =========================================================

    public Transform GetPoint(
        int pointIndex
    )
    {
        if (
            pointIndex < 0 ||
            pointIndex > lastPointIndex
        )
        {
            return null;
        }


        return points[
            pointIndex
        ];
    }


    // =========================================================
    // 자식 Transform 자동 등록
    //
    // Hierarchy 순서대로
    // Point 0, 1, 2...
    // =========================================================

    [ContextMenu(
        "자식 순서대로 Point 자동 등록"
    )]
    private void AutoAssignFromChildren()
    {
        EnsurePointArraySize();


        int childCount =
            Mathf.Min(
                transform.childCount,
                MaxPointCount
            );


        if (childCount < 2)
        {
            Debug.LogWarning(
                "[SpringTrainTrack] 자식 Point가 2개 이상 필요합니다.",
                this
            );


            return;
        }


        for (
            int i = 0;
            i < MaxPointCount;
            i++
        )
        {
            points[i] =
                null;
        }


        for (
            int i = 0;
            i < childCount;
            i++
        )
        {
            points[i] =
                transform.GetChild(
                    i
                );
        }


        lastPointIndex =
            childCount - 1;


        isCacheValid =
            false;


        RebuildCache();


        Debug.Log(
            "[SpringTrainTrack] " +
            $"Point 0 ~ {lastPointIndex} 자동 등록 완료",
            this
        );
    }


    // =========================================================
    // Array 크기 보정
    // =========================================================

    private void EnsurePointArraySize()
    {
        if (
            points != null &&
            points.Length ==
            MaxPointCount
        )
        {
            return;
        }


        Transform[] newPoints =
            new Transform[
                MaxPointCount
            ];


        if (points != null)
        {
            int copyCount =
                Mathf.Min(
                    points.Length,
                    MaxPointCount
                );


            for (
                int i = 0;
                i < copyCount;
                i++
            )
            {
                newPoints[i] =
                    points[i];
            }
        }


        points =
            newPoints;
    }


    // =========================================================
    // Scene View
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        EnsurePointArraySize();


        int last =
            Mathf.Clamp(
                lastPointIndex,
                1,
                90
            );


        for (
            int i = 0;
            i <= last;
            i++
        )
        {
            Transform point =
                points[i];


            if (point == null)
            {
                continue;
            }


            Gizmos.DrawSphere(
                point.position,
                0.06f
            );


            Gizmos.DrawRay(
                point.position,
                point.forward *
                0.35f
            );


            if (
                i >= last ||
                points[i + 1] == null
            )
            {
                continue;
            }


            Gizmos.DrawLine(
                point.position,
                points[i + 1].position
            );
        }
    }


    // =========================================================
    // Inspector
    // =========================================================

    private void OnValidate()
    {
        EnsurePointArraySize();


        lastPointIndex =
            Mathf.Clamp(
                lastPointIndex,
                1,
                90
            );


        isCacheValid =
            false;
    }
}