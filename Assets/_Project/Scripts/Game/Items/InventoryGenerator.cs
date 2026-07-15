using UnityEngine.Serialization;
using Utils;

namespace Game {
    public class InventoryGenerator : MonoBehaviour {
        [SerializeField] InventoryHolder inventoryHolder;
        [FormerlySerializedAs("lootTable")] public ItemWeightedList weightedList;
        [SerializeField] int rolls = 2;
        [SerializeField] bool isSeeded;
        [SerializeField] bool prioritizeEmpty;
        
        [Header("Fixed Drops")]
        [SerializeField] FixedDrop[] fixedDrops;
        private Inventory Inventory => inventoryHolder.Inventory;

        public void Generate() {
            if(prioritizeEmpty) {
                for(int i = 0; i < rolls; i++) {
                    var toAdd = weightedList.GetWeightedItem();
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
                var toAdd = weightedList.GetWeightedItem();
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

        public void Init(ItemWeightedList weightedList, int rolls) {
            this.weightedList = weightedList;
            this.rolls = rolls;
        }
    }
    
    [Serializable]
    public struct FixedDrop
    {
        public WeightedList<Item> table;
        public int count;
    }
}