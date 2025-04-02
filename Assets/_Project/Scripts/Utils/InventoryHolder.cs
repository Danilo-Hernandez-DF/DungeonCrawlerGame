using System.Linq;
using Systems.Persistence;
using UnityEngine;

namespace UtilsModule {
    public class InventoryHolder : Entity {
        [SerializeField] protected int inventorySize = 10;
        public InventoryEventChannel inventoryChannel;
        public EventChannel lootUIChannel;
        public Inventory Inventory;
        
        [SerializeField] protected Entity targetEntity;

        protected override void Awake() {
            base.Awake();
            Inventory = new Inventory(inventorySize, inventoryChannel);
            if(targetEntity) {
                  Inventory.targetEntity = targetEntity;
            }
            GetComponent<InventoryGenrator>()?.Generate();
            inventoryChannel?.Invoke(Inventory);
        }
    }
}