using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
            if (Regex.IsMatch(s, key))
            {
                KeyNames.Remove(s);
                return true;
            }
        }
        return false;
    }

    public static void AddItem(string item)
    {
        KeyNames.Add(item);
    }

    public static void RemoveItem(string item)
    {
        KeyNames.Remove(item);
    }

}
