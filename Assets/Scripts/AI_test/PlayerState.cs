using System.Collections.Generic;

[System.Serializable]
public class PlayerState
{
    public float staySeconds;
    public int hintCount;
    public int failCount;
    public string hintType; // "direct" or "indirect"
    public List<int> completedSteps = new List<int>();
    public List<string> foundClues = new List<string>();
    public List<RepeatedInspection> repeatedInspections = new List<RepeatedInspection>();
}

[System.Serializable]
public class RepeatedInspection
{
    public string objectName;
    public int count;
}
