using UnityEngine;

public struct MathRhu
{
    public static float DistanceOnAxis(Vector3 a, Vector3 b, Vector3 axis)
    {
        a = Vector3.Project(a, axis);
        b = Vector3.Project(b, axis);
        return (Vector3.Distance(a, b));
    }
}
