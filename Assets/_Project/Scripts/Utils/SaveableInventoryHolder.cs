using System.Collections.Generic;
using System.Linq;
using Game;
using Systems.Persistence;
using UnityEngine;

namespace UtilsModule {
    public class SaveableInventoryHolder : InventoryHolder, IBind<InventoryData> {
        InventoryData inventoryData;

        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        void Update() {
            if (inventoryData?.items != null) {
                List<SavedItem> savedItems = new List<SavedItem>();
                foreach (Item item in Inventory.items) {
                    savedItems.Add(new SavedItem(item));
                }

                inventoryData.items = savedItems.ToArray();
            }
        }

        public void Bind(InventoryData data) {
            inventoryData = data;
            inventoryData.Id = Id;

            Inventory = new Inventory(inventorySize, inventoryChannel);
            
            if(targetEntity) {
                Inventory.targetEntity = targetEntity;
            }

            if(inventoryData.items.Length == 0) {
                inventoryData.items = new SavedItem[inventorySize];
                for(int i = 0; i < inventoryData.items.Length; i++) {
                    inventoryData.items[i] = new SavedItem(GameManager.Instance.emptyItem.GetItem());
                }
            }

            for(int i = 0; i < inventoryData.items.Length; i++) {
                Inventory.SetSlot(i, inventoryData.items[i].ToItem(), inventoryData.items[i].count);
                if(Inventory.IsEquipment) AdditionalDataManager.Instance.OnEquip(inventoryData.items[i].ToItem(), Inventory.targetEntity);
            }

            inventoryChannel?.Invoke(Inventory);
        }
    }
}