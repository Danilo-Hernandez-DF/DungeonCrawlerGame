using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule {
    public class FilteredInventoryHolder : Entity {
        [SerializeField] protected List<SlotFilter> slotFilters;
        [SerializeField] protected FilteredInventoryEventChannel inventoryChannel;
        protected FilteredInventory Inventory;
        
        [SerializeField] protected Entity targetEntity;

        protected override void Awake() {
            Inventory = new FilteredInventory(slotFilters, inventoryChannel);
            if(targetEntity) {
                Inventory.targetEntity = targetEntity;
            }
            GetComponent<InventoryGenerator>()?.Generate();
            inventoryChannel?.Invoke(Inventory);
        }
    }
}