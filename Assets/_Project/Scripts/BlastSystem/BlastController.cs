using System.Collections.Generic;
using System.Threading;
using BlockSystem;
using Extensions;
using GameStateSystem;
using UnityEngine;
using GridSystem;
using LevelSystem;


namespace BlastSystem
{
    public class BlastController
    {
        #region Private Fields

        private float[] _spawnHeights;
        private int _fallingBlockCount;

        private readonly GameStateController _stateController;
        private readonly GridController _gridController;
        private readonly LevelData _levelData;
        private readonly BlockFabricator _blockFabricator;
        private readonly float _spacing;
        private readonly Vector3 _gridOrigin;

        #endregion

        #region Constructor

        public BlastController(GameStateController stateController, GridController gridController, LevelData levelData,
            BlockFabricator blockFabricator, Vector3 gridOrigin, float spacing)
        {
            _stateController = stateController;
            _gridController = gridController;
            _levelData = levelData;
            _blockFabricator = blockFabricator;
            _spacing = spacing;
            _gridOrigin = gridOrigin;

            _blockFabricator.InitializePool(_levelData.columnCount * _levelData.rowCount);
            InitializeSpawnHeights(levelData.columnCount);
            SpawnBlocks(levelData);

            Block.OnBlockFell += HandleBlockFell;
            GameStateController.OnCurrentGameStateChanged += HandleGameStateChange;
        }

        #endregion

        #region Action Handlers

        private void HandleGameStateChange(GameState state)
        {
            if (state == GameState.Falling)
                ResetSpawnHeights();
        }


        private void HandleBlockFell()
        {
            if (_stateController.currentGameState != GameState.Falling)
                return;

            _fallingBlockCount -= 1;
            if (_fallingBlockCount > 0)
                return;

            _stateController.currentGameState = GameState.Idle;
            FindGroups();
        }

        #endregion

        #region Destroy Block

        public void DestroyBlockGroup(List<Block> blockGroup)
        {
            _stateController.currentGameState = GameState.Blasting;
            var columnIndices = new List<int>();
            var destroyedColumns = new HashSet<int>();
            foreach (var block in blockGroup)
            {
                columnIndices.Add(block.gridPosition.x);
                destroyedColumns.Add(block.gridPosition.x);
                var cell = _gridController.GetCell(block.gridPosition.x, block.gridPosition.y);
                cell.Block = null;
                _blockFabricator.ReturnToPool(block);
            }

            DropBlocks(destroyedColumns);
            SpawnMissingBlocks(columnIndices);
            _stateController.currentGameState = GameState.Falling;
        }

        #endregion

        #region Spawn Block

        private void SpawnMissingBlocks(List<int> columnIndices)
        {
            _stateController.currentGameState = GameState.Spawning;
            foreach (var columnIndex in columnIndices)
            {
                var column = _gridController.GetColumn(columnIndex);
                Cell targetCell = null;
                foreach (var cell in column)
                {
                    if (cell.Block != null)
                        break;
                    targetCell = cell;
                }

                if (targetCell != null)
                    SpawnRandomBlock(_levelData, targetCell.GridCoordinate.x, targetCell.GridCoordinate.y);
            }
        }

        private void SpawnBlocks(LevelData levelData)
        {
            _stateController.currentGameState = GameState.Spawning;
            for (var y = _gridController.grid.GetLength(1) - 1; y >= 0; y--)
            {
                for (var x = 0; x < _gridController.grid.GetLength(0); x++)
                {
                    SpawnRandomBlock(levelData, x, y);
                }
            }

            _stateController.currentGameState = GameState.Falling;
        }


        private void SpawnRandomBlock(LevelData levelData, int x, int y)
        {
            var cell = _gridController.grid[x, y];
            var block = _blockFabricator.Get(Random.Range(0, levelData.colorCount), _levelData.rowCount);
            block.transform.position = cell.Position.WithY(_spawnHeights[x]);
            _spawnHeights[x] += _spacing;
            block.SetGridPosition(new Vector2Int(x, y));
            cell.Block = block;
            block.FallTo(cell.Position.y);
            _fallingBlockCount++;
        }

        #endregion

        #region Shuffle

        private void Shuffle()
        {
            foreach (var cell in _gridController.grid)
            {
                _blockFabricator.ReturnToPool(cell.Block);
            }

            SpawnBlocks(_levelData);
        }

