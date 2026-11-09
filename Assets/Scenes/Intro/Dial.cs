using UnityEngine;
using System;

[Serializable]
public struct DialScript
{
    public string talker;
    [TextArea(3, 8)]
    public string text;
}

[CreateAssetMenu(fileName = "Dial", menuName = "Scriptable Objects/Dial")]
public class Dial : ScriptableObject
{
    public DialScript[] scripts;
}
