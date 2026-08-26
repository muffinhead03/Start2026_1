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
    // 선로 한 칸
    // =========================================================

    [System.Serializable]
    public class RailSection
    {
        [Header("구분용 이름")]
        public string sectionName;


        [Header("이 선로의 중심점")]
        public Transform centerPoint;


        [Header("시계방향으로 다음 선로까지 가는 보조 경로")]
        [Tooltip(
            "현재 선로 중심 → 다음 선로 중심 사이의 중간 WayPoint입니다."
        )]
        public Transform[] clockwisePathPoints;
    }


    // =========================================================
    // 전체 Path Snapshot
    // =========================================================

    public class PathSnapshot
    {
        private Vector3[] points;

        private float[] cumulativeDistances;

        private float[] sectionDistances;

        private float totalLength;


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
            List<Vector3> sourcePoints,
            int[] sectionPointIndices
        )
        {
            points =
                sourcePoints.ToArray();


            cumulativeDistances =
                new float[points.Length];


            if (points.Length > 0)
            {
                cumulativeDistances[0] =
                    0f;
            }


            for (int i = 1; i < points.Length; i++)
            {
                cumulativeDistances[i] =
                    cumulativeDistances[i - 1] +
                    Vector3.Distance(
                        points[i - 1],
                        points[i]
                    );
            }


            if (points.Length >= 2)
            {
                totalLength =
                    cumulativeDistances[
                        points.Length - 1
                    ] +
                    Vector3.Distance(
                        points[
                            points.Length - 1
                        ],
                        points[0]
                    );
            }


            sectionDistances =
                new float[
                    sectionPointIndices.Length
                ];


            for (
                int i = 0;
                i < sectionPointIndices.Length;
                i++
            )
            {
                int pointIndex =
                    sectionPointIndices[i];


                if (
                    pointIndex >= 0 &&
                    pointIndex <
                    cumulativeDistances.Length
                )
                {
                    sectionDistances[i] =
                        cumulativeDistances[
                            pointIndex
                        ];
                }
            }
        }


        // =====================================================
        // 거리 Wrap
        // =====================================================

        public float WrapDistance(
            float distance
        )
        {
            if (totalLength <= 0f)
            {
                return 0f;
            }


            return
                Mathf.Repeat(
                    distance,
                    totalLength
                );
        }


        // =====================================================
        // 특정 Section Center의 Path 거리
        // =====================================================

        public float GetSectionDistance(
            int sectionIndex
        )
        {
            if (
                sectionDistances == null ||
                sectionDistances.Length == 0
            )
            {
                return 0f;
            }


            sectionIndex =
                Mathf.Clamp(
                    sectionIndex,
                    0,
                    sectionDistances.Length - 1
                );


            return
                sectionDistances[
                    sectionIndex
                ];
        }


        // =====================================================
        // Path상의 특정 거리 위치
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


            if (
                points.Length == 1 ||
                totalLength <= 0f
            )
            {
                return points[0];
            }


            float wrapped =
                WrapDistance(
                    distance
                );


            for (int i = 0; i < points.Length; i++)
            {
                int nextIndex =
                    (i + 1) %
                    points.Length;


                float segmentStart =
                    cumulativeDistances[i];


                float segmentEnd;


                if (
                    i ==
                    points.Length - 1
                )
                {
                    segmentEnd =
                        totalLength;
                }
                else
                {
                    segmentEnd =
                        cumulativeDistances[
                            i + 1
                        ];
                }


                if (
                    wrapped <= segmentEnd ||
                    i == points.Length - 1
                )
                {
                    float segmentLength =
                        segmentEnd -
                        segmentStart;


                    if (
                        segmentLength <=
                        0.000001f
                    )
                    {
                        return
                            points[nextIndex];
                    }


                    float t =
                        (wrapped -
                         segmentStart) /
                        segmentLength;


                    return
                        Vector3.Lerp(
                            points[i],
                            points[nextIndex],
                            t
                        );
                }
            }


            return points[0];
        }


        // =====================================================
        // 해당 거리에서의 진행 방향
        // =====================================================

        public Vector3 EvaluateDirection(
            float distance,
            SpringTrainDirection direction,
            float lookAheadDistance
        )
        {
            float probe =
                Mathf.Max(
                    0.001f,
                    lookAheadDistance
                );


            float sign =
                direction ==
                SpringTrainDirection.Clockwise
                    ? 1f
                    : -1f;


            Vector3 current =
                EvaluatePosition(
                    distance
                );


            Vector3 next =
                EvaluatePosition(
                    distance +
                    sign * probe
                );


            Vector3 result =
                next - current;


            if (
                result.sqrMagnitude <
                0.000001f
            )
            {
                Vector3 previous =
                    EvaluatePosition(
                        distance -
                        sign * probe
                    );


                result =
                    current -
                    previous;
            }


            return
                result.normalized;
        }


        // =====================================================
        // World Position에 가장 가까운 Path 거리
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


            float nearestSqrDistance =
                float.MaxValue;


            float nearestPathDistance =
                0f;


            for (int i = 0; i < points.Length; i++)
            {
                int nextIndex =
                    (i + 1) %
                    points.Length;


                Vector3 start =
                    points[i];


                Vector3 end =
                    points[nextIndex];


                Vector3 segment =
                    end - start;


                float sqrLength =
                    segment.sqrMagnitude;


                if (
                    sqrLength <
                    0.000001f
                )
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


                Vector3 nearestPoint =
                    start +
                    segment * t;


                float sqrDistance =
                    (
                        worldPosition -
                        nearestPoint
                    ).sqrMagnitude;


                if (
                    sqrDistance <
                    nearestSqrDistance
                )
                {
                    nearestSqrDistance =
                        sqrDistance;


                    float segmentStart =
                        cumulativeDistances[i];


                    float segmentLength;


                    if (
                        i ==
                        points.Length - 1
                    )
                    {
                        segmentLength =
                            totalLength -
                            segmentStart;
                    }
                    else
                    {
                        segmentLength =
                            cumulativeDistances[
                                i + 1
                            ] -
                            segmentStart;
                    }


                    nearestPathDistance =
                        segmentStart +
                        segmentLength * t;
                }
            }


            return
                WrapDistance(
                    nearestPathDistance
                );
        }
    }


    // =========================================================
    // 전체 선로
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
    // 가장 가까운 선로
    // =========================================================

    public int GetNearestSectionIndex(
        Vector3 trainCenterPosition
    )
    {
        if (
            sections == null ||
            sections.Length == 0
        )
        {
            return -1;
        }


        int nearestIndex =
            -1;


        float nearestDistance =
            float.MaxValue;


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


            float distance =
                Vector3.SqrMagnitude(
                    trainCenterPosition -
                    section.centerPoint.position
                );


            if (distance < nearestDistance)
            {
                nearestDistance =
                    distance;


                nearestIndex =
                    i;
            }
        }


        return nearestIndex;
    }


    // =========================================================
    // 다음 선로 Index
    // =========================================================

    public int GetNextIndex(
        int currentIndex,
        SpringTrainDirection direction
    )
    {
        if (SectionCount == 0)
        {
            return -1;
        }


        int offset =
            direction ==
            SpringTrainDirection.Clockwise
                ? 1
                : -1;


        return
            WrapIndex(
                currentIndex +
                offset
            );
    }


    // =========================================================
    // 전체 원형 Path Snapshot 생성
    //
    // 현재 Rail / HelpPoint의 World Position을 저장
    // 따라서 선로가 60도 회전했다면
    // 회전된 HelpPoint 위치가 그대로 사용됨
    // =========================================================

    public PathSnapshot BuildPathSnapshot()
    {
        if (
            sections == null ||
            sections.Length < 2
        )
        {
            Debug.LogWarning(
                "[SpringTrainTrack] Sections가 부족합니다."
            );


            return null;
        }


        for (int i = 0; i < sections.Length; i++)
        {
            if (
                sections[i] == null ||
                sections[i].centerPoint == null
            )
            {
                Debug.LogWarning(
                    $"[SpringTrainTrack] Section {i}의 CenterPoint가 없습니다."
                );


                return null;
            }
        }


        List<Vector3> points =
            new List<Vector3>();


        int[] sectionPointIndices =
            new int[
                sections.Length
            ];


        // 0번 Center부터 시작
        sectionPointIndices[0] =
            0;


        points.Add(
            sections[0]
                .centerPoint
                .position
        );


        // =====================================================
        // Section 0 → 1
        // Section 1 → 2
        // ...
        // Section 11 → 0
        // =====================================================

        for (
            int currentIndex = 0;
            currentIndex < sections.Length;
            currentIndex++
        )
        {
            RailSection current =
                sections[
                    currentIndex
                ];


            // 현재 Section →
            // 다음 Section 사이 HelpPoint
            if (
                current.clockwisePathPoints !=
                null
            )
            {
                for (
                    int pointIndex = 0;
                    pointIndex <
                    current.clockwisePathPoints.Length;
                    pointIndex++
                )
                {
                    Transform point =
                        current.clockwisePathPoints[
                            pointIndex
                        ];


                    if (point != null)
                    {
                        points.Add(
                            point.position
                        );
                    }
                }
            }


            int nextIndex =
                WrapIndex(
                    currentIndex + 1
                );


            // 마지막 11 → 0은
            // 마지막 점에서 points[0]으로
            // 자동으로 닫히므로 0을 다시 넣지 않음
            if (nextIndex == 0)
            {
                continue;
            }


            sectionPointIndices[
                nextIndex
            ] =
                points.Count;


            points.Add(
                sections[
                    nextIndex
                ]
                .centerPoint
                .position
            );
        }


        if (points.Count < 2)
        {
            Debug.LogWarning(
                "[SpringTrainTrack] 이동 경로 Point가 부족합니다."
            );


            return null;
        }


        PathSnapshot snapshot =
            new PathSnapshot(
                points,
                sectionPointIndices
            );


        if (
            snapshot.TotalLength <=
            0.0001f
        )
        {
            Debug.LogWarning(
                "[SpringTrainTrack] 전체 Path 길이가 0입니다."
            );


            return null;
        }


        return snapshot;
    }


    // =========================================================
    // 기존 방식도 유지
    // 한 칸 경로가 필요한 경우 사용 가능
    // =========================================================

    public List<Vector3> BuildStepPath(
        int fromIndex,
        SpringTrainDirection direction
    )
    {
        List<Vector3> result =
            new List<Vector3>();


        if (SectionCount == 0)
        {
            return result;
        }


        fromIndex =
            WrapIndex(
                fromIndex
            );


        if (
            sections[fromIndex] == null
        )
        {
            return result;
        }


        if (
            direction ==
            SpringTrainDirection.Clockwise
        )
        {
            RailSection current =
                sections[fromIndex];


            int nextIndex =
                GetNextIndex(
                    fromIndex,
                    SpringTrainDirection.Clockwise
                );


            if (
                current.clockwisePathPoints !=
                null
            )
            {
                foreach (
                    Transform point
                    in current.clockwisePathPoints
                )
                {
                    if (point != null)
                    {
                        result.Add(
                            point.position
                        );
                    }
                }
            }


            if (
                sections[nextIndex] != null &&
                sections[nextIndex].centerPoint != null
            )
            {
                result.Add(
                    sections[nextIndex]
                        .centerPoint
                        .position
                );
            }
        }
        else
        {
            int previousIndex =
                GetNextIndex(
                    fromIndex,
                    SpringTrainDirection.CounterClockwise
                );


            if (
                sections[previousIndex] ==
                null
            )
            {
                return result;
            }


            RailSection previous =
                sections[
                    previousIndex
                ];


            if (
                previous.clockwisePathPoints !=
                null
            )
            {
                for (
                    int i =
                        previous.clockwisePathPoints.Length - 1;
                    i >= 0;
                    i--
                )
                {
                    Transform point =
                        previous.clockwisePathPoints[i];


                    if (point != null)
                    {
                        result.Add(
                            point.position
                        );
                    }
                }
            }


            if (
                previous.centerPoint != null
            )
            {
                result.Add(
                    previous
                        .centerPoint
                        .position
                );
            }
        }


        return result;
    }


    // =========================================================
    // Circular Index
    // =========================================================

    private int WrapIndex(
        int index
    )
    {
        if (SectionCount <= 0)
        {
            return -1;
        }


        while (index < 0)
        {
            index +=
                SectionCount;
        }


        while (
            index >= SectionCount
        )
        {
            index -=
                SectionCount;
        }


        return index;
    }
}