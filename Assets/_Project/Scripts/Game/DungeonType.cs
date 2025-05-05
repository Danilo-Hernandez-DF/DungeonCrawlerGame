using UnityEngine;

namespace Game {
    [CreateAssetMenu(fileName = "DungeonType", menuName = "Dungeon Type")]
    public class DungeonType : ScriptableObject{
        public string dungeonName;
        public Sprite sprite;
        public int difficultyLevel;
    }
}