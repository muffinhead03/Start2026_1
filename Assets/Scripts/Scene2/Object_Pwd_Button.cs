using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Object_Pwd_Button : APwdButton, IMouseInteractable
{
    public UnityEvent OnPressed;

    [Header("Materials")]
    [SerializeField] Material[] materials;

    MeshRenderer mesh;

    Collider col;

    public string GetId() { return id; }

    public void OnMouseEnter() { }

    public void OnMouseExit() { }

    public void OnMouseReleased() { }

    public void OnMouseClick()
    {
        Pressed();
    }

    public override void Pressed()
    {
        object_pwd.PressButton(id);

        OnPressed?.Invoke();

        StartCoroutine(ChangeMaterial());
    }

    IEnumerator ChangeMaterial()
    {
        if (mesh == null) mesh = GetComponent<MeshRenderer>();
        if (col == null) col = GetComponent<Collider>();

        mesh.sharedMaterial = materials[1];

        col.enabled = false;

        yield return new WaitForSeconds(0.5f);

        mesh.sharedMaterial = materials[0];

        col.enabled = true;
    }
}
