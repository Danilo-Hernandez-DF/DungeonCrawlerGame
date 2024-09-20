using UnityEngine;

namespace UtilsModule {
    public class InventoryHolder : MonoBehaviour { 
        [SerializeField] int inventorySize = 10;
        [SerializeField] InventoryEventChannel inventoryChannel;
        public Inventory inventory {get; private set;}

        void Start() {
            inventory = new Inventory(inventorySize, inventoryChannel);
            inventoryChannel?.Invoke(inventory);
        }
    }
}