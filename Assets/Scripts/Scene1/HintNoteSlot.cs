using UnityEngine;

public class HintNoteSlot : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;

    [Header("이 종이의 HintPaperNotesManager")]
    public HintPaperNotesManager manager;

    [Header("힌트 매니저 연결 (오답 시 실패 카운트용, 선택)")]
    public HintManager hintManager;

    [Header("호버 미리보기용 Quad (자식으로 배치, 평소 비활성)")]
    public MeshRenderer previewMesh;

    [Header("커서 전환용 UI 매니저")]
    public Scene_UI_Manager SceneUI;

    bool filled = false;

    void Start()
    {
        if (previewMesh != null)
            previewMesh.enabled = false;
    }

    public void OnHoverEnter()
    {
        Debug.Log($"[HintNoteSlot] {gameObject.name} OnHoverEnter 호출됨, filled={filled}, previewMesh={(previewMesh != null ? "있음" : "NULL")}");

        if (filled) return;

        bool holding = manager.IsHoldingValidNote();
        Debug.Log($"[HintNoteSlot] {gameObject.name} IsHoldingValidNote={holding}");

        if (holding)
        {
            if (previewMesh != null) previewMesh.enabled = true;
            SceneUI?.SwitchCursor(true);
        }
    }

    public void OnHoverExit()
    {
        if (previewMesh != null) previewMesh.enabled = false;
        SceneUI?.SwitchCursor(false);
    }

    public void OnInteract()
    {
        if (filled) return;

        bool success = manager.TryPlaceNote(transform);
        Debug.Log($"[HintNoteSlot] {gameObject.name} OnInteract 호출됨, success={success}");

        if (success)
        {
            filled = true;
            if (previewMesh != null) previewMesh.enabled = false;
        }
        else
        {
            var grab = player.GetComponent<Player_Grab>();
            if (grab.isGrab())
                hintManager?.RegisterFail();
        }
    }
}