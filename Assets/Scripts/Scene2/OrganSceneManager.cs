using UnityEngine;

public class OrganSceneManager : MonoBehaviour
{
    [SerializeField] GameObject[] pipes;
    [SerializeField] GameObject[] papers;
    [SerializeField] GameObject[] clues;
    [SerializeField] GameObject[] puzzle_obj;
    [SerializeField] HintTreeNode hint_root;

    private HintTree tree;
    
    public event System.Action<int> OnStepCompleted;

    public GameObject[] Pipes => pipes;
    public GameObject[] Papers => papers;
    public GameObject[] Clues => clues;
    public GameObject[] PuzzleObjects => puzzle_obj;

    private bool[] pipe = new bool[4];
    private bool[] lp = new bool[5];
    private bool[] paper = new bool[4];
    private bool[] musicbox = new bool[2];
    private int current_lp;

    private void Start()
    {
        InitHintTree();
    }

    void InitHintTree()
    {
        tree = new HintTree(hint_root, 50);
    }

    public int GetCurrentStep()
    {
        return tree.FindHintID();
    }

    public void PlayLP()
    {
        current_lp++;

        if (current_lp >= lp.Length) return;

        if (lp[current_lp]) tree.SetCond(12, true);
        else
        {
            if (current_lp == 3 || current_lp == 4) tree.SetCond(13, true);
            else tree.SetCond(13, false);

            tree.SetCond(12, false);
        }
    }

    public void CompleteStep(int step)
    {
        OnStepCompleted?.Invoke(step);

        tree.SetCond(step, true);
    }

    public void FindMusicbox(int id)
    {
        if (musicbox[id]) return;

        musicbox[id] = true;

        if (id == 0) tree.SetCond(8, true);
        if (musicbox[0] && musicbox[1]) tree.SetCond(7, true);
    }

    public void FindPipe(int id)
    {
        id--;
        if (pipe[id]) return;

        pipe[id] = true;

        tree.SetCond(3, true);

        bool findAllpipe = true;
        for(int i=0;i<pipe.Length;i++)
        {
            if (!pipe[i]) findAllpipe = false;
        }

        tree.SetCond(2, findAllpipe);

        if ((id == 0 || id == 1)&& pipe[0] && pipe[1]) tree.SetCond(5, true);

        if (id == 2 && pipe[2]) tree.SetCond(9, true);
    }

    public void FindLP(int id)
    {
        id--;
        if (lp[id]) return;

        lp[id] = true;

        tree.SetCond(11, true);

        if(id == current_lp) tree.SetCond(12, true);
    }

    public void FindPaper(int id)
    {
        id--;
        if (paper[id]) return;

        paper[id] = true;

        bool findAllpaper = true;
        for (int i = 0; i < pipe.Length; i++)
        {
            if (!paper[i]) findAllpaper = false;
        }

        tree.SetCond(15, findAllpaper);
    }
}