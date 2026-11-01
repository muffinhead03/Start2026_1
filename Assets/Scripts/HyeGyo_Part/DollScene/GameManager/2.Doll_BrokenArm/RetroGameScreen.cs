using System.Collections;
using UnityEngine;

/// <summary>
/// 레트로 게임기 화면 Material 전환.
///
/// Off      : 기본 상태 (Mat_Retrogame_screen 0)
/// NoCoin   : 동전 없이 상호작용 → 잠깐 켜졌다가 꺼짐 (Mat_Retrogame_screen 1)
/// Playing  : 동전 투입 완료 → playingDuration(3초) 동안 켜짐 (Mat_Retrogame_screen 2)
///            → 꺼지는 타이밍은 RetroGameManager가 관리 (꺼진 직후 인형 팔 배출)
///
/// 화면 오브젝트(retro_game/screen)에 붙여서 사용.
/// 상호작용 잠금 판단은 RetroGameManager가 이 상태를 보고 결정한다.
/// </summary>
public class RetroGameScreen : MonoBehaviour
{
    public enum ScreenState
    {
        Off,
        NoCoin,
        Playing
    }


    // ============================================
    // Renderer
    // ============================================

    [Header("화면 Renderer (비워두면 이 오브젝트에서 자동으로 찾음)")]
    [SerializeField]
    private Renderer screenRenderer;


    // ============================================
    // Material
    // ============================================

    [Header("화면 Material")]
    [Tooltip("기본(꺼진) 화면 : Mat_Retrogame_screen 0")]
    [SerializeField]
    private Material offMaterial;

    [Tooltip("동전 없이 상호작용했을 때 화면 : Mat_Retrogame_screen 1")]
    [SerializeField]
    private Material noCoinMaterial;

    [Tooltip("동전 투입 후 화면 : Mat_Retrogame_screen 2")]
    [SerializeField]
    private Material playingMaterial;


    // ============================================
    // Timing
    // ============================================

    [Header("동전 없이 켜졌을 때 유지 시간(초)")]
    [SerializeField]
    private float noCoinDuration = 3f;


    [Header("동전 투입 후 켜져 있는 시간(초) → 꺼지면 인형 팔 배출")]
    [SerializeField]
    private float playingDuration = 3f;

    public float PlayingDuration => playingDuration;


    // ============================================
    // Camera Focus
    // ============================================

    [Header("화면이 켜져 있는 동안 카메라를 CameraPoint로 이동")]
    [Tooltip("체크하면 화면이 켜져 있는 동안 플레이어 카메라가 CameraPoint로 이동하고,\n화면이 꺼지면 원래 위치로 돌아옴")]
    [SerializeField]
    private bool moveCameraWhileOn = true;

    [Tooltip("retro_game/CameraPoint")]
    [SerializeField]
    private Transform cameraPoint;

    [Tooltip("비워두면 CameraPoint에 Object_FixCamera를 자동으로 추가/설정함\n(Use Camera Point 모드로 사용)")]
    [SerializeField]
    private Object_FixCamera cameraFixer;

    private bool isCameraMoved = false;


    // ============================================
    // State
    // ============================================

    private ScreenState state = ScreenState.Off;

    private Coroutine noCoinRoutine;

    public ScreenState State => state;

    /// 동전 없이 잠깐 켜져 있는 중인지 (이 동안 상호작용 불가)
    public bool IsTemporarilyOn => state == ScreenState.NoCoin;


    // ============================================
    // Awake
    // ============================================

    private void Awake()
    {
        if (screenRenderer == null)
        {
            screenRenderer =
                GetComponent<Renderer>();
        }

        // 시작은 항상 꺼진 화면
        state = ScreenState.Off;
        ApplyMaterial(offMaterial);
    }


    // ============================================
    // 동전 없이 상호작용 → 3초간 켜짐
    // ============================================

    /// <returns>화면을 켰으면 true, 이미 켜져 있어서 무시했으면 false</returns>
    public bool ShowNoCoinScreen()
    {
        if (state != ScreenState.Off)
        {
            return false;
        }

        state = ScreenState.NoCoin;
        ApplyMaterial(noCoinMaterial);

        BeginCameraFocus();

        noCoinRoutine =
            StartCoroutine(
                TurnOffAfterDelay()
            );

        return true;
    }


    private IEnumerator TurnOffAfterDelay()
    {
        yield return new WaitForSeconds(
            noCoinDuration
        );

        noCoinRoutine = null;

        // 그 사이 동전 화면으로 바뀌었으면 건드리지 않음
        if (state != ScreenState.NoCoin)
        {
            yield break;
        }

        state = ScreenState.Off;
        ApplyMaterial(offMaterial);

        EndCameraFocus();
    }


