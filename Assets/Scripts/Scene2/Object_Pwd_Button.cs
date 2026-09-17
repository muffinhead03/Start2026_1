using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Object_Pwd_Button : MonoBehaviour
{
    public string id;
    public UnityEvent OnPressed;

    [Header("Materials")]
    [SerializeField] Material[] materials;

    MeshRenderer mesh;

    Collider col;

    public void Pressed()
    {
        OnPressed?.Invoke();

        StartCoroutine(ChangeMaterial());
    }

    IEnumerator ChangeMaterial()
    {
        if (mesh == null) mesh = GetComponent<MeshRenderer>();
        if (col == null) col = GetComponent<Collider>();

        mesh.material = materials[1];

        col.enabled = false;

        yield return new WaitForSeconds(0.5f);

        mesh.material = materials[0];

        col.enabled = true;
    }
}
