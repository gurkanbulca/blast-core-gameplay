using System.Linq;
using UnityEngine;

public class Grid
{
    public Cell[,] grid;

    public readonly GridData GridData;

    public Grid(GridData gridData)
    {
        GridData = gridData;
    }

    public void GenerateGrid()
    {
        grid = new Cell[GridData.size.x, GridData.size.y];
        for (int x = 0; x < GridData.size.x; x++)
        {
            for (int y = 0; y < GridData.size.y; y++)
            {
                var cell = new Cell(CalculateCellPosition(x, y, GridData.size.x, GridData.origin),
                    new Vector2Int(x, y));
                grid[x, y] = cell;
            }
        }
    }

    public Vector3 CalculateCellPosition(int x, int y, int columnCount, Vector3 origin)
    {
        return new Vector3((origin.x - GridData.spacing * columnCount / 2f + GridData.spacing / 2f) + GridData.spacing * x,
            origin.y - GridData.spacing * y);
    }

    public Cell[] GetColumn(int index)
    {
        return Enumerable.Range(0, grid.GetLength(1))
            .Select(x => grid[index, x])
            .ToArray();
    }

    public Cell[] GetRow(int index)
    {
        return Enumerable.Range(0, grid.GetLength(0))
            .Select(x => grid[x, index])
            .ToArray();
    }

    public Cell GetCell(int x, int y)
    {
        return grid[x, y];
    }
}