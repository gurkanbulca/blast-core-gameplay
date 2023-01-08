using GridSystem;
using NUnit.Framework;
using UnityEngine;

namespace _Project.Tests.EditMode.GridSystem
{
    public class GridDataTests
    {
        [Test]
        public void IsSizeSetCorrectAfterInitialize()
        {
            var size = new Vector2Int(1, 2);
            var gridData = new GridData(size, Vector3.zero, 0);
            Assert.AreEqual(size, gridData.Size);
        }

        [Test]
        public void IsOriginSetCorrectAfterInitialize()
        {
            var origin = new Vector3(1, 2, 3);
            var gridData = new GridData(Vector2Int.down, origin, 0);
            Assert.AreEqual(origin, gridData.Origin);
        }

        [Test]
        public void IsSpacingSetCorrectAfterInitialize()
        {
            var spacing = 1f;
            var gridData = new GridData(Vector2Int.down, Vector3.back, spacing);
            Assert.AreEqual(spacing, gridData.Spacing);
        }
    }
}