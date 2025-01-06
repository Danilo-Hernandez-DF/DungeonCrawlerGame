using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule {
    public class FilteredInventoryHolder : MonoBehaviour {
        [SerializeField] protected List<SlotFilter> slotFilters;
        [SerializeField] protected FilteredInventoryEventChannel inventoryChannel;
        public int InventorySize => slotFilters.Count;
        public FilteredInventory Inventory;
        public Inventory HeldInventory => Inventory;

        void Awake() {
            Inventory = new FilteredInventory(slotFilters, inventoryChannel);
            GetComponent<InventoryGenrator>()?.Generate();
            inventoryChannel?.Invoke(Inventory);
        }
    }
}