using ProcGen;
using UnityEngine;

namespace Game {
    [CreateAssetMenu(fileName = "DungeonData", menuName = "Data/Dungeon Data")]
    public class DungeonData : ScriptableObject {
        public string dungeonName;
        public DungeonType dungeonType;
        public DungeonModifier[] modifiers;
        public int maxLevel;
        public int roomCount;
        public Quest completionQuest;
        public Quest[] additionalQuests;
        public RoomRequirements[] roomsPerLevel;
        public LootSetting[] lootSettings;
    }
}