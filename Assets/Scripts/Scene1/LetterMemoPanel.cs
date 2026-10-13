using TMPro;
using UnityEngine;

/// <summary>
/// 책장 옆 메모 종이 — 플레이어가 알아낸 알파벳 4개를 기록하는 UI.
///
/// 흐름: 종이 상호작용 → Object_FixCamera.Fixed → Open()
///       → 상/하 버튼으로 글자 변경 → E → Object_FixCamera.UnFixed → Close()
///       → 닫을 때 종이 위 TMP 글자에 반영 (기획서: 판넬을 끄면 실제 종이도 바뀜)
///
/// 힌트 시스템(HintManager)은 참조하지 않음.
/// WineHintBridge가 CurrentLetters / CorrectCount / IsSolved를 폴링해서 스텝 완료·진행 힌트를 처리함.
/// 브리지가 읽는 값은 "닫을 때 확정된 글자" 기준 (패널 조작 중인 값은 아님).
///
/// 이 컴포넌트는 종이 오브젝트에 붙이는 걸 권장하지만, 판넬 자기 자신에 붙여도 동작하도록 되어 있음
/// (판넬이 꺼진 채 시작하면 Awake가 Open 이후에 늦게 불리는데, 그때 판넬을 다시 끄지 않게 처리함).
/// </summary>
public class LetterMemoPanel : MonoBehaviour
{
    const int SlotCount = 4;

    [Header("정답 (칸 순서 = 노랑, 파랑, 빨강, 초록)")]
    [SerializeField] string answer = "BTHY";

    [Header("UI 판넬 (Canvas 아래)")]
    [SerializeField] GameObject panel;
    [Tooltip("판넬 안의 글자 4칸 (왼쪽부터)")]
    [SerializeField] TMP_Text[] panelLetters = new TMP_Text[SlotCount];

    [Header("실제 종이 위 글자 (3D TextMeshPro)")]
    [SerializeField] TMP_Text paperText;
    [Tooltip("종이에 글자 사이 띄어쓰기를 넣을지 (A A A A)")]
    [SerializeField] bool spacedOnPaper = true;

    [Header("커서")]
    [SerializeField] Scene_UI_Manager sceneUI;

    [Header("디버그")]
    [SerializeField] bool verboseLog = true;

    // 판넬에서 조작 중인 글자
    char[] editing;
    // 판넬을 닫으며 확정된 글자 (브리지가 읽는 값)
    char[] committed;

    bool initialized;
    bool isOpen;

    /// <summary>닫을 때 확정된 글자 4개 (예: "BTAA")</summary>
    public string CurrentLetters
    {
        get { EnsureInit(); return new string(committed); }
    }

    /// <summary>정답과 같은 자리에 같은 글자가 들어간 칸 수 (0~4)</summary>
    public int CorrectCount
    {
        get
        {
            EnsureInit();
            int count = 0;
            for (int i = 0; i < SlotCount && i < answer.Length; i++)
                if (committed[i] == answer[i]) count++;
            return count;
        }
    }

    /// <summary>확정된 글자가 정답과 완전히 같은지</summary>
    public bool IsSolved => CorrectCount == SlotCount;

    public bool IsOpen => isOpen;

    void Awake()
    {
        EnsureInit();

        // Open()으로 이미 열린 뒤에 Awake가 늦게 불린 경우(판넬 자신에 붙인 경우)엔 끄지 않음
        if (!isOpen && panel != null) panel.SetActive(false);

        RefreshPaper();
    }

    // 오브젝트가 꺼져 있어서 Awake보다 Open()이 먼저 불려도 안전하게 초기화
    void EnsureInit()
    {
        if (initialized) return;
        initialized = true;

        editing = new char[SlotCount];
        committed = new char[SlotCount];
        for (int i = 0; i < SlotCount; i++)
        {
            editing[i] = 'A';
            committed[i] = 'A';
        }
    }

    // ───────── Object_FixCamera 이벤트에 연결 ─────────

    /// <summary>Object_FixCamera.Fixed에 연결</summary>
    public void Open()
    {
        EnsureInit();
        if (verboseLog) Debug.Log($"[LetterMemoPanel] Open 호출 (이미 열림: {isOpen})", this);
        if (isOpen) return;
        isOpen = true;

        // 마지막으로 확정한 글자에서 이어서 수정
        System.Array.Copy(committed, editing, SlotCount);

        if (panel == null)
        {
            Debug.LogWarning("[LetterMemoPanel] Panel 칸이 비어 있어서 판넬을 켤 수 없음", this);
        }
        else
        {
            panel.SetActive(true);
            if (verboseLog && !panel.activeInHierarchy)
                Debug.LogWarning($"[LetterMemoPanel] 판넬은 켰지만 부모가 꺼져 있어서 화면에 안 보임: {GetPath(panel.transform)}", panel);
        }

        RefreshPanel();

        // Player_FixCamera에서는 커서 해제가 주석 처리돼 있어서 여기서 직접 처리
        if (sceneUI != null) sceneUI.UnlockPointer();
        else Debug.LogWarning("[LetterMemoPanel] Scene UI 칸이 비어 있어서 마우스 커서를 풀 수 없음", this);
    }

    /// <summary>Object_FixCamera.UnFixed에 연결</summary>
    public void Close()
    {
        EnsureInit();
        if (!isOpen) return;
        isOpen = false;

        System.Array.Copy(editing, committed, SlotCount);
        RefreshPaper();

        if (panel != null) panel.SetActive(false);
        if (sceneUI != null) sceneUI.LockPointer();

        Debug.Log($"[LetterMemoPanel] 기록 확정: {CurrentLetters} (정답 {CorrectCount}/{SlotCount})", this);
    }

    // ───────── 버튼 OnClick에 연결 (인자로 칸 번호 0~3) ─────────

    public void Up(int slot) => Shift(slot, +1);
    public void Down(int slot) => Shift(slot, -1);

    void Shift(int slot, int delta)
    {
        EnsureInit();
        if (!isOpen || slot < 0 || slot >= SlotCount) return;

        int index = editing[slot] - 'A';
        index = (index + delta + 26) % 26; // Z 다음은 A, A 이전은 Z
        editing[slot] = (char)('A' + index);

        RefreshPanel();
    }

    // ───────── 표시 갱신 ─────────

    void RefreshPanel()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            if (i < panelLetters.Length && panelLetters[i] != null)
                panelLetters[i].text = editing[i].ToString();
        }
    }

    void RefreshPaper()
    {
        if (paperText == null) return;
        string letters = new string(committed);
        paperText.text = spacedOnPaper ? string.Join(" ", letters.ToCharArray()) : letters;
    }

    static string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null) { t = t.parent; path = t.name + "/" + path; }
        return path;
    }
}