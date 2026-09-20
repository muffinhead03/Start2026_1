using UnityEngine;
using UnityEngine.SceneManagement;

public class Doll_SceneChangeTrigger : MonoBehaviour
{
    // ================================================
    // Scene
    // ================================================

    [Header("Scene Change")]

    [Tooltip("이동할 Scene 이름을 정확하게 입력")]
    [SerializeField]
    private string targetSceneName;


    // ================================================
    // Player
    // ================================================

    [Header("Player")]

    [Tooltip("Player 오브젝트의 Tag")]
    [SerializeField]
    private string playerTag = "Player";


    // ================================================
    // State
    // ================================================

    private bool isChangingScene = false;


    // ================================================
    // Trigger Enter
    // ================================================

    private void OnTriggerEnter(Collider other)
    {
        // 이미 Scene 이동 중이면 중복 실행 방지
        if (isChangingScene)
        {
            return;
        }


        // Player가 아니면 무시
        if (!other.CompareTag(playerTag))
        {
            return;
        }


        // Scene 이름 확인
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning(
                "[Doll_SceneChangeTrigger] " +
                "Target Scene Name이 비어있습니다.",
                this
            );

            return;
        }


        // Scene List에 등록되어 있는지 확인
        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.LogError(
                "[Doll_SceneChangeTrigger] " +
                $"Scene '{targetSceneName}'을 찾을 수 없습니다. " +
                "Build Profiles의 Scene List를 확인하세요.",
                this
            );

            return;
        }


        isChangingScene = true;


        Debug.Log(
            "[Doll_SceneChangeTrigger] " +
            $"Player 진입 → Scene 이동 : {targetSceneName}",
            this
        );


        SceneManager.LoadScene(targetSceneName);
    }
}