using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


public class DollPoseMoveManager : MonoBehaviour
{
    // =========================================================
    // 관절 Pose 데이터
    // =========================================================

    [System.Serializable]
    public class DollPartPose
    {
        [Header("관절")]
        public Transform target;


        [Header("Initial Pose")]
        public Vector3 initialLocalPosition;
        public Vector3 initialLocalEulerAngles;


        [Header("Final Pose")]
        public Vector3 finalLocalPosition;
        public Vector3 finalLocalEulerAngles;
    }


    // =========================================================
    // 인형 관절
    // =========================================================

    [Header("인형 관절")]
    [SerializeField]
    private DollPartPose[] dollParts;


    // =========================================================
    // 이동 설정
    // =========================================================

    [Header("이동 설정")]

    [Tooltip("Final Pose까지 이동하는 시간")]
    [SerializeField]
    private float moveDuration = 2f;


    // =========================================================
    // 시작 설정
    // =========================================================

    [Header("시작 설정")]

    [Tooltip("Play 시작 시 저장된 Initial Pose를 자동 적용")]
    [SerializeField]
    private bool applyInitialPoseOnStart = true;


    [Header("테스트")]

    [Tooltip("테스트용. Play 시작 후 자동으로 Final Pose로 이동")]
    [SerializeField]
    private bool testMoveToFinalOnStart = false;

    [Tooltip("테스트용 Final 이동 시작 대기시간")]
    [SerializeField]
    private float testDelay = 1f;


    // =========================================================
    // State
    // =========================================================

    private bool isMoving = false;

    public bool IsMoving => isMoving;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        // ---------------------------------------------
        // 게임 시작 시 무조건 Initial Pose 적용
        // ---------------------------------------------

        if (applyInitialPoseOnStart)
        {
            ApplyInitialPose();

            Debug.Log(
                "[DollPose] 게임 시작 → Initial Pose 적용",
                this
            );
        }


        // ---------------------------------------------
        // 테스트용
        // ---------------------------------------------

