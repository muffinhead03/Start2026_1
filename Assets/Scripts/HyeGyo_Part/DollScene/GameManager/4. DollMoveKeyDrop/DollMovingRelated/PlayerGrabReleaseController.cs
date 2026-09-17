using UnityEngine;

public class PlayerGrabReleaseController : MonoBehaviour
{
    [SerializeField]
    private Player_Grab playerGrab;


    private bool blockRelease = false;


    private void Start()
    {
        if (playerGrab == null)
        {
            playerGrab =
                FindFirstObjectByType<Player_Grab>();
        }
    }


    // E 입력에서 기존 Release() 대신 이것을 호출
    public void TryRelease()
    {
        if (blockRelease)
        {
            Debug.Log(
                "[ReleaseController] Release 임시 차단"
            );

            return;
        }


        if (playerGrab != null)
        {
            playerGrab.Release();
        }
    }


    public void BlockRelease()
    {
        blockRelease = true;

        Debug.Log(
            "[ReleaseController] Release 차단 ON"
        );
    }


    public void AllowRelease()
    {
        blockRelease = false;

        Debug.Log(
            "[ReleaseController] Release 차단 OFF"
        );
    }
}