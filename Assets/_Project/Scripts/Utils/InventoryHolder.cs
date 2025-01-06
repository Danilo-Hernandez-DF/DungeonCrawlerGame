using System.Linq;
using Systems.Persistence;
using UnityEngine;

namespace UtilsModule {
    public class InventoryHolder : MonoBehaviour {
        [SerializeField] protected int inventorySize = 10;
        [SerializeField] protected InventoryEventChannel inventoryChannel;
        public Inventory Inventory;

        void Awake() {
            Inventory = new Inventory(inventorySize, inventoryChannel);
            GetComponent<InventoryGenrator>()?.Generate();
            inventoryChannel?.Invoke(Inventory);
        }
    }
}