using UnityEngine;

public class DollScene_Spring : MonoBehaviour
{
    [Header("Doll Scene Game Manager")]
    [SerializeField] private DollScene_GameManager gameManager;


    [Header("Rail Objects")]
    [SerializeField] private Transform redRail;
    [SerializeField] private Transform yellowRail;
    [SerializeField] private Transform greenRail;


    [Header("현재 회전 횟수")]
    [SerializeField] private int redRotationIndex = 0;
    [SerializeField] private int yellowRotationIndex = 0;
    [SerializeField] private int greenRotationIndex = 0;


    [Header("정답 회전 횟수")]
    [SerializeField] private int correctRedIndex = 0;
    [SerializeField] private int correctYellowIndex = 0;
    [SerializeField] private int correctGreenIndex = 0;


    [Header("Puzzle State")]
    [SerializeField] private bool isPuzzleSolved = false;


    private const float RotationAngle = 30f;


    private void Start()
    {
        if (gameManager == null)
        {
            gameManager =
                GetComponentInParent<DollScene_GameManager>();
        }
    }


    // ================================
    // 화남 버튼
    // 빨강 선로
    // ================================

    public void PressAngryButton()
    {
        if (isPuzzleSolved)
            return;

        RotateRail(
            redRail,
            ref redRotationIndex
        );

        CheckPuzzle();
    }


    // ================================
    // 기쁨 버튼
    // 노랑 선로
    // ================================

    public void PressHappyButton()
    {
        if (isPuzzleSolved)
            return;

        RotateRail(
            yellowRail,
            ref yellowRotationIndex
        );

        CheckPuzzle();
    }


    // ================================
    // 슬픔 버튼
    // 초록 선로
    // ================================

    public void PressSadButton()
    {
        if (isPuzzleSolved)
            return;

        RotateRail(
            greenRail,
            ref greenRotationIndex
        );

        CheckPuzzle();
    }


    // ================================
    // 선로 회전
    // ================================

    private void RotateRail(
        Transform rail,
        ref int rotationIndex)
    {
        if (rail == null)
            return;

        rotationIndex++;

        if (rotationIndex >= 12)
            rotationIndex = 0;

        rail.Rotate(
            0f,
            RotationAngle,
            0f
        );
    }


    // ================================
    // 퍼즐 정답 확인
    // ================================

    private void CheckPuzzle()
    {
        if (redRotationIndex != correctRedIndex)
            return;

        if (yellowRotationIndex != correctYellowIndex)
            return;

        if (greenRotationIndex != correctGreenIndex)
            return;


        SolvePuzzle();
    }


    // ================================
    // 퍼즐 완료
    // ================================

    private void SolvePuzzle()
    {
        if (isPuzzleSolved)
            return;

        isPuzzleSolved = true;

        Debug.Log("[Spring Puzzle] Rail Complete");


        /*
         * TODO
         *
         * 색깔 기차 이동
         *
         * ↓
         *
         * 검은 기차를 밀어냄
         *
         * ↓
         *
         * 태엽 오브젝트 노출
         */
    }


    // ================================
    // 태엽 획득 완료
    // ================================

    public void CompleteFindSpring()
    {
        /*
         * 실제 태엽이 플레이어 인벤토리에
         * 들어간 시점에 호출 예정
         */

        if (gameManager != null)
        {
            gameManager.CompleteFindSpring();
        }
    }
}