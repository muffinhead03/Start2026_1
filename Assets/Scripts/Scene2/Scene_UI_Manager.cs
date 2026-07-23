using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;

public class Scene_UI_Manager : MonoBehaviour
{
    [Header("판넬")]
    public GameObject[] panels;

    [Header("커서 판넬")]
    public GameObject cursor;

    [Header("커서 이미지")]
    public Sprite[] cursor_imgs;

    [Header("텍스트")]
    public TextMeshProUGUI[] texts;

    bool cursor_id;

    void Start()
    {
        cursor_id = false;
    }

    public void SetActivePanel(int id, bool active)
    {
        panels[id].SetActive(active);
    }

    public void SetActiveCursor(bool active)
    {
        cursor.SetActive(active);
    }

    public void SwitchCursor()
    {
        cursor_id = !cursor_id;
        if (cursor_id) cursor.GetComponent<Image>().sprite = cursor_imgs[1];
        else cursor.GetComponent<Image>().sprite = cursor_imgs[0];
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
}
