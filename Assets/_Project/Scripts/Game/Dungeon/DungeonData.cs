using Localisation;
using ProcGen;

namespace Game {
    [CreateAssetMenu(fileName = "DungeonData", menuName = "Data/Dungeon Data")]
    public class DungeonData : ScriptableObject {
        public string dungeonName => LocalisationSystem.GetLocalisedValue(nameKey);
        public string nameKey;
        public DungeonType dungeonType;
        //public DungeonModifier[] modifiers;
        public int roomCount;
        public Quest completionQuest;
        //public Quest[] additionalQuests;
        public List<AdditionalRoomRequirements> roomsPerLevel;
        public LootSetting[] lootSettings; 
        [SerializeField] private DungeonDataWeightedList nextLevel;
        
        public DungeonData NextLevel() => nextLevel?.GetWeightedItem();
        
        public List<ItemData> AvailableItems() {
            List<ItemData> items = new List<ItemData>();
            
            foreach(var setting in lootSettings) {
                foreach (var item in setting.weightedList.GetAllItems()) {
                    if(items.Exists(x => x == item.data)) continue;
                    items.Add(item.data);
                }
            }
            
            return items;
        }

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
            
            List<ItemData> includedItems = new List<ItemData>();
            
            foreach (var item in AvailableItems()) {
                if(includedItems.Contains(item)) continue;
                includedItems.Add(item);
                lootLevel += item.rarity;
                count++;
            }

            if (count == 0) return 0;
            return lootLevel / count;
        }
    }
}