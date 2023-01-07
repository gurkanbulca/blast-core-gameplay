using System.Collections.Generic;
using BlastSystem;
using BlockSystem;
using GameStateSystem;
using GridSystem;
using LevelSystem;
using UnityEngine;

namespace GameCore
{
    public class GameManager : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private GameState initialGameState;
        [SerializeField] private LevelData exampleLevel;
        [SerializeField] private BlockFabricator blockFabricator;
        [SerializeField] private Vector3 gridOrigin;

        #endregion

        #region Private Fields

        private InputController _inputController;
        private GameStateController _stateController;
        private BlastController _blastController;
        private GridController _gridController;

        private const float Spacing = .88f;

        #endregion

        #region Unity LifeCycle

        private void Awake()
        {
            _stateController = new GameStateController(initialGameState);
            _inputController = new InputController();
            _inputController.OnRayCastHit += HandleRayCastHit;
        }

        private void Start()
        {
            LoadLevel(exampleLevel);
        }

        private void OnDestroy()
        {
            _inputController.OnRayCastHit -= HandleRayCastHit;
        }


        private void Update()
        {
            if (_stateController.currentGameState == GameState.Idle)
                _inputController.Tick();
        }

        #endregion

        #region Action Handlers

        private void HandleRayCastHit(List<Block> blockGroup)
        {
            if (blockGroup.Count < 2)
                return;
            _blastController.DestroyBlockGroup(blockGroup);
        }

        #endregion

        #region LevelController

        private void LoadLevel(LevelData levelData)
        {
            CreateGrid();
            _gridController.GenerateGrid();
            _blastController =
                new BlastController(_stateController, _gridController, levelData, blockFabricator, gridOrigin, Spacing);
        }

        #endregion

        #region Helper Methods

        private void CreateGrid()
        {
            _gridController = new GridController(new GridData(
                new Vector2Int(exampleLevel.columnCount, exampleLevel.rowCount),
                gridOrigin, Spacing));
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            if (!exampleLevel)
                return;
            if (_gridController == null)
            {
                CreateGrid();
            }

            Gizmos.color = Color.yellow;

            for (var x = 0; x < exampleLevel.columnCount; x++)
            {
                for (var y = 0; y < exampleLevel.rowCount; y++)
                {
                    var point = _gridController.CalculateCellPosition(x, y, exampleLevel.columnCount, gridOrigin);
                    Gizmos.DrawWireCube(point, Spacing * Vector3.one);
                }
            }
        }

        #endregion
    }
}