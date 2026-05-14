using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Player_Inventory : MonoBehaviour
{
    private static List<string> KeyNames;

    void Start()
    {
        KeyNames= new List<string>();
    }

    public static bool hasKey(string key)
    {
        foreach (string s in KeyNames)
        {
            if (s == key)
            {
                KeyNames.Remove(key);
                return true;
            }
        }
        return false;
    }

    public static void AddItem(string item)
    {
        KeyNames.Add(item);
    }
}
