using System;
using UnityEngine;

namespace UtilsModule {
    //[RequireComponent(typeof(IInventoryHolder))]
    public class InventoryGenerator : MonoBehaviour {
        [SerializeField] InventoryHolder inventoryHolder;
        [SerializeField] LootTable<Item> lootTable;
        [SerializeField] int rolls = 2;
        [SerializeField] bool isSeeded;
        [SerializeField] bool prioritizeEmpty;
        
        [Header("Fixed Drops")]
        [SerializeField] FixedDrop[] fixedDrops;
        private Inventory Inventory => inventoryHolder.Inventory;

        public void Generate() {
            if(prioritizeEmpty) {
                for(int i = 0; i < rolls; i++) {
                    var toAdd = lootTable.GetWeightedItem(seeded: isSeeded);
                    if(Inventory.EmptySlots() == 0) {
                        inventoryHolder.OnGenerate();
                        return;
                    }
                    Inventory.SetSlot(Inventory.NextEmpty(), toAdd, toAdd.count, true);
                }

                foreach(FixedDrop drop in fixedDrops) {
                    for (int i = 0; i < drop.count; i++) {
                        var toAdd = drop.table.GetWeightedItem(seeded: isSeeded);
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
                var toAdd = lootTable.GetWeightedItem(seeded: isSeeded);
                Inventory.SetAtRandom(toAdd, toAdd.count, true);
            }

            foreach(FixedDrop drop in fixedDrops) {
                for (int i = 0; i < drop.count; i++) {
                    var toAdd = drop.table.GetWeightedItem(seeded: isSeeded);
                    Inventory.SetAtRandom(toAdd, toAdd.count, true);
                }
            }

            inventoryHolder.OnGenerate();
        }

        public void Init(LootTable<Item> lootTable, int rolls) {
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