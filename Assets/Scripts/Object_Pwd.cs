using UnityEngine;
using UnityEngine.Analytics;

public class Object_Pwd : MonoBehaviour
{
    public string pwd;
    public Object_Pwd_Locked obj;

    private string input="";

    public void PressButton(string n)
    {
        Debug.Log(n);
        if (n == "submit")
        {
            if (pwd == input) obj.UnlockDoor();
            else input = "";
        }
        else input += n;
    }
}
