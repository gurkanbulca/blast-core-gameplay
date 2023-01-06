using UnityEngine;

public class GridData
{
    public Vector2Int size;
    public Vector3 origin;
    public float spacing;

    public GridData(Vector2Int size, Vector3 origin, float spacing)
    {
        this.size = size;
        this.origin = origin;
        this.spacing = spacing;
    }
}