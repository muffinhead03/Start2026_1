using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Object_Pwd_Button : MonoBehaviour, APwdButton
{
    [SerializeField]private string id;
    public UnityEvent OnPressed;

    [Header("Materials")]
    [SerializeField] Material[] materials;

    MeshRenderer mesh;

    Collider col;

    public string GetId() { return id; }

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
