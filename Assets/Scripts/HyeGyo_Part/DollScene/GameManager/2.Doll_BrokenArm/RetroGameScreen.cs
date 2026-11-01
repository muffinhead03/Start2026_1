using System.Collections;
using UnityEngine;

/// <summary>
/// 레트로 게임기 화면 Material 전환.
///
/// Off      : 기본 상태 (Mat_Retrogame_screen 0)
/// NoCoin   : 동전 없이 상호작용 → 잠깐 켜졌다가 꺼짐 (Mat_Retrogame_screen 1)
/// Playing  : 동전 투입 완료 → 계속 켜져 있음 (Mat_Retrogame_screen 2)
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
    }


    // ============================================
    // 동전 투입 완료 → 계속 켜짐
    // ============================================

    public void ShowPlayingScreen()
    {
        StopNoCoinRoutine();

        state = ScreenState.Playing;
        ApplyMaterial(playingMaterial);
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
