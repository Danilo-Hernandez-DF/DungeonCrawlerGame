using Utils;

namespace Game {
    internal class ItemLootTableDatabase : SODatabase<ItemWeightedList>
    {
        public override ItemWeightedList GetData(string name) {
            if (string.IsNullOrEmpty(name)) {
                return null;
            }
            return Items.Find(item => item.nameKey == name);
        }
    }
}