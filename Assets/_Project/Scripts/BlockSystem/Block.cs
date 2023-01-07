using System;
using System.Collections.Generic;
using DG.Tweening;
using GameStateSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BlockSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Block : MonoBehaviour
    {
        #region Actions

        public static event Action OnBlockFell = delegate { };

        #endregion

        #region Serialized Fields

        [SerializeField] private float fallSpeed = 2;

        #endregion

        #region Public Fields

        [ReadOnly] public List<Block> blockGroup;

        #endregion

        #region Private Fields

        private SpriteRenderer _renderer;
        private Transform _transform;
        private float _fallHeight = Mathf.Infinity;

        #endregion

        #region Properties

        public Vector2Int gridPosition { get; private set; }
        public BlockData blockData { get; private set; }

        #endregion

        #region Unity LifeCycle

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _transform = transform;
        }

        private void OnEnable()
        {
            GameStateController.OnCurrentGameStateChanged += HandleGameStateChange;
        }

        private void OnDisable()
        {
            GameStateController.OnCurrentGameStateChanged -= HandleGameStateChange;
        }

        #endregion

        #region Action Handlers

        private void HandleGameStateChange(GameState state)
        {
            if (state != GameState.Falling)
                return;
            Fall();
            blockGroup = null;
        }

        #endregion

        #region Public Methods

        public void Initialize(BlockData data)
        {
            blockData = data;
            SetIcon(0);
        }


        public void SetGridPosition(Vector2Int position)
        {
            gridPosition = position;
        }

        public void SetSortingOrder(int sortingOrder)
        {
            _renderer.sortingOrder = sortingOrder;
        }

        public void SetIcon(int index)
        {
            _renderer.sprite = blockData.icons[index];
        }

        public void FallTo(float height)
        {
            _fallHeight = height;
        }

        #endregion

        #region Helper Methods

        private void Fall()
        {
            if (float.IsPositiveInfinity(_fallHeight))
                return;
            var currentHeight = _transform.position.y;
            _transform.DOMoveY(_fallHeight, (currentHeight - _fallHeight) / fallSpeed)
                .OnComplete(() =>
                {
                    _fallHeight = Mathf.Infinity;
                    OnBlockFell();
                });
        }

        #endregion
    }
}