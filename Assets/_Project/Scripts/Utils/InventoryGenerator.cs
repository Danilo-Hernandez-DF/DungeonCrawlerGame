using KBCore.Refs;
using Unity.Collections;
using UnityEngine;

namespace UtilsModule {
    [RequireComponent(typeof(InventoryHolder))]
    public class InventoryGenrator : ValidatedMonoBehaviour {
        [SerializeField, Self] InventoryHolder inventoryHolder;
        [SerializeField] LootTable lootTable;
        [SerializeField] int rolls = 2;
        public Inventory inventory => inventoryHolder.inventory;

        public void Generate() {
            for(int i = 0; i < rolls; i++) {
                var toAdd = lootTable.GetWeightedItem();
                inventory.SetAtRandom(toAdd, toAdd.count, true);
            }

            Debug.Log(inventory.items.Count);

            foreach(var i in inventory.items) {
                string tagString = "";
                for(int j = 0; j < i.tags.Count; j++) {
                    tagString += i.tags[j].ToString() + (j == i.tags.Count - 1 ? "" : ", ");
                }

                Debug.Log($"{i.data.name}: [Tags: [{tagString}], Count: {i.count}]");
            }
        }

        public void Init(LootTable lootTable, int rolls) {
            this.lootTable = lootTable;
            this.rolls = rolls;
        }
    }
}