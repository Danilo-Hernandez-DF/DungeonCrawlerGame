using UtilsModule;

namespace Game {
    [System.Serializable]
    public class LootSetting {
        public LootType lootType;
        public ItemLootTable lootTable;
        public int rolls; 
    }
}