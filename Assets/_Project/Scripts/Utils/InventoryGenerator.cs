using System;
using UnityEngine;

namespace UtilsModule {
    public class InventoryGenerator : MonoBehaviour {
        [SerializeField] InventoryHolder inventoryHolder;
        public ItemLootTable lootTable;
        [SerializeField] int rolls = 2;
        [SerializeField] bool isSeeded;
        [SerializeField] bool prioritizeEmpty;
        
        [Header("Fixed Drops")]
        [SerializeField] FixedDrop[] fixedDrops;
        private Inventory Inventory => inventoryHolder.Inventory;

        public void Generate() {
            if(prioritizeEmpty) {
                for(int i = 0; i < rolls; i++) {
                    var toAdd = lootTable.GetWeightedItem();
                    if(Inventory.EmptySlots() == 0) {
                        inventoryHolder.OnGenerate();
                        return;
                    }
                    Inventory.SetSlot(Inventory.NextEmpty(), toAdd, toAdd.count, true);
                }

                foreach(FixedDrop drop in fixedDrops) {
                    for (int i = 0; i < drop.count; i++) {
                        var toAdd = drop.table.GetWeightedItem();
                        if(Inventory.EmptySlots() == 0) {
                            inventoryHolder.OnGenerate();
                            return;
                        }
                        Inventory.SetSlot(Inventory.NextEmpty(), toAdd, toAdd.count, true);
                    }
                }

                inventoryHolder.OnGenerate();
                return;
            }

            for(int i = 0; i < rolls; i++) {
                var toAdd = lootTable.GetWeightedItem();
                Inventory.SetAtRandom(toAdd, toAdd.count);
            }

            foreach(FixedDrop drop in fixedDrops) {
                for (int i = 0; i < drop.count; i++) {
                    var toAdd = drop.table.GetWeightedItem();
                    Inventory.SetAtRandom(toAdd, toAdd.count);
                }
            }

            inventoryHolder.OnGenerate();
        }

        public void Init(ItemLootTable lootTable, int rolls) {
            this.lootTable = lootTable;
            this.rolls = rolls;
        }
    }
    
    [Serializable]
    public struct FixedDrop
    {
        public LootTable<Item> table;
        public int count;
    }
}