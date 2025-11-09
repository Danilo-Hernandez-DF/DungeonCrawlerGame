using System.Linq;
using Systems.Persistence;
using UnityEngine;

namespace UtilsModule {
    public class InventoryHolder : Entity {
        public int inventorySize = 10;
        public InventoryEventChannel inventoryChannel;
        public EventChannel lootUIChannel;
        public Inventory Inventory;
        
        [SerializeField] protected Entity targetEntity;
        public InventoryGenerator inventoryGenrator;

        [Header("Loot Generation")]
        [SerializeField] protected bool generateOnAwake = true;

        protected void Awake() {
            Inventory = new Inventory(inventorySize, inventoryChannel);

            if(targetEntity) {
                  Inventory.targetEntity = targetEntity;
            }

            if(generateOnAwake) {
                 inventoryGenrator?.Generate();
            }

            inventoryChannel?.Invoke(Inventory);
        }

        public virtual void Generate(ItemLootTable lootTable, int rolls)
        {
            if(inventoryGenrator == null) return;

            Inventory.Clear();
            inventoryGenrator.Init(lootTable, rolls);
            inventoryGenrator.Generate();
        }

        public virtual void OnGenerate() { }
    }
}