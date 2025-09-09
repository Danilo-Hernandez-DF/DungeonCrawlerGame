using UtilsModule;

namespace Game {
    public class ItemDatabase : SODatabase<ItemData> {
        public Item GetEquipment(Tag.Equipment type) {
            return Items.Find(item => item.HasTag("tag_equipment", (float)type, true)).GetItem();
        }

        public override ItemData GetData(string name) {
            return Items.Find(item => item.nameKey == name);
        }
    }
}