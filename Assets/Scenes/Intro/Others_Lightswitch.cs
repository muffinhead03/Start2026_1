using UnityEngine;

public class Others_Lightswitch : MonoBehaviour
{
    [SerializeField] MeshRenderer[] light_mesh;
    [SerializeField] GameObject[] light_objects;

    [SerializeField] Material[] light_materials;

    bool isLightOn;

    private void Start()
    {
        isLightOn = false;
    }

    public void ToggleLight()
    {
        if (isLightOn) TurnOffLight();
        else TurnOnLight();

    }

    void TurnOnLight()
    {
        if (isLightOn) return;
        isLightOn = true;

        foreach(MeshRenderer mesh in light_mesh)
        {
            mesh.sharedMaterial = light_materials[1];
        }

        foreach(GameObject obj in light_objects)
        {
            obj.SetActive(true);
        }
    }

    void TurnOffLight()
    {
        if (!isLightOn) return;
        isLightOn = false;

        foreach (MeshRenderer mesh in light_mesh)
        {
            mesh.sharedMaterial = light_materials[0];
        }

        foreach (GameObject obj in light_objects)
        {
            obj.SetActive(false);
        }
    }
}
