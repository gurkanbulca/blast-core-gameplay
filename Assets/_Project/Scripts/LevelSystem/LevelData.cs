using Sirenix.OdinInspector;
using UnityEngine;

namespace LevelSystem
{
    [CreateAssetMenu(menuName = "Level Data", fileName = "Level 1")]

    public class LevelData : ScriptableObject
    {
        [Range(2, 10)] public int rowCount = 2;
        [Range(2, 10)] public int columnCount = 2;
        [Range(1, 6)] public int colorCount = 1;

        [MinValue(1)] public int firstIconCondition = 1;
        [MinValue("greaterThanFirst")] public int secondIconCondition = 2;
        [MinValue("greaterThanSecond")] public int thirdIconCondition = 3;

        private int greaterThanFirst => firstIconCondition + 1;
        private int greaterThanSecond => secondIconCondition + 1;
    }
}