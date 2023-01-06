using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[CreateAssetMenu(menuName = "Block Fabricator", fileName = "Block Fabricator")]
public class BlockFabricator : ScriptableObject
{
    [SerializeField] private Block blockPrefab;
    [SerializeField] private BlockData[] blockTypes;

    private Transform _poolParent;
    private Queue<Block> _poolItems;
    private int _poolSize = 1;

    private void Awake()
    {
        Block.OnReturnToPool += HandleReturnToPool;
    }

    private void OnDestroy()
    {
        Block.OnReturnToPool -= HandleReturnToPool;
    }

    private void HandleReturnToPool(Block block)
    {
        if (_poolItems.Count >= _poolSize)
        {
            Destroy(block.gameObject);
        }
        block.transform.SetParent(_poolParent);
        _poolItems.Enqueue(block);
    }


    public void InitializePool(int poolSize)
    {
        _poolSize = poolSize;
        _poolItems = new Queue<Block>();

        if (_poolParent)
        {
            Destroy(_poolParent.gameObject);
        }

        _poolParent = new GameObject(nameof(blockPrefab) + " Pool").transform;

        for (int i = 0; i < poolSize; i++)
        {
            var poolObject = Instantiate(blockPrefab, _poolParent);
            _poolItems.Enqueue(poolObject);
        }
    }

    public Block Get(int colorIndex)
    {
        if (_poolItems.Count == 0)
        {
            InitializePool(_poolSize);
        }

        var block = _poolItems.Dequeue();
        block.gameObject.SetActive(true);
        block.Initialize(blockTypes[colorIndex]);
        return block;
    }
    
    
}