    // ============================================
    // 동전 투입 완료 → 켜짐 (끄는 건 RetroGameManager가 TurnOff로)
    // ============================================

    public void ShowPlayingScreen()
    {
        StopNoCoinRoutine();

        state = ScreenState.Playing;
        ApplyMaterial(playingMaterial);

        BeginCameraFocus();
    }


    // ============================================
    // 화면 끄기 (기본 화면으로)
    // ============================================

    public void TurnOff()
    {
        StopNoCoinRoutine();

        state = ScreenState.Off;
        ApplyMaterial(offMaterial);

        EndCameraFocus();
    }


    // ============================================
    // 중간에 비활성화되면 3초 화면이 켜진 채로 멈추지 않게 복구
    // ============================================

    private void OnDisable()
    {
        StopNoCoinRoutine();

        if (state == ScreenState.NoCoin)
        {
            state = ScreenState.Off;
            ApplyMaterial(offMaterial);
        }

        // 카메라가 CameraPoint에 묶인 채로 남지 않게
        EndCameraFocus();
    }


    // ============================================
    // 카메라 → CameraPoint
    //
    // 팀 공용 Object_FixCamera(Use Camera Point)를 그대로 사용.
    // 이동 중에는 Player_FixCamera가 이동 / 입력을 잠가서
    // 화면이 켜져 있는 동안 상호작용이 불가능해짐.
    // ============================================

    private void BeginCameraFocus()
    {
        if (!moveCameraWhileOn ||
            isCameraMoved)
        {
            return;
        }


        if (!PrepareCameraFixer())
        {
            return;
        }


        // 이미 다른 오브젝트(자물쇠 등)로 카메라가 고정돼 있으면 건드리지 않음
        if (cameraFixer.player.isPlayerFix())
        {
            return;
        }


        cameraFixer.FixCamera();


        // Object_FixCamera는 고정 중 E를 누르면 바로 풀리는 구조.
        // 화면이 켜져 있는 동안은 빠져나가지 못하게 E 처리 대상에서 뺀다.
        InspectInput.End(cameraFixer);


        isCameraMoved = true;
    }


    private void EndCameraFocus()
    {
        if (!isCameraMoved)
        {
            return;
        }


        isCameraMoved = false;


        if (cameraFixer != null)
        {
            cameraFixer.UnFixCamera();
        }
    }


    private bool PrepareCameraFixer()
    {
        if (cameraPoint == null)
        {
            Debug.LogWarning(
                "[RetroGameScreen] Camera Point가 연결되지 않아 카메라 이동을 건너뜁니다.",
                this
            );

            return false;
        }


        if (cameraFixer == null)
        {
            cameraFixer =
                cameraPoint.GetComponent<Object_FixCamera>();
        }


        if (cameraFixer == null)
        {
            cameraFixer =
                cameraPoint.gameObject.AddComponent<Object_FixCamera>();
        }


        cameraFixer.useCameraPoint = true;
        cameraFixer.cameraPoint = cameraPoint;


        if (cameraFixer.player == null)
        {
            cameraFixer.player =
                FindFirstObjectByType<Player_FixCamera>();
        }


        if (cameraFixer.SceneUI == null)
        {
            cameraFixer.SceneUI =
                FindFirstObjectByType<Scene_UI_Manager>();
        }


        if (cameraFixer.player == null ||
            cameraFixer.SceneUI == null)
        {
            Debug.LogWarning(
                "[RetroGameScreen] Player_FixCamera / Scene_UI_Manager를 찾지 못해 카메라 이동을 건너뜁니다.",
                this
            );

            return false;
        }


        return true;
    }


    private void StopNoCoinRoutine()
    {
        if (noCoinRoutine == null)
        {
            return;
        }

        StopCoroutine(noCoinRoutine);
        noCoinRoutine = null;
    }


    private void ApplyMaterial(Material material)
    {
        if (screenRenderer == null)
        {
            Debug.LogWarning(
                "[RetroGameScreen] 화면 Renderer가 없습니다.",
                this
            );

            return;
        }

        if (material == null)
        {
            Debug.LogWarning(
                $"[RetroGameScreen] {state} 상태의 Material이 연결되지 않았습니다.",
                this
            );

            return;
        }

        // sharedMaterial : Material 에셋을 복제하지 않고 슬롯만 교체
        screenRenderer.sharedMaterial = material;
    }
}
