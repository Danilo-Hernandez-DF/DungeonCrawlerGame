using System.Collections.Generic;
using UtilsModule;

namespace Game {
    public class ItemDatabase {
        private readonly List<ItemData> Items = new();

        public Item GetEquipment(Tag.Equipment type) {
            return Items.Find(item => item.HasTag("Equipment", (float)type, true)).GetItem();
        }

        public ItemData GetItemData(string name) {
            return Items.Find(item => item.name == name);
        }

        public void AddItem(ItemData item) {
            Items.Add(item);
        }
    }
}