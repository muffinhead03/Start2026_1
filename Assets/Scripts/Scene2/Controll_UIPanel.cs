using System;
using UnityEngine;

[Serializable]
public struct TextUI
{
    [TextArea(3, 8)] public string disc;
    public int id;
}

public class Controll_UIPanel: MonoBehaviour
{
    [SerializeField] private Scene_UI_Manager SceneUI;

    [Header("ÅØ½ºÆ®")]
    [SerializeField] private TextUI[] text_list;

    [Header("ÆÇ³Ú")]
    [SerializeField] private int[] panel_list;

    public void ShowPanel()
    {
        for(int i = 0; i < text_list.Length; i++)
        {
            SceneUI.ChangeText(text_list[i].id, text_list[i].disc);
            SceneUI.ShowText(text_list[i].id);
        }

        for(int i = 0; i < panel_list.Length; i++)
        {
            SceneUI.SetActivePanel(panel_list[i], true);
        }
    }

    public void HidePanel()
    {
        for (int i = 0; i < text_list.Length; i++)
        {
            SceneUI.HideText(text_list[i].id);
        }

        for (int i = 0; i < panel_list.Length; i++)
        {
            SceneUI.SetActivePanel(panel_list[i], false);
        }
    }
}
