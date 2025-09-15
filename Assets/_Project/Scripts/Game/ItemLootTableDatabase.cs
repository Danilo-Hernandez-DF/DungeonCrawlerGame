using UtilsModule;

namespace Game {
    internal class ItemLootTableDatabase : SODatabase<ItemLootTable>
    {
        public override ItemLootTable GetData(string name) {
            if (string.IsNullOrEmpty(name)) {
                return null;
            }
            return Items.Find(item => item.nameKey == name);
        }
    }
}