using System.Collections;
using BlockSystem;
using GameStateSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BlockSystemTests
{
    public class BlockTests
    {
        private Sprite[] icons = new[]
        {
            Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.zero),
            Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.zero),
            Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.zero),
            Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), Vector2.zero),
        };

        private BlockData _blockData;
        private const int RowCount = 5;

        private Block InitializeBlock()
        {
            var go = new GameObject();
            go.AddComponent<SpriteRenderer>();
            var block = go.AddComponent<Block>();

            _blockData = ScriptableObject.CreateInstance<BlockData>();
            _blockData.icons = icons;

            block.Initialize(_blockData, RowCount);
            return block;
        }


        [UnityTest]
        public IEnumerator IsBlockDataSetAfterInitialize()
        {
            var block = InitializeBlock();
            Assert.AreEqual(_blockData, block.blockData);
            yield break;
        }

        [UnityTest]
        public IEnumerator IsDefaultIconSetAfterInitialize()
        {
            var block = InitializeBlock();
            Assert.AreEqual(icons[0], block.GetComponent<SpriteRenderer>().sprite);
            yield break;
        }

        [UnityTest]
        public IEnumerator IsSetGridPositionWorkCorrectly()
        {
            var block = InitializeBlock();
            var gridPosition = new Vector2Int(1, 2);
            block.SetGridPosition(gridPosition);

            Assert.AreEqual(gridPosition, block.gridPosition);
            yield break;
        }

        [UnityTest]
        [TestCase(0, ExpectedResult = null)]
        [TestCase(1, ExpectedResult = null)]
        [TestCase(2, ExpectedResult = null)]
        [TestCase(3, ExpectedResult = null)]
        public IEnumerator IsSetIconWorkCorrectly(int index)
        {
            var block = InitializeBlock();
            block.SetIcon(index);
            Assert.AreEqual(icons[index], block.GetComponent<SpriteRenderer>().sprite);
            yield break;
        }

        [UnityTest]
        public IEnumerator IsFallingToCorrectPosition()
        {
            var height = 1f;
            var block = InitializeBlock();
            var stateController = new GameStateController(GameState.Idle);
            var isWaiting = true;
            Block.OnBlockFell += () => isWaiting = false;
            block.FallTo(height);
            stateController.currentGameState = GameState.Falling;
            yield return new WaitWhile(() => isWaiting);
            Assert.AreEqual(height, block.transform.position.y);
        }

        [UnityTest]
        [TestCase(0, ExpectedResult = null)]
        [TestCase(1, ExpectedResult = null)]
        [TestCase(2, ExpectedResult = null)]
        [TestCase(3, ExpectedResult = null)]
        [TestCase(4, ExpectedResult = null)]
        public IEnumerator IsSortingOrderSettingCorrectly(int y)
        {
            var gridPosition = new Vector2Int(0, y);

            var block = InitializeBlock();
            var stateController = new GameStateController(GameState.Idle);
            block.SetGridPosition(gridPosition);

            stateController.currentGameState = GameState.Falling;

            Assert.AreEqual(RowCount - gridPosition.y, block.GetComponent<SpriteRenderer>().sortingOrder);
            yield return null;
        }
    }
}