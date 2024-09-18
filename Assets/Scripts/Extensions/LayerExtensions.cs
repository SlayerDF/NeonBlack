using UnityEngine;

public static class LayerExtensions
{
    public static LayerMask ToMask(this Layer layer)
    {
        return 1 << (int)layer;
    }
}
