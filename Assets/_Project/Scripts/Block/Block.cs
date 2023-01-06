using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Block : MonoBehaviour
{
    public static event Action<Block> OnReturnToPool = delegate { };
    public static event Action OnBlockFell = delegate { };

    [SerializeField] private float fallSpeed = 2;

    public List<Block> blockGroup;
    
    private SpriteRenderer _renderer;
    private Transform _transform;
    

    public Vector2Int gridPosition { get; private set; }
    public BlockData BlockData { get; private set; }


    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _transform = transform;
    }

    private void OnEnable()
    {
        GameManager.OnCurrentGameStateChanged += HandleGameStateChange;
    }

    private void OnDisable()
    {
        OnReturnToPool(this);
        GameManager.OnCurrentGameStateChanged -= HandleGameStateChange;
    }

    private void HandleGameStateChange(GameState state)
    {
        if (state != GameState.Falling)
            return;
        blockGroup = null;
    }


    public void Initialize(BlockData blockData)
    {
        BlockData = blockData;
        SetIcon(0);
    }

    public void SetGridPosition(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        _renderer.sortingOrder = sortingOrder;
    }

    public void SetIcon(int index)
    {
        _renderer.sprite = BlockData.icons[index];
    }

    public void FallTo(float height)
    {
        var currentHeight = _transform.position.y;
        _transform.DOMoveY(height, (currentHeight - height) / fallSpeed)
            .OnComplete(() => OnBlockFell());
    }
}