        if (testMoveToFinalOnStart)
        {
            StartCoroutine(
                TestMoveCoroutine()
            );
        }
    }


    // =========================================================
    // 현재 자세 → Initial 저장
    // =========================================================

    [ContextMenu("1. 현재 자세를 Initial Pose로 저장")]
    public void SaveCurrentPoseAsInitial()
    {
        if (dollParts == null)
            return;


        foreach (DollPartPose part in dollParts)
        {
            if (part == null ||
                part.target == null)
            {
                continue;
            }


            part.initialLocalPosition =
                part.target.localPosition;


            part.initialLocalEulerAngles =
                part.target.localEulerAngles;
        }


        SaveEditorChanges();


        Debug.Log(
            "[DollPose] 현재 자세를 Initial Pose로 저장 완료",
            this
        );
    }


    // =========================================================
    // 현재 자세 → Final 저장
    // =========================================================

    [ContextMenu("2. 현재 자세를 Final Pose로 저장")]
    public void SaveCurrentPoseAsFinal()
    {
        if (dollParts == null)
            return;


        foreach (DollPartPose part in dollParts)
        {
            if (part == null ||
                part.target == null)
            {
                continue;
            }


            part.finalLocalPosition =
                part.target.localPosition;


            part.finalLocalEulerAngles =
                part.target.localEulerAngles;
        }


        SaveEditorChanges();


        Debug.Log(
            "[DollPose] 현재 자세를 Final Pose로 저장 완료",
            this
        );
    }


    // =========================================================
    // Initial Pose 즉시 적용
    // =========================================================

    [ContextMenu("3. Initial Pose로 즉시 이동")]
    public void ApplyInitialPose()
    {
        if (dollParts == null)
            return;


        foreach (DollPartPose part in dollParts)
        {
            if (part == null ||
                part.target == null)
            {
                continue;
            }


            part.target.localPosition =
                part.initialLocalPosition;


            part.target.localRotation =
                Quaternion.Euler(
                    part.initialLocalEulerAngles
                );
        }


#if UNITY_EDITOR

        if (!Application.isPlaying)
        {
            EditorSceneManager.MarkSceneDirty(
                gameObject.scene
            );
        }

#endif


        Debug.Log(
            "[DollPose] Initial Pose 적용 완료",
            this
        );
    }


    // =========================================================
    // Final Pose 즉시 적용
    // =========================================================

    [ContextMenu("4. Final Pose 미리보기")]
    public void ApplyFinalPoseImmediately()
    {
        if (dollParts == null)
            return;


        foreach (DollPartPose part in dollParts)
        {
            if (part == null ||
                part.target == null)
            {
                continue;
            }


            part.target.localPosition =
                part.finalLocalPosition;


            part.target.localRotation =
                Quaternion.Euler(
                    part.finalLocalEulerAngles
                );
        }


#if UNITY_EDITOR

        if (!Application.isPlaying)
        {
            EditorSceneManager.MarkSceneDirty(
                gameObject.scene
            );
        }

#endif


        Debug.Log(
            "[DollPose] Final Pose 즉시 적용 완료",
            this
        );
    }


    // =========================================================
    // 게임용
    // Final Pose로 천천히 이동
    // =========================================================

    public void MoveToFinalPose()
    {
        if (isMoving)
            return;


        StartCoroutine(
            MoveToFinalPoseCoroutine()
        );
    }


    // =========================================================
    // 테스트
    // =========================================================

    private IEnumerator TestMoveCoroutine()
    {
        yield return new WaitForSeconds(
            testDelay
        );


        MoveToFinalPose();
    }


    // =========================================================
    // Final Pose 이동 Coroutine
    // =========================================================

    private IEnumerator MoveToFinalPoseCoroutine()
    {
        if (dollParts == null ||
            dollParts.Length == 0)
        {
            yield break;
        }


        isMoving = true;


        Vector3[] startPositions =
            new Vector3[dollParts.Length];


        Quaternion[] startRotations =
            new Quaternion[dollParts.Length];


        // ---------------------------------------------
        // 현재 Pose 저장
        // ---------------------------------------------

        for (int i = 0;
             i < dollParts.Length;
             i++)
        {
            DollPartPose part =
                dollParts[i];


            if (part == null ||
                part.target == null)
            {
                continue;
            }


            startPositions[i] =
                part.target.localPosition;


            startRotations[i] =
                part.target.localRotation;
        }


        // ---------------------------------------------
        // Final Pose로 이동
        // ---------------------------------------------

        float elapsed = 0f;


        while (elapsed < moveDuration)
        {
            elapsed +=
                Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    elapsed /
                    Mathf.Max(
                        moveDuration,
                        0.0001f
                    )
                );


            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );


            for (int i = 0;
                 i < dollParts.Length;
                 i++)
            {
                DollPartPose part =
                    dollParts[i];


                if (part == null ||
                    part.target == null)
                {
                    continue;
                }


                // Position
                part.target.localPosition =
                    Vector3.Lerp(
                        startPositions[i],
                        part.finalLocalPosition,
                        smoothT
                    );


                // Rotation
                Quaternion finalRotation =
                    Quaternion.Euler(
                        part.finalLocalEulerAngles
                    );


                part.target.localRotation =
                    Quaternion.Slerp(
                        startRotations[i],
                        finalRotation,
                        smoothT
                    );
            }


            yield return null;
        }


        // ---------------------------------------------
        // 마지막 정확한 보정
        // ---------------------------------------------

        foreach (DollPartPose part in dollParts)
        {
            if (part == null ||
                part.target == null)
            {
                continue;
            }


            part.target.localPosition =
                part.finalLocalPosition;


            part.target.localRotation =
                Quaternion.Euler(
                    part.finalLocalEulerAngles
                );
        }


        isMoving = false;


        Debug.Log(
            "[DollPose] Final Pose 이동 완료",
            this
        );
    }


    // =========================================================
    // Editor 저장 처리
    // =========================================================

    private void SaveEditorChanges()
    {
#if UNITY_EDITOR

        if (Application.isPlaying)
            return;


        EditorUtility.SetDirty(
            this
        );


        if (gameObject.scene.IsValid())
        {
            EditorSceneManager.MarkSceneDirty(
                gameObject.scene
            );
        }

#endif
    }
}