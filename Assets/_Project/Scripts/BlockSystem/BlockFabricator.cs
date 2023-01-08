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

        /// <summary>
        /// Returns block to pool.
        /// </summary>
        /// <param name="block"></param>
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

        /// <summary>
        /// Initialize block pool by pool size.
        /// </summary>
        /// <param name="poolSize"></param>
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

        /// <summary>
        /// Get available block from pool. If pool is empty, grows pool.
        /// </summary>
        /// <param name="colorIndex"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public Block Get(int colorIndex, int rowCount)
        {
            if (_poolItems == null)
                InitializePool(_poolSize);

            if (_poolItems.Count == 0)
                GrowPool(_poolSize);

            var block = _poolItems.Dequeue();
            block.gameObject.SetActive(true);
            block.Initialize(blockTypes[colorIndex], rowCount);
            return block;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Grows pool by pool size.
        /// </summary>
        /// <param name="poolSize"></param>
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