        #endregion

        #region Helper Methods

        private void ResetSpawnHeights()
        {
            for (var i = 0; i < _spawnHeights.Length; i++)
            {
                _spawnHeights[i] = _gridOrigin.y + _spacing * 2;
            }
        }

        private void InitializeSpawnHeights(int columnCount)
        {
            _spawnHeights = new float[columnCount];
            ResetSpawnHeights();
        }

        private void FindGroups()
        {
            var highestGroupLength = 0;
            foreach (var cell in _gridController.grid)
            {
                var group = FindGroupForBlock(cell.Block);
                cell.Block.blockGroup = group;
                cell.Block.SetIcon(GetIconIndex(group.Count));
                highestGroupLength = Mathf.Max(highestGroupLength, group.Count);
            }

            if (highestGroupLength < 2)
            {
                Debug.Log("DeadLock detected! Blocks will be shuffled.");
                Shuffle();
            }
        }

        private int GetIconIndex(int groupLength)
        {
            if (groupLength > _levelData.thirdIconCondition)
            {
                return 3;
            }

            if (groupLength > _levelData.secondIconCondition)
            {
                return 2;
            }

            return groupLength > _levelData.firstIconCondition ? 1 : 0;
        }

        #endregion

        #region Dropping

        private void DropBlocks(HashSet<int> destroyedColumns)
        {
            var threads = new List<Thread>();
            foreach (var columnIndex in destroyedColumns)
            {
                var thread = new Thread(() => DropColumn(_gridController.GetColumn(columnIndex)));
                thread.Start();
                threads.Add(thread);
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        private void DropColumn(IReadOnlyList<Cell> cells)
        {
            for (var i = cells.Count - 2; i >= 0; i--)
            {
                var cell = cells[i];
                if (cell.Block == null)
                    continue;
                var nextIndex = i + 1;
                var targetCell = cell;
                while (nextIndex < cells.Count)
                {
                    if (cells[nextIndex].Block != null)
                        break;
                    targetCell = cells[nextIndex++];
                }

                if (targetCell == cell)
                    continue;
                targetCell.Block = cell.Block;
                cell.Block = null;
                targetCell.Block.FallTo(targetCell.Position.y);
                targetCell.Block.SetGridPosition(targetCell.GridCoordinate);
                _fallingBlockCount++;
            }
        }

        #endregion

        #region Block Grouping

        private List<Block> FindGroupForBlock(Block block)
        {
            if (block.blockGroup != null)
                return block.blockGroup;
            var group = new List<Block>();
            group.Add(block);
            block.blockGroup = group;
            CheckNeighborsForGroup(ref group, block);
            return group;
        }

        private void CheckNeighborsForGroup(ref List<Block> group, Block block)
        {
            CheckRight(ref group, block);
            CheckLeft(ref group, block);
            CheckUp(ref group, block);
            CheckDown(ref group, block);
        }

        private void CheckNeighbor(ref List<Block> group, Block block, int x, int y)
        {
            var neighbor = _gridController.GetCell(x, y).Block;
            if (neighbor.blockGroup != null)
                return;
            if (neighbor.blockData == block.blockData)
            {
                group.Add(neighbor);
                neighbor.blockGroup = group;
                CheckNeighborsForGroup(ref group, neighbor);
            }
        }

        private void CheckDown(ref List<Block> group, Block block)
        {
            if (block.gridPosition.y + 1 == _levelData.rowCount)
                return;
            CheckNeighbor(ref group, block, block.gridPosition.x, block.gridPosition.y + 1);
        }

        private void CheckUp(ref List<Block> group, Block block)
        {
            if (block.gridPosition.y - 1 < 0)
                return;
            CheckNeighbor(ref group, block, block.gridPosition.x, block.gridPosition.y - 1);
        }

        private void CheckLeft(ref List<Block> group, Block block)
        {
            if (block.gridPosition.x - 1 < 0)
                return;
            CheckNeighbor(ref group, block, block.gridPosition.x - 1, block.gridPosition.y);
        }

        private void CheckRight(ref List<Block> group, Block block)
        {
            if (block.gridPosition.x + 1 == _levelData.columnCount)
                return;
            CheckNeighbor(ref group, block, block.gridPosition.x + 1, block.gridPosition.y);
        }

        #endregion
    }
}