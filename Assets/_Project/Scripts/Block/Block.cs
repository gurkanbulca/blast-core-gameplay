using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Block : MonoBehaviour
{
    public static event Action<Block> OnReturnToPool = delegate { };

    public BlockData BlockData { get; private set; }


    private SpriteRenderer _renderer;


    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void OnDisable()
    {
        OnReturnToPool(this);
    }


    public void Initialize(BlockData blockData)
    {
        BlockData = blockData;
        SetIcon(0);
    }

    public void SetIcon(int index)
    {
        _renderer.sprite = BlockData.icons[index];
    }
}