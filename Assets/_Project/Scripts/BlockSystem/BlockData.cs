using UnityEngine;

namespace BlockSystem
{
    [CreateAssetMenu(fileName = "New Block Data", menuName = "Block Data")]

    public class BlockData : ScriptableObject
    {
        public Sprite[] icons;
    }
}