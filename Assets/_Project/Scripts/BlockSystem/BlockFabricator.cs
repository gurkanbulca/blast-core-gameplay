using System.Collections.Generic;
using UnityEngine;

namespace BlockSystem
{
    [CreateAssetMenu(menuName = "Block Fabricator", fileName = "Block Fabricator")]

    public class BlockFabricator : ScriptableObject
    {
        #region Serialized Fields

        [SerializeField] private Block blockPrefab;
        [SerializeField] private BlockData[] blockTypes;

        #endregion

        #region Private Fields

        private Transform _poolParent;
        private Queue<Block> _poolItems;
        private int _poolSize = 1;

        #endregion

        #region Public Methods

        public void ReturnToPool(Block block)
        {
            if (_poolItems.Count >= _poolSize)
            {
                Destroy(block.gameObject);
            }

            block.transform.SetParent(_poolParent);
            _poolItems.Enqueue(block);
            block.gameObject.SetActive(false);
        }


        public void InitializePool(int poolSize)
        {
            _poolSize = poolSize;
            _poolItems = new Queue<Block>();

            if (_poolParent)
            {
                Destroy(_poolParent.gameObject);
            }

            _poolParent = new GameObject(blockPrefab.name + " Pool").transform;

            GrowPool(_poolSize);
        }


        public Block Get(int colorIndex)
        {
            if (_poolItems == null)
                InitializePool(_poolSize);

            if (_poolItems.Count == 0)
                GrowPool(_poolSize);

            var block = _poolItems.Dequeue();
            block.gameObject.SetActive(true);
            block.Initialize(blockTypes[colorIndex]);
            return block;
        }

        #endregion

        #region Helper Methods

        private void GrowPool(int poolSize)
        {
            for (int i = 0; i < poolSize; i++)
            {
                var poolObject = Instantiate(blockPrefab, _poolParent);
                poolObject.gameObject.SetActive(false);
                _poolItems.Enqueue(poolObject);
            }
        }

        #endregion
    }
}