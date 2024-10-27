using System;
using System.Collections.Generic;
using Game;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UtilsModule {
    public class InventoryUI : UIBase {
        [Header("InvUI")]
        [SerializeField] int columns = 6;
        [SerializeField] int rows = 6;
        [SerializeField] protected GameObject slotPrefab;
        [SerializeField] Transform slotHolder;
        [SerializeField] IntEventChannel slotIndexChannel;
        [SerializeField] ItemEventChannel itemDisplayChannel;
        [SerializeField] UIBase parent;
        [SerializeField] public Entity targetEntity;
        bool isEquipment => targetEntity != null;
        bool isChild => parent != null;
        public int maxDisplaySlots => columns * rows;
        List<InventorySlotUI> slots;
        Inventory targetInventory;
        public Inventory TargetInventory => targetInventory;
        int page = 1;
        int pageCount => Mathf.CeilToInt((float)targetInventory.Size / maxDisplaySlots);
        int pageOffset => (page - 1) * maxDisplaySlots;
        int PageSize => targetInventory.Size - pageOffset > maxDisplaySlots ? maxDisplaySlots : targetInventory.Size - pageOffset;

        void Awake() {
            Populate();
        }

        protected virtual void Start() {
            if(isChild) {
                parent.OpenChildrenEvent += Open;
                parent.CloseChildrenEvent += Close;
            }
        }

        public override void Open() {
            if(targetInventory == null) return;
            if(GameManager.Instance.openUI != null) return;
            page = 1;

            startUI = slots[0].gameObject;

            if(!isChild) {
                OpenChildrenEvent?.Invoke();
                itemDisplayChannel?.Invoke(null);
                base.Open();
            } else {
                open = true;
            }

            OnUpdate(targetInventory);
        }

        public override void Close() {
            if(GameManager.Instance.openUI != handledUI) return;
            foreach(var slot in slots) slot.Clear();

            if(!isChild) {
                CloseChildrenEvent?.Invoke();
                if(!InventorySlotUI.heldItem?.IsEmpty ?? false) {
                    targetInventory.Add(InventorySlotUI.heldItem, InventorySlotUI.heldItem.count);
                    InventorySlotUI.heldItem = null;
                }
                base.Close();
            } else {
                open = false;
            }
        }

        void Populate() {
            GridLayoutGroup grid = slotHolder.GetComponent<GridLayoutGroup>();
            slots = new List<InventorySlotUI>();
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
                
            for(int i = 0; i < maxDisplaySlots; i++) {
                var newSlot = Instantiate(slotPrefab);
                newSlot.transform.SetParent(slotHolder, false);
                var slotComp = newSlot.GetComponent<InventorySlotUI>();
                slotComp.Init(i, slotIndexChannel);
                slots.Add(slotComp);
            }
        }

        public void OnUpdate(Inventory inventory) {
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
            if(!open) return;
            if(index < 0 || index >= targetInventory.Size) return;
            Debug.Log("Pressed: " + index);
            if(InventorySlotUI.heldItem?.IsEmpty ?? true) {
                Debug.Log("Empty held item");
                if(targetInventory.items[index + pageOffset].IsEmpty) return;
                InventorySlotUI.heldItem = targetInventory.items[index + pageOffset].Copy();
                targetInventory.ResetSlot(index + pageOffset);

                if(isEquipment) {
                    EquipmentManager.Instance.OnUnequip(InventorySlotUI.heldItem, targetEntity);
                }
            } else if(InventorySlotUI.heldItem.Matches(targetInventory.items[index + pageOffset])) {
                Debug.Log("Matching held item");
                int remainder = targetInventory.AddAt(index + pageOffset, InventorySlotUI.heldItem, InventorySlotUI.heldItem.count);
                if(remainder > 0) InventorySlotUI.heldItem.count = remainder;
                else InventorySlotUI.heldItem = new Item(GameManager.Instance.EmptyItem);
            } else {
                Debug.Log("Different held item");
                var tempItem = InventorySlotUI.heldItem.Copy();
                InventorySlotUI.heldItem = targetInventory.items[index + pageOffset].Copy();
                if(!targetInventory.SetSlot(index + pageOffset, tempItem, tempItem.count)) {
                    InventorySlotUI.heldItem = tempItem.Copy();
                } else if(isEquipment) {
                    EquipmentManager.Instance.OnUnequip(InventorySlotUI.heldItem, targetEntity);
                    EquipmentManager.Instance.OnEquip(targetInventory.items[index + pageOffset], targetEntity);
                }
            }

            OnUpdate(targetInventory);
        }

        public bool IsActive() {
            foreach(var slot in slots) {
                if(slot.IsSelected) return true;
            }
            return false;
        }

        public void OnPageChange(int direction) {
            if(!IsActive()) return;
            if(pageCount == 1) return;

            GameManager.Instance.eventSystem.SetSelectedGameObject(startUI);

            if(page + direction > pageCount) page = 1;
            else if(page + direction < 1) page = pageCount;
            else page += direction;

            OnUpdate(targetInventory);
        }
    }
}