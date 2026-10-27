using System;
using Unity.VisualScripting;
using UnityEngine;

public class TextureChange
{
    private static readonly Lazy<MaterialPropertyBlock> instance = new Lazy<MaterialPropertyBlock>(() => new MaterialPropertyBlock());

    public static MaterialPropertyBlock Instance => instance.Value;
}
