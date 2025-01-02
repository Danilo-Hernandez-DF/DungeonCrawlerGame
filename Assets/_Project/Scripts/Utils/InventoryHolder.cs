using System.Linq;
using Systems.Persistence;
using UnityEngine;

namespace UtilsModule {
    public class InventoryHolder : MonoBehaviour {
        [SerializeField] protected int inventorySize = 10;
        [SerializeField] protected InventoryEventChannel inventoryChannel;
        protected Inventory inventory;
        public Inventory inventory_ => inventory;

        void Awake() {
            inventory = new Inventory(inventorySize, inventoryChannel);
            GetComponent<InventoryGenrator>()?.Generate();
            inventoryChannel?.Invoke(inventory);
        }
    }
}