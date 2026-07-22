using UnityEngine;

public struct RandomVectors
{
    public static Vector2 Range2(float minInclusive, float maxInclusive)
    {
        return new Vector2
        (
            Random.Range(minInclusive, maxInclusive), 
            Random.Range(minInclusive, maxInclusive)
        );
    }
    public static Vector2 Range2(Vector2 minInclusive, Vector2 maxInclusive)
    {
        return new Vector2
        (
            Random.Range(minInclusive.x, maxInclusive.x),
            Random.Range(minInclusive.y, maxInclusive.y)
        );
    }
    
    public static Vector3 Range3(float minInclusive, float maxInclusive)
    {
        return new Vector3
        (
            Random.Range(minInclusive, maxInclusive), 
            Random.Range(minInclusive, maxInclusive), 
            Random.Range(minInclusive, maxInclusive)
        );
    }
    public static Vector3 Range3(Vector3 minInclusive, Vector3 maxInclusive)
    {
        return new Vector3
        (
            Random.Range(minInclusive.x, maxInclusive.x),
            Random.Range(minInclusive.y, maxInclusive.y),
            Random.Range(minInclusive.z, maxInclusive.z)
        );
    }
    
    public static Vector4 Range4(float minInclusive, float maxInclusive)
    {
        return new Vector4
        (
            Random.Range(minInclusive, maxInclusive), 
            Random.Range(minInclusive, maxInclusive), 
            Random.Range(minInclusive, maxInclusive), 
            Random.Range(minInclusive, maxInclusive)
        );
    }
    public static Vector4 Range4(Vector4 minInclusive, Vector4 maxInclusive)
    {
        return new Vector4
        (
            Random.Range(minInclusive.x, maxInclusive.x),
            Random.Range(minInclusive.y, maxInclusive.y),
            Random.Range(minInclusive.z, maxInclusive.z),
            Random.Range(minInclusive.w, maxInclusive.w)
        );
    }
}
