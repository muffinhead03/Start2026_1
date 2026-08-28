using System;
using System.Collections.Generic;
using UnityEngine;


public enum SpringTrainDirection
{
    Clockwise = 1,
    CounterClockwise = -1
}


public class SpringTrainTrack : MonoBehaviour
{
    // =========================================================
    // Section
    // =========================================================

    [Serializable]
    public class RailSection
    {
        [Header("구분용 이름")]
        public string sectionName;


        [Header("이 선로의 중심점")]
        public Transform centerPoint;


        [Header("시계방향으로 다음 선로까지 가는 보조 경로")]
        public Transform[] clockwisePathPoints;
    }


    // =========================================================
    // 전체 Section
    // =========================================================

    [Header("시계 방향 순서로 선로 12개")]
    [SerializeField]
    private RailSection[] sections =
        new RailSection[12];


    public int SectionCount
    {
        get
        {
            if (sections == null)
            {
                return 0;
            }


            return sections.Length;
        }
    }


    // =========================================================
    // Runtime Path
    // =========================================================

    public class PathSnapshot
    {
        private readonly Vector3[] points;

        private readonly float[] segmentLengths;

        private readonly float[] cumulativeDistances;

        private readonly float totalLength;


        public float TotalLength =>
            totalLength;


        public int PointCount
        {
            get
            {
                if (points == null)
                {
                    return 0;
                }


                return points.Length;
            }
        }


        // =====================================================
        // 생성
        // =====================================================

        public PathSnapshot(
            List<Vector3> sourcePoints
        )
        {
            if (
                sourcePoints == null ||
                sourcePoints.Count < 2
            )
            {
                points =
                    Array.Empty<Vector3>();

                segmentLengths =
                    Array.Empty<float>();

                cumulativeDistances =
                    Array.Empty<float>();

                totalLength = 0f;

                return;
            }


            points =
                sourcePoints.ToArray();


            segmentLengths =
                new float[points.Length];


            cumulativeDistances =
                new float[points.Length];


            float distance = 0f;


            for (int i = 0; i < points.Length; i++)
            {
                cumulativeDistances[i] =
                    distance;


                int nextIndex =
                    (i + 1) %
                    points.Length;


                float segmentLength =
                    Vector3.Distance(
                        points[i],
                        points[nextIndex]
                    );


                segmentLengths[i] =
                    segmentLength;


                distance +=
                    segmentLength;
            }


            totalLength =
                distance;
        }


        // =====================================================
        // 거리 Wrap
        // =====================================================

        public float WrapDistance(
            float distance
        )
        {
            if (totalLength <= 0.0001f)
            {
                return 0f;
            }


            return Mathf.Repeat(
                distance,
                totalLength
            );
        }


        // =====================================================
        // 지정 거리의 World Position
        //
        // Point A -> Point B는 무조건 직선
        // =====================================================

        public Vector3 EvaluatePosition(
            float distance
        )
        {
            if (
                points == null ||
                points.Length == 0
            )
            {
                return Vector3.zero;
            }


            if (points.Length == 1)
            {
                return points[0];
            }


            float wrapped =
                WrapDistance(
                    distance
                );


            for (int i = 0; i < points.Length; i++)
            {
                float startDistance =
                    cumulativeDistances[i];


                float length =
                    segmentLengths[i];


                float endDistance =
                    startDistance +
                    length;


                if (
                    wrapped <= endDistance ||
                    i == points.Length - 1
                )
                {
                    if (length <= 0.0001f)
                    {
                        return points[i];
                    }


                    float t =
                        Mathf.Clamp01(
                            (
                                wrapped -
                                startDistance
                            ) /
                            length
                        );


                    int nextIndex =
                        (i + 1) %
                        points.Length;


                    return Vector3.Lerp(
                        points[i],
                        points[nextIndex],
                        t
                    );
                }
            }


            return points[0];
        }


        // =====================================================
        // 현재 직선 Segment 방향
        // =====================================================

        public Vector3 EvaluateDirection(
            float distance,
            SpringTrainDirection direction
        )
        {
            if (
                points == null ||
                points.Length < 2
            )
            {
                return Vector3.forward;
            }


            float wrapped =
                WrapDistance(
                    distance
                );


            for (int i = 0; i < points.Length; i++)
            {
                float start =
                    cumulativeDistances[i];


                float end =
                    start +
                    segmentLengths[i];


                if (
                    wrapped <= end ||
                    i == points.Length - 1
                )
                {
                    int next =
                        (i + 1) %
                        points.Length;


                    Vector3 result =
                        points[next] -
                        points[i];


                    if (
                        direction ==
                        SpringTrainDirection.CounterClockwise
                    )
                    {
                        result =
                            -result;
                    }


                    if (
                        result.sqrMagnitude <
                        0.000001f
                    )
                    {
                        return Vector3.forward;
                    }


                    return result.normalized;
                }
            }


            return Vector3.forward;
        }


