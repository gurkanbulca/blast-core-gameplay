using System;
using GridSystem;
using NUnit.Framework;
using UnityEngine;
using Extensions;


namespace _Project.Tests.EditMode.GridSystem
{
    public class GridControllerTests
    {
        private GridData _gridData =
            new GridData(new Vector2Int(4, 5), new Vector3(1, 2, 3), 1);

        private GridController GenerateGrid()
        {
            var grid = new GridController(_gridData);
            grid.GenerateGrid();
            return grid;
        }

        [Test]
        public void IsGeneratingGrid()
        {
            var grid = GenerateGrid();
            Assert.NotNull(grid.grid);
        }

        [Test]
        public void IsGridSizeCorrect()
        {
            var grid = GenerateGrid();

            Assert.AreEqual(_gridData.Size.x, grid.grid.GetLength(0));
            Assert.AreEqual(_gridData.Size.y, grid.grid.GetLength(1));
        }

        [Test]
        public void IsOriginCorrect()
        {
            var grid = GenerateGrid();
            var midPointX = (grid.grid[grid.grid.GetLength(0) - 1, 0].Position.x + grid.grid[0, 0].Position.x) / 2;

            Assert.AreEqual(_gridData.Origin, grid.grid[0, 0].Position.WithX(midPointX));
        }

        [Test]
        public void IsSpacingCorrect()
        {
            var grid = GenerateGrid();

            var xSpacing = Mathf.Abs(grid.grid[1, 0].Position.x - grid.grid[0, 0].Position.x);
            var ySpacing = Mathf.Abs(grid.grid[0, 1].Position.y - grid.grid[0, 0].Position.y);

            Assert.AreEqual(_gridData.Spacing, xSpacing);
            Assert.AreEqual(_gridData.Spacing, ySpacing);
        }

        [Test]
        public void IsCalculateCellPositionThrowExceptionForNegativeX()
        {
            var grid = GenerateGrid();
            Assert.Throws<ArgumentOutOfRangeException>(()
                => grid.CalculateCellPosition(-1, 0, 0, Vector3.zero));
        }

        [Test]
        public void IsCalculateCellPositionThrowExceptionForNegativeY()
        {
            var grid = GenerateGrid();
            Assert.Throws<ArgumentOutOfRangeException>(()
                => grid.CalculateCellPosition(0, -1, 0, Vector3.zero));
        }

        [Test]
        public void IsCalculateCellPositionThrowExceptionForNegativeColumnCount()
        {
            var grid = GenerateGrid();
            Assert.Throws<ArgumentOutOfRangeException>(()
                => grid.CalculateCellPosition(0, 0, -1, Vector3.zero));
        }

        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(1, 1)]
        [TestCase(0, 1)]
        public void IsCalculateCellPositionResultCorrect(int x, int y)
        {
            var grid = GenerateGrid();
            var position = grid.CalculateCellPosition(x, y, _gridData.Size.x, _gridData.Origin);
            Assert.AreEqual(grid.grid[x, y].Position, position);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void IsReturnedGroupOfCellFromGetColumnCorrect(int columnIndex)
        {
            var grid = GenerateGrid();
            var column = grid.GetColumn(columnIndex);
            for (int y = 0; y < grid.grid.GetLength(1); y++)
            {
                Assert.AreEqual(grid.grid[columnIndex, y], column[y]);
            }
        }

        [TestCase(0, 3)]
        [TestCase(1, 2)]
        [TestCase(2, 0)]
        [TestCase(0, 1)]
        public void IsReturnedCellFromGetCellCorrect(int x, int y)
        {
            var grid = GenerateGrid();
            Assert.AreEqual(grid.grid[x, y], grid.GetCell(x, y));
        }
    }
}