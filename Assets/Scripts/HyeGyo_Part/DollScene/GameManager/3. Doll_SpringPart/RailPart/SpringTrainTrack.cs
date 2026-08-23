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
            "현재 선로 중심 → 다음 선로 중심 사이에 있는 보조 WayPoint입니다. " +
            "직선은 비워도 되고, 곡선은 2~4개 정도 배치하면 자연스럽습니다."
        )]
        public Transform[] clockwisePathPoints;
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
    // 기차 중심점 바로 아래에 있는 선로 찾기
    // =========================================================

    public int GetNearestSectionIndex(
        Vector3 trainCenterPosition
    )
    {
        if (sections == null ||
            sections.Length == 0)
        {
            return -1;
        }


        int nearestIndex =
            -1;

        float nearestDistance =
            float.MaxValue;


        for (int i = 0; i < sections.Length; i++)
        {
            if (sections[i] == null ||
                sections[i].centerPoint == null)
            {
                continue;
            }


            float distance =
                Vector3.SqrMagnitude(
                    trainCenterPosition -
                    sections[i].centerPoint.position
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
            direction == SpringTrainDirection.Clockwise
                ? 1
                : -1;


        return WrapIndex(
            currentIndex + offset
        );
    }


    // =========================================================
    // 한 칸 이동 경로 생성
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


        if (direction ==
            SpringTrainDirection.Clockwise)
        {
            RailSection current =
                sections[fromIndex];


            int nextIndex =
                GetNextIndex(
                    fromIndex,
                    SpringTrainDirection.Clockwise
                );


            // 현재 칸 → 다음 칸 사이
            // 곡선 WayPoint
            if (current.clockwisePathPoints != null)
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


            if (sections[nextIndex].centerPoint != null)
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
            // 반시계 방향은
            // 이전 선로의 Clockwise 경로를
            // 역순으로 사용
            int previousIndex =
                GetNextIndex(
                    fromIndex,
                    SpringTrainDirection.CounterClockwise
                );


            RailSection previous =
                sections[previousIndex];


            if (previous.clockwisePathPoints != null)
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


            if (previous.centerPoint != null)
            {
                result.Add(
                    previous.centerPoint.position
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


        while (index >= SectionCount)
        {
            index -=
                SectionCount;
        }


        return index;
    }
}