using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;

namespace UtilsModule {
    public class PlayerInventoryUI: UIBase { 
        [SerializeField] private InventoryUI inventory;
        [SerializeField] private InventoryUI equipmentUI;
        
        public InventoryUI Inventory => inventory;
        public InventoryUI Equipment => equipmentUI;

        protected override void Awake() {
            base.Awake();
            //equipmentUI.targetEntity = PlayerDetector.GetPlayer().GetComponent<PlayerController>();

            children = new List<UIBase> {
                inventory,
                equipmentUI
            };

            foreach(UIBase child in children) {
                child.SetParent(this);
            }
        }
        
        protected override void OnClose() {
            if(InventorySlotUI.HeldItemEmpty) return;
            if(inventory.TargetInventory.AvailableCount(InventorySlotUI.heldItem) < InventorySlotUI.heldItem.count) {
                equipmentUI.TargetInventory.Add(InventorySlotUI.heldItem, InventorySlotUI.heldItem.count);
            } else {
                inventory.TargetInventory.Add(InventorySlotUI.heldItem, InventorySlotUI.heldItem.count);
            }
                
            InventorySlotUI.heldItem = null;
        }

        protected new void OnEnable() {
            GameManager.Instance.input.OpenMenu += Open;
            base.OnEnable();
        }

        protected new void OnDisable() {
            GameManager.Instance.input.OpenMenu -= Open;
            base.OnDisable();
        }
    }
}