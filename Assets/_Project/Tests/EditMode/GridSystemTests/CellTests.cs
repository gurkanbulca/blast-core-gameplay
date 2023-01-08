using GridSystem;
using NUnit.Framework;
using UnityEngine;

namespace _Project.Tests.EditMode.GridSystem
{
    public class CellTests
    {
        [Test]
        public void IsPositionSetCorrectAfterInitialize()
        {
            var position = Vector3.up;
            var cell = new Cell(position, Vector2Int.zero);
            Assert.AreEqual(position, cell.Position);
        }

        [Test]
        public void IsGridCoordinateSetCorrectAfterInitialize()
        {
            var gridCoordinate = Vector2Int.one;
            var cell = new Cell(Vector3.positiveInfinity, gridCoordinate);
            Assert.AreEqual(gridCoordinate, cell.GridCoordinate);
        }
    }
}