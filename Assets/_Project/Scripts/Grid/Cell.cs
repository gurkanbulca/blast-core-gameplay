using UnityEngine;

public class Cell
{
    public readonly Vector3 position;
    public readonly Vector2Int gridCoordinate;
    public Block block;

    public Cell(Vector3 position, Vector2Int coordinate)
    {
        this.position = position;
        gridCoordinate = coordinate;
    }
}