        // 기존 코드 호환용
        public Vector3 EvaluateDirection(
            float distance,
            SpringTrainDirection direction,
            float lookAheadDistance
        )
        {
            return EvaluateDirection(
                distance,
                direction
            );
        }


        // =====================================================
        // 현재 World Position에서
        // 가장 가까운 Path 거리
        //
        // 이것은 시작할 때 딱 한 번만 사용
        // =====================================================

        public float GetNearestDistance(
            Vector3 worldPosition
        )
        {
            if (
                points == null ||
                points.Length < 2
            )
            {
                return 0f;
            }


            float nearestSqr =
                float.MaxValue;


            float nearestDistance =
                0f;


            for (int i = 0; i < points.Length; i++)
            {
                int next =
                    (i + 1) %
                    points.Length;


                Vector3 start =
                    points[i];


                Vector3 end =
                    points[next];


                Vector3 segment =
                    end -
                    start;


                float sqrLength =
                    segment.sqrMagnitude;


                if (sqrLength <= 0.000001f)
                {
                    continue;
                }


                float t =
                    Vector3.Dot(
                        worldPosition - start,
                        segment
                    ) /
                    sqrLength;


                t =
                    Mathf.Clamp01(
                        t
                    );


                Vector3 nearest =
                    start +
                    segment * t;


                float sqrDistance =
                    (
                        worldPosition -
                        nearest
                    ).sqrMagnitude;


                if (sqrDistance < nearestSqr)
                {
                    nearestSqr =
                        sqrDistance;


                    nearestDistance =
                        cumulativeDistances[i] +
                        segmentLengths[i] *
                        t;
                }
            }


            return WrapDistance(
                nearestDistance
            );
        }
    }


    // =========================================================
    // Path 생성
    // =========================================================

    public PathSnapshot BuildPathSnapshot()
    {
        List<Vector3> points =
            new List<Vector3>();


        if (sections == null)
        {
            return null;
        }


        for (int i = 0; i < sections.Length; i++)
        {
            RailSection section =
                sections[i];


            if (section == null)
            {
                continue;
            }


            // -----------------------------------------
            // TrainCenter
            // -----------------------------------------

            if (section.centerPoint != null)
            {
                AddPointIfNeeded(
                    points,
                    section.centerPoint.position
                );
            }


            // -----------------------------------------
            // Element 0 -> Element 1 -> ...
            // -----------------------------------------

            if (
                section.clockwisePathPoints ==
                null
            )
            {
                continue;
            }


            for (
                int p = 0;
                p < section.clockwisePathPoints.Length;
                p++
            )
            {
                Transform point =
                    section.clockwisePathPoints[p];


                if (point == null)
                {
                    continue;
                }


                AddPointIfNeeded(
                    points,
                    point.position
                );
            }
        }


        if (points.Count < 2)
        {
            Debug.LogWarning(
                "[SpringTrainTrack] Path Point가 부족합니다.",
                this
            );


            return null;
        }


        return new PathSnapshot(
            points
        );
    }


    // =========================================================
    // 같은 좌표 중복 방지
    // =========================================================

    private void AddPointIfNeeded(
        List<Vector3> points,
        Vector3 position
    )
    {
        if (points.Count == 0)
        {
            points.Add(
                position
            );

            return;
        }


        Vector3 previous =
            points[
                points.Count - 1
            ];


        if (
            Vector3.SqrMagnitude(
                previous -
                position
            ) <
            0.000001f
        )
        {
            return;
        }


        points.Add(
            position
        );
    }


    // =========================================================
    // 가장 가까운 Section
    // =========================================================

    public int GetNearestSectionIndex(
        Vector3 worldPosition
    )
    {
        if (
            sections == null ||
            sections.Length == 0
        )
        {
            return -1;
        }


        float nearest =
            float.MaxValue;


        int result =
            -1;


        for (int i = 0; i < sections.Length; i++)
        {
            RailSection section =
                sections[i];


            if (
                section == null ||
                section.centerPoint == null
            )
            {
                continue;
            }


            float sqr =
                (
                    section.centerPoint.position -
                    worldPosition
                ).sqrMagnitude;


            if (sqr < nearest)
            {
                nearest = sqr;

                result = i;
            }
        }


        return result;
    }


    // =========================================================
    // 다음 Section
    // =========================================================

    public int GetNextIndex(
        int index,
        SpringTrainDirection direction
    )
    {
        if (SectionCount <= 0)
        {
            return -1;
        }


        int result =
            index +
            (
                direction ==
                SpringTrainDirection.Clockwise
                    ? 1
                    : -1
            );


        result %=
            SectionCount;


        if (result < 0)
        {
            result +=
                SectionCount;
        }


        return result;
    }
}