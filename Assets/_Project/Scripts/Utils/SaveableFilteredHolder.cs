using System.Collections.Generic;
using Systems.Persistence;
using UnityEngine;

namespace UtilsModule {
    public class SaveableFilteredHolder : FilteredInventoryHolder, IBind<InventoryData> {
        InventoryData inventoryData;

        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        void Update() {
            inventoryData.Items = inventory.items.ToArray();
        }

        public void Bind(InventoryData data) {
            inventoryData = data;
            inventoryData.Id = Id;

            inventory = new FilteredInventory(slotFilters, inventoryChannel);

            for(int i = 0; i < inventoryData.Items.Length; i++) {
                inventory.SetSlot(i, inventoryData.Items[i], inventoryData.Items[i].count, true);
            }

            inventoryChannel?.Invoke(inventory);
        }
    }
}