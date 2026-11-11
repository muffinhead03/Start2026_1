using UnityEngine;
using System.Collections;

public class Others_Lightswitch : MonoBehaviour
{
    [SerializeField] GameObject[] light_objects;

    [SerializeField] Material light_material;

    [SerializeField] Transform lever;

    [SerializeField] Intro_StepForOpen step;

    bool isLightOn;
    bool isActive;
    float[] lever_rotate = { 80, -80 };

    private void Start()
    {
        isLightOn = false;
        isActive = true;

        lever.localRotation = Quaternion.Euler(0, lever_rotate[0], 0);
    }

    public void ToggleLight()
    {
        if (!isActive) return;

        if (isLightOn) TurnOffLight();
        else TurnOnLight();

        StartCoroutine(DelayToTurn());
    }

    void TurnOnLight()
    {
        if (isLightOn) return;
        isLightOn = true;

        lever.localRotation = Quaternion.Euler(0, lever_rotate[1], 0);

        light_material.EnableKeyword("_EMISSION");

        foreach(GameObject obj in light_objects)
        {
            obj.SetActive(true);
        }

        step.SetLight(true);
    }

    void TurnOffLight()
    {
        if (!isLightOn) return;
        isLightOn = false;

        lever.localRotation = Quaternion.Euler(0, lever_rotate[0], 0);

        light_material.DisableKeyword("_EMISSION");

        foreach (GameObject obj in light_objects)
        {
            obj.SetActive(false);
        }

        step.SetLight(false);
    }

    IEnumerator DelayToTurn()
    {
        isActive = false;

        yield return new WaitForSeconds(2.0f);

        isActive = true;
    }
}
