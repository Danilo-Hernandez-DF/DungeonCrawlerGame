using System.Linq;
using Systems.Persistence;
using UnityEngine;

namespace UtilsModule {
    public class SaveableInventoryHolder : InventoryHolder, IBind<InventoryData> {
        InventoryData inventoryData;

        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        void Update() {
            inventoryData.Items = inventory.items.ToArray();
        }

        public void Bind(InventoryData data) {
            inventoryData = data;
            inventoryData.Id = Id;

            inventory = new Inventory(inventorySize, inventoryChannel);

            for(int i = 0; i < inventoryData.Items.Length; i++) {
                inventory.SetSlot(i, inventoryData.Items[i], inventoryData.Items[i].count);
            }

            inventoryChannel?.Invoke(inventory);
        }
    }
}