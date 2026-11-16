using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "HintTreeNode", menuName = "Scriptable Objects/HintTreeNode")]
public class HintTreeNode : ScriptableObject
{
    public int hint_id;
    public int param_id;

    public HintTreeNode left;
    public HintTreeNode right;
}

public class HintTree
{
    HintTreeNode root;
    private List<bool> cond;

    public HintTree(HintTreeNode node, int cond_len)
    {
        root = node;

        cond = new List<bool>();
        for (int i = 0; i < cond_len; i++) cond.Add(false);

        FindHintID();
    }

    public void SetCond(int id, bool isTrue)
    {
        if (cond[id] != isTrue)
        {
            cond[id] = isTrue;

            FindHintID();
        }
    }

    public int FindHintID()
    {
        HintTreeNode current = root;

        while (current.hint_id == -1)
        {
            if (cond[current.param_id]) current = current.right;
            else current = current.left;
        }

        Debug.Log(current.hint_id);
        return current.hint_id+1;
    }
}