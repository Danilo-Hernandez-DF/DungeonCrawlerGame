using UtilsModule;

namespace Game {
    public class ItemDatabase : SODatabase<ItemData> {
        public Item GetEquipment(Tag.Equipment type) {
            return Items.Find(item => item.HasTag("Equipment", (float)type, true)).GetItem();
        }

        public override ItemData GetData(string name) {
            return Items.Find(item => item.name == name);
        }

        public override ItemData GetData(SerializableGuid id) {
            return Items.Find(item => item.id == id);
        }
    }
}