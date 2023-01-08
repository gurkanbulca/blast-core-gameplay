using UnityEngine;
using BlockSystem;

namespace GridSystem
{
    public class Cell
    {
        #region Public Fields

        public readonly Vector3 Position;
        public readonly Vector2Int GridCoordinate;
        public Block Block;

        #endregion

        #region Constructor

        public Cell(Vector3 position, Vector2Int coordinate)
        {
            Position = position;
            GridCoordinate = coordinate;
        }

        #endregion
    }
}