using UnityEngine;

public class DollRepairCompletionManager : MonoBehaviour
{
    [Header("Doll State")]
    [SerializeField]
    private DollScene_ChangeDoll changeDoll;


    [Header("Key Drop")]
    [SerializeField]
    private DollKeyDropManager keyDropManager;


    private bool completedSent = false;


    private void Start()
    {
        if (changeDoll == null)
        {
            changeDoll =
                FindFirstObjectByType<DollScene_ChangeDoll>();
        }


        if (keyDropManager == null)
        {
            keyDropManager =
                FindFirstObjectByType<DollKeyDropManager>();
        }
    }


    public void CheckRepairCompleted()
    {
        if (completedSent)
        {
            return;
        }


        if (changeDoll == null)
        {
            return;
        }


        bool allCompleted =
            changeDoll.IsBrokenLegFixed &&
            changeDoll.IsBrokenArmFixed &&
            changeDoll.IsSpringFixed;


        Debug.Log(
            $"[DollRepairCompletion] 전체 수리 상태 = {allCompleted}"
        );


        if (!allCompleted)
        {
            return;
        }


        completedSent = true;


        changeDoll.RepairDoll();


        Debug.Log(
            "[DollRepairCompletion] ★ 인형 전체 수리 완료 ★"
        );


        if (keyDropManager != null)
        {
            keyDropManager.SetRepairCompleted(true);
        }
        else
        {
            Debug.LogWarning(
                "[DollRepairCompletion] KeyDropManager가 없습니다.",
                this
            );
        }
    }
}