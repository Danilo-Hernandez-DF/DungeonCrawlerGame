using System.Collections.Generic;
using System.Linq;
using Localisation;
using ProcGen;
using UnityEngine;
using UtilsModule;

namespace Game {
    [CreateAssetMenu(fileName = "DungeonData", menuName = "Data/Dungeon Data")]
    public class DungeonData : ScriptableObject {
        public string dungeonName => LocalisationSystem.GetLocalisedValue(nameKey);
        public string sceneName;
        public Sprite icon;
        public string nameKey;
        public DungeonType dungeonType;
        public DungeonModifier[] modifiers;
        public int roomCount;
        public Quest completionQuest;
        //public Quest[] additionalQuests;
        public List<AdditionalRoomRequirements> roomsPerLevel;
        public LootSetting[] lootSettings; 
        public DungeonData nextLevel;

        public List<RoomRequirements> RoomRequirements() {
            List<RoomRequirements> requirements = new List<RoomRequirements>();
            
            foreach(var requirement in dungeonType.roomRequirements) {
                if (roomsPerLevel.Exists(x => x.type == requirement.type)) {
                    requirements.Add(new RoomRequirements(requirement.roomPrefabs, requirement.type, 
                        roomsPerLevel.Find(x => x.type == requirement.type).count + requirement.count));
                }
                else {
                    requirements.Add(requirement);
                }
            }
            
            return requirements;
        }

        public int LootLevel() {
            int lootLevel = 0;
            int count = 0;
            
            List<Item> includedItems = new List<Item>();
            
            foreach (var setting in lootSettings) {
                foreach (var item in setting.lootTable.GetAllItems()) {
                    if(includedItems.Exists(x => x.Matches(item))) continue;
                    includedItems.Add(item);
                    lootLevel += item.GetRarity();
                    count++;
                }
            }

            if (count == 0) return 0;
            return lootLevel / count;
        }
    }
}