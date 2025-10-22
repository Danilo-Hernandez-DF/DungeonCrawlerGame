using UtilsModule;

namespace Game {
    [System.Serializable]
    public class LootSetting {
        public LootType lootType;
        public LootTable<Item> lootTable;
        public int rolls; 
    }
}