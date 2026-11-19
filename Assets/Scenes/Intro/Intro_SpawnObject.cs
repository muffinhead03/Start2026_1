using UnityEngine;

public class Intro_SpawnObject: MonoBehaviour
{
    [SerializeField] Transform obj, target_obj;


    public void SpawnObject()
    {
        SetObjectPos(obj, target_obj);
    }

    protected void SetObjectPos(Transform obj, Transform target_obj)
    {
        obj.position = target_obj.position;
        obj.rotation = target_obj.rotation;
    }
}
