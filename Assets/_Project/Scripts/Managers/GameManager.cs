using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameState initialGameState;
    [SerializeField] private LevelData exampleLevel;
    [SerializeField] private BlockFabricator blockFabricator;
    [SerializeField] private Vector3 gridOrigin;
    


    private Grid _grid;
    private List<Block[]> _groups;

    public GameState CurrentGameState { get; private set; }

    private void Awake()
    {
        _groups = new List<Block[]>();
        CurrentGameState = initialGameState;
    }
    
    private void Start()
    {
        LoadLevel(exampleLevel);
    }

    private void CreateGrid()
    {
        _grid = new Grid(new GridData(new Vector2Int(exampleLevel.rowCount, exampleLevel.colorCount),
            transform.position, .8f));
    }

    private void LoadLevel(LevelData levelData)
    {
        CreateGrid();
        _grid.GenerateGrid();
    }

    private void CheckForDeadLock()
    {
    }

    private void Shuffle()
    {
    }

    private void FindGroups()
    {
    }

    private void DropBlocks(Cell[,] cells)
    {
    }

    private void DropColumn(Cell[] cells)
    {
    }

    public Block[] FindGroupForBlock(Block block)
    {
        return null;
    }

    private void OnDrawGizmos()
    {
        if (_grid == null)
        {
            CreateGrid();
        }
    }
}

public enum GameState
{
    Idle,
    Blasting,
    Falling
}