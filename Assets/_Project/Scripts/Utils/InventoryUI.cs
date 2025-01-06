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
        bool IsEquipment => targetEntity != null;
        public int MaxDisplaySlots => columns * rows;
        List<InventorySlotUI> slots;
        Inventory targetInventory;
        public Inventory TargetInventory => targetInventory;
        int page = 1;
        int PageCount => Mathf.CeilToInt((float)targetInventory.Size / MaxDisplaySlots);
        int PageOffset => (page - 1) * MaxDisplaySlots;
        int PageSize => targetInventory.Size - PageOffset > MaxDisplaySlots ? MaxDisplaySlots : targetInventory.Size - PageOffset;

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
                
            for(int i = 0; i < MaxDisplaySlots; i++) {
                var newSlot = Instantiate(slotPrefab);
                newSlot.transform.SetParent(slotHolder, false);
                var slotComp = newSlot.GetComponent<InventorySlotUI>();
                slotComp.Init(i);
                slots.Add(slotComp);
            }

            //Debug.Log("Populated");
        }

        public void OnUpdate(Inventory inventory) {
            if(inventory != targetInventory) targetInventory = inventory;
            
            if(MaxDisplaySlots > targetInventory.Size) {
                for(int i = 0; i < MaxDisplaySlots; i++) {
                    if(i < targetInventory.Size) slots[i].gameObject.SetActive(true);
                    else slots[i].gameObject.SetActive(false);
                }
            }
        
            for(int i = 0; i < slots.Count; i++) {
                if(i < PageSize) {
                    slots[i].gameObject.SetActive(true);
                    slots[i].Refresh(PageOffset);
                    slots[i].OnUpdate(this);
                } else slots[i].gameObject.SetActive(false);
            }
        }

        public void Refresh() {
            OnUpdate(targetInventory);
        }

        void SwitchSelection(int index) {
            if(index < 0 || index >= targetInventory.Size) return;
            //Debug.Log("Pressed: " + index);
            if(InventorySlotUI.HeldItemEmpty) {
                //Debug.Log("Empty held item");
                if(targetInventory.items[index + PageOffset].IsEmpty) return;
                InventorySlotUI.heldItem = targetInventory.items[index + PageOffset].Copy();
                targetInventory.ResetSlot(index + PageOffset);

                if(IsEquipment) {
                    AdditionalDataManager.Instance.OnUnequip(InventorySlotUI.heldItem, targetEntity);
                }
            } else if(InventorySlotUI.heldItem.Matches(targetInventory.items[index + PageOffset])) {
                //Debug.Log("Matching held item");
                int remainder = targetInventory.AddAt(index + PageOffset, InventorySlotUI.heldItem, InventorySlotUI.heldItem.count);
                if(remainder > 0) InventorySlotUI.heldItem.count = remainder;
                else InventorySlotUI.heldItem = new Item(GameManager.Instance.emptyItem);
            } else {
                //Debug.Log("Different held item");
                var tempItem = InventorySlotUI.heldItem.Copy();
                InventorySlotUI.heldItem = targetInventory.items[index + PageOffset].Copy();
                if(!targetInventory.SetSlot(index + PageOffset, tempItem, tempItem.count)) {
                    InventorySlotUI.heldItem = tempItem.Copy();
                } else if(IsEquipment) {
                    AdditionalDataManager.Instance.OnUnequip(InventorySlotUI.heldItem, targetEntity);
                    AdditionalDataManager.Instance.OnEquip(targetInventory.items[index + PageOffset], targetEntity);
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
            if(PageCount == 1) return;

            GameManager.Instance.eventSystem.SetSelectedGameObject(StartElement);

            if(page + direction > PageCount) page = 1;
            else if(page + direction < 1) page = PageCount;
            else page += direction;

            OnUpdate(targetInventory);
        }
    }
}