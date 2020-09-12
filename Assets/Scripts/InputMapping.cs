using System;
using UnityEngine.Events;

[Serializable]
public class InputMapping
{
    [Serializable]
    public enum Variant
    {
        Hold,
        Press,
        Release
    }

    public string key;
    public Variant variant;
    public UnityEvent handler;
}
