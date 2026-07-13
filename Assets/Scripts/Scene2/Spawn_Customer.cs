using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class Spawn_Customer : MonoBehaviour
{
    [SerializeField]
    private GameObject customer_prefab;

    List<GameObject> pool;

    void Start()
    {
        pool= new List<GameObject>();
    }

    public void Spawn()
    {
        GameObject spawned = null;
        foreach (GameObject obj in pool)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                spawned = obj;
                break;
            }
        }

        if (spawned == null)
        {
            spawned = Instantiate(customer_prefab, transform);
            pool.Add(spawned);
        }
    }
}
