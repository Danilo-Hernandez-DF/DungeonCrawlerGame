using System.Collections.Generic;
using Game;
using UnityEngine;

namespace UtilsModule {
    public class LootingInventoryUI : UIBase {
        [SerializeField] InventoryUI playerInventory;
        [SerializeField] InventoryUI otherInventory;
        [SerializeField] ItemEventChannel itemDisplayChannel;

        public override void Open() {
            if(GameManager.Instance.openUI != null) return;
            OpenChildrenEvent?.Invoke();
            startUI = playerInventory.StartUI;
            base.Open();

            itemDisplayChannel?.Invoke(null);
        }

        public override void Close() {
            if(GameManager.Instance.openUI != handledUI) return;

            if(!InventorySlotUI.heldItem?.IsEmpty ?? false) {
                if(playerInventory.TargetInventory.AvailableCount(InventorySlotUI.heldItem) < InventorySlotUI.heldItem.count) {
                    otherInventory.TargetInventory.Add(InventorySlotUI.heldItem, InventorySlotUI.heldItem.count);
                } else {
                    playerInventory.TargetInventory.Add(InventorySlotUI.heldItem, InventorySlotUI.heldItem.count);
                }
                
                InventorySlotUI.heldItem = null;
            }

            CloseChildrenEvent?.Invoke();

            base.Close();
        }
    }
}