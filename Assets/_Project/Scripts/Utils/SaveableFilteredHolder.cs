using System.Collections.Generic;
using Systems.Persistence;
using UnityEngine;

namespace UtilsModule {
    public class SaveableFilteredHolder : FilteredInventoryHolder, IBind<InventoryData> {
        InventoryData inventoryData;

        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        void Update() {
            if(inventoryData?.items != null) inventoryData.items = Inventory.items.ToArray();
        }

        public void Bind(InventoryData data) {
            inventoryData = data;
            inventoryData.Id = Id;

            Inventory = new FilteredInventory(slotFilters, inventoryChannel);
            
            if(targetEntity) {
                Inventory.targetEntity = targetEntity;
            }

            for(int i = 0; i < inventoryData.items.Length; i++) {
                Inventory.SetSlot(i, inventoryData.items[i], inventoryData.items[i].count, true);
                if(Inventory.IsEquipment) AdditionalDataManager.Instance.OnEquip(inventoryData.items[i], Inventory.targetEntity);
            }

            inventoryChannel?.Invoke(Inventory);
        }
    }
}