using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 With(this Vector3 vector3, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? vector3.x, y ?? vector3.y, z ?? vector3.z);
    }
}
