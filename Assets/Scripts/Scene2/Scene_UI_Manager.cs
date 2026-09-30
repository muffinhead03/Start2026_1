using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;

public class Scene_UI_Manager : MonoBehaviour
{
    [Header("판넬")]
    [SerializeField]
    GameObject[] panels;

    [Header("버튼")]
    public GameObject[] buttons;

    [Header("슬라이더")]
    public Slider[] sliders;

    [Header("커서")]
    [SerializeField] GameObject panel_cursor;
    [SerializeField] GameObject panel_e;
    [SerializeField] GameObject cursor;
    [SerializeField] Sprite[] cursor_imgs;

    [Header("텍스트")]
    public TextMeshProUGUI[] texts;

    [Header("포인터 잠금")]
    public bool pointerInitState;


    //bool cursor_id;

    private void OnEnable()
    {
        if(pointerInitState) LockPointer();
    }

    public void SetActivePanel(int id, bool active)
    {
        panels[id].SetActive(active);
    }

    public void SetActiveButton(int id, bool active)
    {
        buttons[id].SetActive(active);
    }

    public void SetActiveCursor(bool active)
    {
        if(panel_cursor!=null) panel_cursor.SetActive(active);
    }

    public void SwitchCursor(bool on)
    {
        if (on)
        {
            cursor.GetComponent<Image>().sprite = cursor_imgs[1];
        }
        else
        {
            cursor.GetComponent<Image>().sprite = cursor_imgs[0];
        }

        panel_e?.SetActive(on);
    }

    public void LockPointer()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockPointer()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ChangeText(int id, string text)
    {
        texts[id].text = text;
    }

    public void SetSlider(int id, float value)
    {
        sliders[id].value = value;
    }
}
