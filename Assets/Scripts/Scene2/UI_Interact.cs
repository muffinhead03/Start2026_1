using TMPro;
using UnityEngine;

public class UI_Interact: MonoBehaviour
{
    [SerializeField]
    private GameObject tmp;

    public void ShowText()
    {
        tmp.SetActive(true);
    }

    public void HideText() 
    {
        tmp.SetActive(false);
    }
}
