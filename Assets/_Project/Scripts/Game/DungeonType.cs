using System.Collections.Generic;
using ProcGen;
using UnityEngine;
using UtilsModule;
using System.Linq;
using Localisation;
using UnityEngine.Tilemaps;

namespace Game {
    [CreateAssetMenu(fileName = "DungeonType", menuName = "Dungeon Type")]
    public class DungeonType : ScriptableObject
    {
        public Sprite icon;
        public string sceneName;
        public string dungeonName;
        public new string name => LocalisationSystem.GetLocalisedValue(dungeonName);
        public Sprite sprite;
        public int difficultyLevel;
        public GOLootTable startRoomPrefab;
        public GOLootTable roomPrefabs;
        public TileBaseLootTable tilePrefabs;
        public List<EntityData> spawnableEntities;
        public List<RoomRequirements> roomRequirements;
        public DungeonDataLootTable variants;
        
        public TileBase GetTile() {
            return tilePrefabs.GetWeightedItem();
        }
    }
}