using UnityEngine;
using TMPro;

// 사용법:
// 1. 얼룩 페어 중점 오브젝트 4개(Cross_Red 등)에 이 스크립트 추가
// 2. letter에 이 중점의 알파벳 입력 (예: 'S')
// 3. Event_On_Ray OnClick → Inspect() 연결
// 4. notePickup에 이 교차점 근처에 미리 배치해둔 쪽지 오브젝트(비활성 상태) 연결
//    쪽지 오브젝트에는 Object_Grabbable + Event_On_Ray(OnClick → OnGrab) 필요
// 5. grabbableLayer에 쪽지가 속한 레이어(Grabbable) 지정
public class StainIntersection : MonoBehaviour
{
    [Header("이 중점의 알파벳")]
    public char letter;

    [Header("UI 텍스트")]
    public TextMeshProUGUI infoText;

    [Header("연결")]
    public HintManager hintManager;
    public FloorStainManager stainManager;

    [Header("쪽지 수집")]
    [Tooltip("조사 시 활성화될 쪽지 오브젝트. 처음엔 비활성 상태로 배치해두세요.")]
    public GameObject notePickup;

    [Header("쪽지 겹침 방지")]
    [Tooltip("이 반경 안에 쪽지(아무 쪽지나)가 놓여 있으면 내 콜라이더를 잠깐 꺼서 쪽지를 주울 수 있게 함")]
    public float checkRadius = 0.15f;
    public LayerMask grabbableLayer;

    bool inspected = false;
    Collider myCollider;

    void Start()
    {
        myCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (myCollider == null) return;

        // 교차점 위에 쪽지가 겹쳐 있으면 그동안만 콜라이더를 꺼서
        // E키 레이가 쪽지에 먼저 닿게 함 (다른 교차점의 쪽지도 포함)
        bool somethingOnTop = Physics.CheckSphere(
            transform.position + transform.up * 0.02f,
            checkRadius,
            grabbableLayer,
            QueryTriggerInteraction.Ignore
        );

        myCollider.enabled = !somethingOnTop;
    }

    public void Inspect()
    {
        // 글자는 조사할 때마다 다시 보여줌 (힌트 출처 확인용)
        if (infoText != null)
            infoText.text = $"알파벳: {letter}";

        if (inspected) return;
        inspected = true;

        // 쪽지는 첫 조사 때 한 번만 활성화 (인벤에 넣은 뒤 다시 조사해도 안 나타나게)
        if (notePickup != null)
            notePickup.SetActive(true);

        hintManager?.AddLastAction("inspect_alphabet_marker_" + letter);
        stainManager?.OnMidpointInspected(gameObject);
    }

    public void Hide()
    {
        if (infoText != null) infoText.text = "";
    }
}