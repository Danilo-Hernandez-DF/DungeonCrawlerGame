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
        [SerializeField] protected InventoryGenrator inventoryGenrator;

        [Header("Loot Generation")]
        [SerializeField] protected bool generateOnAwake = true;

        protected override void Awake() {
            base.Awake();
            Inventory = new Inventory(inventorySize, inventoryChannel);

            if(targetEntity) {
                  Inventory.targetEntity = targetEntity;
            }

            if(generateOnAwake) {
                 inventoryGenrator?.Generate();
            }

            inventoryChannel?.Invoke(Inventory);
        }

        public virtual void OnGenerate() { }
    }
}