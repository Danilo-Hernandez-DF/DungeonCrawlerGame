using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

namespace UtilsModule {
    public class InventoryUI : UIBase {
        [Header("InvUI")]
        [SerializeField] int columns = 6;
        [SerializeField] int rows = 6;
        [SerializeField] protected GameObject slotPrefab;
        [SerializeField] Transform slotHolder;
        [SerializeField] ItemEventChannel itemDisplayChannel;
        [SerializeField] public Entity targetEntity;
        bool isEquipment => targetEntity != null;
        public int maxDisplaySlots => columns * rows;
        List<InventorySlotUI> slots;
        Inventory targetInventory;
        public Inventory TargetInventory => targetInventory;
        int page = 1;
        int pageCount => Mathf.CeilToInt((float)targetInventory.Size / maxDisplaySlots);
        int pageOffset => (page - 1) * maxDisplaySlots;
        int PageSize => targetInventory.Size - pageOffset > maxDisplaySlots ? maxDisplaySlots : targetInventory.Size - pageOffset;

        protected override void Awake() {
            base.Awake();
            Populate();
            startElement = slots[0].gameObject;
        }

        protected override void OnOpen() {
            if(targetInventory == null) {
                Debug.LogError("No target inventory set");
                return;
            }
            page = 1;

            OnUpdate(targetInventory);
        }

        protected override void OnClose() {
            foreach(var slot in slots) slot.Clear();
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
                slotComp.Init(i);
                slots.Add(slotComp);
            }

            Debug.Log("Populated");
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
                    slots[i].OnUpdate(this);
                } else slots[i].gameObject.SetActive(false);
            }
        }

        public void Refresh() {
            OnUpdate(targetInventory);
        }

        void SwitchSelection(int index) {
            if(index < 0 || index >= targetInventory.Size) return;
            Debug.Log("Pressed: " + index);
            if(InventorySlotUI.HeldItemEmpty) {
                Debug.Log("Empty held item");
                if(targetInventory.items[index + pageOffset].IsEmpty) return;
                InventorySlotUI.heldItem = targetInventory.items[index + pageOffset].Copy();
                targetInventory.ResetSlot(index + pageOffset);

                if(isEquipment) {
                    AdditionalDataManager.Instance.OnUnequip(InventorySlotUI.heldItem, targetEntity);
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
                    AdditionalDataManager.Instance.OnUnequip(InventorySlotUI.heldItem, targetEntity);
                    AdditionalDataManager.Instance.OnEquip(targetInventory.items[index + pageOffset], targetEntity);
                }
            }
        }

        public void OnSlotPressed(int index, int action) {
            if(!open) return;
            switch(action) {
                case -1:
                    SwitchSelection(index);
                    break;
                case 1:
                    //Split stack
                default:
                    break;
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

            GameManager.Instance.eventSystem.SetSelectedGameObject(StartElement);

            if(page + direction > pageCount) page = 1;
            else if(page + direction < 1) page = pageCount;
            else page += direction;

            OnUpdate(targetInventory);
        }
    }
}