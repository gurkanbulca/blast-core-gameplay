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
        private int _rowCount;

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
            _fallHeight = Mathf.Infinity;
        }

        #endregion

        #region Action Handlers

        /// <summary>
        /// On beginning of the falling state, method sets sorting for current grid position and also trigger fall animation if fall height has been set.
        /// </summary>
        /// <param name="state"></param>
        private void HandleGameStateChange(GameState state)
        {
            if (state != GameState.Falling)
                return;
            SetSortingOrder(_rowCount - gridPosition.y);
            Fall();
            blockGroup = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize block type by block data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="rowCount"></param>
        public void Initialize(BlockData data, int rowCount)
        {
            blockData = data;
            SetIcon(0);
            _rowCount = rowCount;
        }

        /// <summary>
        /// Sets grid position.
        /// </summary>
        /// <param name="position"></param>
        public void SetGridPosition(Vector2Int position)
        {
            gridPosition = position;
        }

        /// <summary>
        /// Set SpriteRenderer sprite from BlockData icons by sprite index.
        /// </summary>
        /// <param name="index"></param>
        public void SetIcon(int index)
        {
            _renderer.sprite = blockData.icons[index];
        }

        /// <summary>
        /// Sets fall height.
        /// </summary>
        /// <param name="height"></param>
        public void FallTo(float height)
        {
            _fallHeight = height;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Trigger fall animation if fall height has been set.
        /// </summary>
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

        /// <summary>
        /// Set sorting order of SpriteRenderer.
        /// </summary>
        /// <param name="sortingOrder"></param>
        private void SetSortingOrder(int sortingOrder)
        {
            _renderer.sortingOrder = sortingOrder;
        }

        #endregion
    }
}