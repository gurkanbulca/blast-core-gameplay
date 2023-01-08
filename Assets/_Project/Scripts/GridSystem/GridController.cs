using System;
using System.Linq;
using UnityEngine;

namespace GridSystem
{
    public class GridController
    {
        #region Private Fields

        private readonly GridData _gridData;

        #endregion

        #region Properties

        public Cell[,] grid { get; private set; }

        #endregion

        #region Constructor

        public GridController(GridData gridData)
        {
            _gridData = gridData;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generates grid by grid data.
        /// </summary>
        public void GenerateGrid()
        {
            grid = new Cell[_gridData.Size.x, _gridData.Size.y];
            for (int x = 0; x < _gridData.Size.x; x++)
            {
                for (int y = 0; y < _gridData.Size.y; y++)
                {
                    var cell = new Cell(CalculateCellPosition(x, y, _gridData.Size.x, _gridData.Origin),
                        new Vector2Int(x, y));
                    grid[x, y] = cell;
                }
            }
        }
        
        /// <summary>
        /// Calculates cell position for specific coordinate.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="columnCount"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Vector3 CalculateCellPosition(int x, int y, int columnCount, Vector3 origin)
        {
            if (x < 0 || y < 0 || columnCount < 0)
                throw new ArgumentOutOfRangeException(nameof(x), "Value can not be negative!");

            return new Vector3(
                (origin.x - _gridData.Spacing * columnCount / 2f + _gridData.Spacing / 2f) + _gridData.Spacing * x,
                origin.y - _gridData.Spacing * y, origin.z);
        }

        /// <summary>
        /// Gives specific column of the grid by column index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Cell[] GetColumn(int index)
        {
            return Enumerable.Range(0, grid.GetLength(1))
                .Select(x => grid[index, x])
                .ToArray();
        }

        /// <summary>
        /// Gives specific cell of the grid by coordinate.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Cell GetCell(int x, int y)
        {
            return grid[x, y];
        }

        #endregion
    }
}