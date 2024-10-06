using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule {
    public class FilteredInventoryHolder : MonoBehaviour, IInventoryHolder {
        [SerializeField] List<SlotFilter> slotFilters;
        [SerializeField] protected InventoryEventChannel inventoryChannel;
        public int InventorySize => slotFilters.Count;
        public FilteredInventory inventory;
        public Inventory heldInventory => inventory;

        void Awake() {
            inventory = new FilteredInventory(slotFilters, inventoryChannel);
            if(GetComponent<InventoryGenrator>() != null) {
                GetComponent<InventoryGenrator>().Generate();
            }
            inventoryChannel?.Invoke(inventory);
        }
    }
}