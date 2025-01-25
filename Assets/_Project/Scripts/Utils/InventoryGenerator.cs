using UnityEngine;

namespace UtilsModule {
    [RequireComponent(typeof(IInventoryHolder))]
    public class InventoryGenrator : MonoBehaviour {
        [SerializeField] InventoryHolder inventoryHolder;
        [SerializeField] LootTable<Item> lootTable;
        [SerializeField] int rolls = 2;
        [SerializeField] bool isSeeded = false;
        private Inventory Inventory => inventoryHolder.Inventory;

        public void Generate() {
            for(int i = 0; i < rolls; i++) {
                var toAdd = lootTable.GetWeightedItem(seeded: isSeeded);
                Inventory.SetAtRandom(toAdd, toAdd.count, true);
            }

            //Debug.Log(inventory.items.Count);

            /*foreach(var i in Inventory.items) {
                string tagString = "";
                for(int j = 0; j < i.tags.Count; j++) {
                    tagString += i.tags[j].ToString() + (j == i.tags.Count - 1 ? "" : ", ");
                }

                Debug.Log($"{i.data.name}: [Tags: [{tagString}], Count: {i.count}]");
            }*/
        }

        public void Init(LootTable<Item> lootTable, int rolls) {
            this.lootTable = lootTable;
            this.rolls = rolls;
        }
    }
}