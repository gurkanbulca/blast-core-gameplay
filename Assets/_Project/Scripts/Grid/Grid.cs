using System.Linq;

public class Grid
{
    public Cell[,] grid;

    private GridData _gridData;

    public Grid(GridData gridData)
    {
        _gridData = gridData;
    }

    public void GenerateGrid()
    {
    }

    public Cell[] GetRow(int index)
    {
        return Enumerable.Range(0, grid.GetLength(1))
            .Select(x => grid[index, x])
            .ToArray();
    }

    public Cell[] GetColumn(int index)
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