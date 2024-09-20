using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Unity.VisualScripting;
using UnityEngine;

namespace UtilsModule {
    public class InventoryUI : UIBase {
        [Header("InvUI")]
        [SerializeField] int maxDisplaySlots = 36;
        [SerializeField] int columns = 6;
        [SerializeField] protected GameObject slotPrefab;
        [SerializeField] Transform slotHolder;
        [SerializeField] IntEventChannel slotIndexChannel;
        [SerializeField] ItemEventChannel itemDisplayChannel;
        List<InventorySlotUI> slots;
        Inventory targetInventory;
        int rows => Mathf.CeilToInt((float)maxDisplaySlots / columns);
        int page = 1;
        int pageOffset => (page - 1) * maxDisplaySlots;
        int PageSize => targetInventory.Size - pageOffset > maxDisplaySlots ? maxDisplaySlots : targetInventory.Size - pageOffset;

        void Awake() {
            Populate();
        }

        public override void Open() {
            if(targetInventory == null) return;
            if(GameManager.Instance.openUI != null) return;
            page = 1;

            startUI = slots[0].gameObject;
            GameManager.Instance.itemDisplayUI.SetActive(true);
            itemDisplayChannel?.Invoke(null);
            base.Open();
        }

        public override void Close() {
            if(GameManager.Instance.openUI != handledUI) return;
            GameManager.Instance.itemDisplayUI.SetActive(false);
            foreach(var slot in slots) slot.Clear();
            if(InventorySlotUI.heldItem != null) {
                targetInventory.Add(InventorySlotUI.heldItem, InventorySlotUI.heldItem.count);
                InventorySlotUI.heldItem = null;
            }
            base.Close();
        }

        void Populate() {
            slots = new List<InventorySlotUI>();
                
            for(int i = 0; i < maxDisplaySlots; i++) {
                var newSlot = Instantiate(slotPrefab);
                newSlot.transform.SetParent(slotHolder, false);
                var slotComp = newSlot.GetComponent<InventorySlotUI>();
                slotComp.Init(i, slotIndexChannel);
                slots.Add(slotComp);
            }
        }

        public void OnUpdate(Inventory inventory) {
            Debug.Log(slots.Count);
            if(inventory != targetInventory) targetInventory = inventory;

                if(maxDisplaySlots > targetInventory.Size) {
                    for(int i = 0; i < maxDisplaySlots; i++) {
                        if(i < targetInventory.Size) slots[i].gameObject.SetActive(true);
                        else slots[i].gameObject.SetActive(false);
                    }
                }
            
                for(int i = 0; i < slots.Count; i++) {
                    if(i < PageSize) {
                        slots[i].gameObject.SetActive(true);
                        slots[i].Refresh(pageOffset);
                        slots[i].OnUpdate(targetInventory);
                    } 
                    else slots[i].gameObject.SetActive(false);
                }
        }

        public void OnSlotPressed(int index) {
            if(index < 0 || index >= targetInventory.Size) return;
            if(InventorySlotUI.heldItem?.IsEmpty ?? true) {
                if(targetInventory.items[index + pageOffset].IsEmpty) return;
                InventorySlotUI.heldItem = targetInventory.items[index + pageOffset].Copy();
                targetInventory.SetSlot(index + pageOffset, new Item(GameManager.Instance.EmptyItem), 0);
            } else if(InventorySlotUI.heldItem.Matches(targetInventory.items[index + pageOffset])) {
                int remainder = targetInventory.AddAt(index + pageOffset, InventorySlotUI.heldItem, InventorySlotUI.heldItem.count);
                if(remainder > 0) InventorySlotUI.heldItem.count = remainder;
                else InventorySlotUI.heldItem = new Item(GameManager.Instance.EmptyItem);
            } else {
                var tempItem = InventorySlotUI.heldItem.Copy();
                InventorySlotUI.heldItem = targetInventory.items[index + pageOffset].Copy();
                targetInventory.SetSlot(index + pageOffset, tempItem, tempItem.count);
            }

            OnUpdate(targetInventory);
        }
    }
}