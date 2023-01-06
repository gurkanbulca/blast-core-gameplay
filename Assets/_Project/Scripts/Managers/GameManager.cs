using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static event Action<GameState> OnCurrentGameStateChanged = delegate { };

    [SerializeField] private GameState initialGameState;
    [SerializeField] private LevelData exampleLevel;
    [SerializeField] private BlockFabricator blockFabricator;
    [SerializeField] private Vector3 gridOrigin;


    private Grid _grid;
    private List<Block[]> _groups;
    private GameState _currentGameState;
    private float[] _spawnHeights;
    private int _fallingBlockCount;

    private const float Spacing = .88f;

    public GameState currentGameState
    {
        get => _currentGameState;
        private set
        {
            if (value == _currentGameState)
            {
                return;
            }

            _currentGameState = value;
            Debug.Log($"Current game state changed to: {value.ToString()}");
            OnCurrentGameStateChanged(value);
        }
    }


    private void Awake()
    {
        _groups = new List<Block[]>();
        currentGameState = initialGameState;
        Block.OnBlockFell += HandleBlockFell;
    }

    private void Start()
    {
        LoadLevel(exampleLevel);
    }

    private void OnDestroy()
    {
        Block.OnBlockFell += HandleBlockFell;
    }

    private void HandleBlockFell()
    {
        if (currentGameState != GameState.Falling)
            return;

        _fallingBlockCount -= 1;
        if (_fallingBlockCount > 0)
            return;

        currentGameState = GameState.Idle;
        FindGroups();
    }

    private void CreateGrid()
    {
        _grid = new Grid(new GridData(new Vector2Int(exampleLevel.columnCount, exampleLevel.rowCount),
            gridOrigin, Spacing));
    }

    private void LoadLevel(LevelData levelData)
    {
        SetSpawnHeights(levelData.columnCount);
        blockFabricator.InitializePool(levelData.columnCount * levelData.rowCount);
        CreateGrid();
        _grid.GenerateGrid();
        currentGameState = GameState.Spawning;
        for (int y = _grid.grid.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < _grid.grid.GetLength(0); x++)
            {
                var cell = _grid.grid[x, y];
                var block = blockFabricator.Get(Random.Range(0, levelData.colorCount));
                block.SetSortingOrder(levelData.columnCount - cell.gridCoordinate.y);
                block.SetGridPosition(new Vector2Int(x, y));
                block.transform.position = cell.position.WithY(_spawnHeights[x]);
                cell.block = block;
                block.FallTo(cell.position.y);
                _spawnHeights[x] += Spacing;
            }
        }

        _fallingBlockCount = _grid.grid.Length;
        Debug.Log(_fallingBlockCount);
        currentGameState = GameState.Falling;
    }

    private void SetSpawnHeights(int columnCount)
    {
        _spawnHeights = new float[columnCount];
        for (var i = 0; i < _spawnHeights.Length; i++)
        {
            _spawnHeights[i] = gridOrigin.y + Spacing * 3;
        }
    }

    private void CheckForDeadLock()
    {
    }

    private void Shuffle()
    {
    }

    private void FindGroups()
    {
        foreach (var cell in _grid.grid)
        {
            var group = FindGroupForBlock(cell.block);
            cell.block.blockGroup = group;
            Debug.Log(group.Count);
            cell.block.SetIcon(GetIconIndex(group.Count));
        }
    }

    private int GetIconIndex(int groupLength)
    {
        if (groupLength > exampleLevel.thirdIconCondition)
        {
            return 3;
        }

        if (groupLength > exampleLevel.secondIconCondition)
        {
            return 2;
        }

        return groupLength > exampleLevel.firstIconCondition ? 1 : 0;
    }

    private void DropBlocks(Cell[,] cells)
    {
    }

    private void DropColumn(Cell[] cells)
    {
    }

    public List<Block> FindGroupForBlock(Block block)
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

    private void CheckDown(ref List<Block> group, Block block)
    {
        if (block.gridPosition.y + 1 == exampleLevel.rowCount)
            return;
        var neighbor = _grid.GetCell(block.gridPosition.x, block.gridPosition.y + 1).block;
        if (neighbor.blockGroup != null)
            return;
        if (neighbor.BlockData == block.BlockData)
        {
            group.Add(neighbor);
            neighbor.blockGroup = group;
            CheckNeighborsForGroup(ref group, neighbor);
        }
    }

    private void CheckUp(ref List<Block> group, Block block)
    {
        if (block.gridPosition.y - 1 < 0)
            return;
        var neighbor = _grid.GetCell(block.gridPosition.x, block.gridPosition.y - 1).block;
        if (neighbor.blockGroup != null)
            return;
        if (neighbor.BlockData == block.BlockData)
        {
            group.Add(neighbor);
            neighbor.blockGroup = group;
            CheckNeighborsForGroup(ref group, neighbor);
        }
    }

    private void CheckLeft(ref List<Block> group, Block block)
    {
        if (block.gridPosition.x - 1 < 0)
            return;
        var neighbor = _grid.GetCell(block.gridPosition.x - 1, block.gridPosition.y).block;
        if (neighbor.blockGroup != null)
            return;
        if (neighbor.BlockData == block.BlockData)
        {
            group.Add(neighbor);
            neighbor.blockGroup = group;
            CheckNeighborsForGroup(ref group, neighbor);
        }
    }

    private void CheckRight(ref List<Block> group, Block block)
    {
        if (block.gridPosition.x + 1 == exampleLevel.columnCount)
            return;
        var neighbor = _grid.GetCell(block.gridPosition.x + 1, block.gridPosition.y).block;
        if (neighbor.blockGroup != null)
            return;
        if (neighbor.BlockData == block.BlockData)
        {
            group.Add(neighbor);
            neighbor.blockGroup = group;
            CheckNeighborsForGroup(ref group, neighbor);
        }
    }

    private void OnDrawGizmos()
    {
        if (!exampleLevel)
            return;
        if (_grid == null)
        {
            CreateGrid();
        }

        Gizmos.color = Color.yellow;

        for (var x = 0; x < exampleLevel.columnCount; x++)
        {
            for (var y = 0; y < exampleLevel.rowCount; y++)
            {
                var point = _grid.CalculateCellPosition(x, y, exampleLevel.columnCount, gridOrigin);
                Gizmos.DrawWireCube(point, Spacing * Vector3.one);
            }
        }
    }
}

public enum GameState
{
    Idle,
    Blasting,
    Falling,
    Spawning
}