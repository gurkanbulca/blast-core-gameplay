using NUnit.Framework;
using Extensions;
using UnityEngine;

namespace _Project.Tests.EditMode.ExtensionTests
{
    public class ExtensionTests
    {
        [Test]
        public void IsWithYExtensionWorkingCorrectly()
        {
            var point = new Vector3(1, 2, 3);
            point = point.WithX(4);
            Assert.AreEqual(4, point.x);
        }

        [Test]
        public void IsWithXExtensionWorkingCorrectly()
        {
            var point = new Vector3(1, 2, 3);
            point = point.WithY(4);
            Assert.AreEqual(4, point.y);
        }
    }
}