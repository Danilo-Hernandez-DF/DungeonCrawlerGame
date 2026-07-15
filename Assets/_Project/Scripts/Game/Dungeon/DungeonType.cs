using ProcGen;
using Localisation;
using UnityEngine.Tilemaps;
using Utils;

namespace Game {
    [CreateAssetMenu(fileName = "DungeonType", menuName = "Dungeon Type")]
    public class DungeonType : ScriptableObject
    {
        public Sprite icon;
        public string sceneName;
        public string dungeonName;
        public new string name => LocalisationSystem.GetLocalisedValue(dungeonName);
        public int difficultyLevel;
        public GoWeightedList startRoomPrefab;
        public GoWeightedList roomPrefabs;
        public TileBaseWeightedList tilePrefabs;
        public List<EntityData> spawnableEntities;
        public List<RoomRequirements> roomRequirements;
        public DungeonDataWeightedList variants;
        public List<DungeonModifier> modifiers;
        
        public TileBase GetTile() {
            return tilePrefabs.GetWeightedItem();
        }
        
        public List<ItemData> GetItems() {
            return variants.GetItems();
        }
    }
}