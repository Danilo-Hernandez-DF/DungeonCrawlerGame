using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule {
    public class FilteredInventoryHolder : MonoBehaviour {
        [SerializeField] protected List<SlotFilter> slotFilters;
        [SerializeField] protected FilteredInventoryEventChannel inventoryChannel;
        protected FilteredInventory Inventory;
        
        [SerializeField] protected Entity targetEntity;

        void Awake() {
            Inventory = new FilteredInventory(slotFilters, inventoryChannel);
            if(targetEntity) {
                Inventory.targetEntity = targetEntity;
            }
            GetComponent<InventoryGenrator>()?.Generate();
            inventoryChannel?.Invoke(Inventory);
        }
    }
}