using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Event_LoadScene : MonoBehaviour
{
    [Header("Scene")]
    public string sceneName;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 3)
        {
            GoToScene(sceneName);
        }
    }
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
