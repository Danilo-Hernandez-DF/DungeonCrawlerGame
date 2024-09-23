using UnityEngine;

namespace UtilsModule {
    public class InventoryHolder : MonoBehaviour { 
        [SerializeField] int inventorySize = 10;
        [SerializeField] protected InventoryEventChannel inventoryChannel;
        public Inventory inventory {get; private set;}

        void Awake() {
            inventory = new Inventory(inventorySize, inventoryChannel);
            if(GetComponent<InventoryGenrator>() != null) {
                GetComponent<InventoryGenrator>().Generate();
            }
            inventoryChannel?.Invoke(inventory);
        }
    }
}