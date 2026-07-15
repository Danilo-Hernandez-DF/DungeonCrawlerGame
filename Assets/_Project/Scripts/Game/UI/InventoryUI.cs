using System.Linq;
using UnityEngine.UI;
using Utils;

namespace Game {
    public class InventoryUI : UIBase {
        [Header("InvUI")]
        [SerializeField] int columns = 6;
        [SerializeField] int rows = 6;
        [SerializeField] protected GameObject slotPrefab;
        [SerializeField] Transform slotHolder;
        [SerializeField] ItemEventChannel itemDisplayChannel;
        
        private int MaxDisplaySlots => columns * rows;
        List<InventorySlotUI> slots;
        public Inventory TargetInventory { get; private set; }

        int page = 1;
        int PageCount => Mathf.CeilToInt((float)TargetInventory.Size / MaxDisplaySlots);
        int PageOffset => (page - 1) * MaxDisplaySlots;
        int PageSize => TargetInventory.Size - PageOffset > MaxDisplaySlots ? MaxDisplaySlots : TargetInventory.Size - PageOffset;

        protected override void Awake() {
            base.Awake();
            Populate();
            startElement = slots[0].gameObject;
        }

        protected override void OnOpen() {
            if(TargetInventory == null) {
                Debug.LogError("No target inventory set");
                return;
            }
            page = 1;

            OnUpdate(TargetInventory);
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
                var newSlot = Instantiate(slotPrefab, slotHolder, false);
                var slotComp = newSlot.GetComponent<InventorySlotUI>();
                slotComp.Init(i);
                slots.Add(slotComp);
            }
        }

        public void OnUpdate(Inventory inventory) {
            if(inventory != TargetInventory) TargetInventory = inventory;
            
            if(MaxDisplaySlots > TargetInventory.Size) {
                for(int i = 0; i < MaxDisplaySlots; i++) {
                    slots[i].gameObject.SetActive(i < TargetInventory.Size);
                }
            }
        
            for(int i = 0; i < slots.Count; i++) {
                if(i < PageSize) {
                    slots[i].gameObject.SetActive(true);
                    slots[i].Refresh(PageOffset);
                    slots[i].OnUpdate(this, gameObject.GetComponent<ShopInventoryUI>());
                } else slots[i].gameObject.SetActive(false);
            }
        }

        public void Refresh() {
            OnUpdate(TargetInventory);
        }

        void SwitchSelection(int index) {
            if(index < 0 || index >= TargetInventory.Size) return;
            if(InventorySlotUI.HeldItemEmpty) {
                if(TargetInventory.items[index].IsEmpty) return;
                InventorySlotUI.heldItem = TargetInventory.items[index].Copy();
                TargetInventory.ResetSlot(index);

                if(TargetInventory.IsEquipment) {
                    AdditionalDataManager.Instance.OnUnequip(InventorySlotUI.heldItem, TargetInventory.targetEntity);
                }
            } else if(InventorySlotUI.heldItem.Matches(TargetInventory.items[index])) {
                int remainder = TargetInventory.AddAt(index, InventorySlotUI.heldItem, InventorySlotUI.heldItem.count);
                if(remainder > 0) InventorySlotUI.heldItem.count = remainder;
                else InventorySlotUI.heldItem = new Item(GameManager.emptyItem);
            } else {
                var tempItem = InventorySlotUI.heldItem.Copy();
                InventorySlotUI.heldItem = TargetInventory.items[index].Copy();
                if(!TargetInventory.SetSlot(index, tempItem, tempItem.count)) {
                    InventorySlotUI.heldItem = tempItem.Copy();
                } else if(TargetInventory.IsEquipment) {
                    AdditionalDataManager.Instance.OnUnequip(InventorySlotUI.heldItem, TargetInventory.targetEntity);
                    AdditionalDataManager.Instance.OnEquip(TargetInventory.items[index], TargetInventory.targetEntity);
                }
            }
        }

        public virtual void OnSlotPressed(int index, int action) {
            if(!open) return;
            switch(action) {
                case -1:
                    SwitchSelection(index);
                    break;
                case 1:
                    if(!IsChild) return;
                    var parentAsLoot = Parent.GetComponent<LootingInventoryUI>();
                    var parentAsPlayer = Parent.GetComponent<PlayerInventoryUI>();
                    
                    if (!parentAsLoot && !parentAsPlayer) return;

                    if (parentAsLoot) {
                        if (parentAsLoot.PlayerInventory == this) TransferItem(index, parentAsLoot.OtherInventory);
                        else TransferItem(index, parentAsLoot.PlayerInventory);
                    } else if(parentAsPlayer) {
                        if (parentAsPlayer.Inventory == this) TransferItem(index, parentAsPlayer.Equipment);
                        else TransferItem(index, parentAsPlayer.Inventory); 
                    }

                    break;
            }

            OnUpdate(TargetInventory);
        }
        
        protected void TransferItem(int index, InventoryUI other) {
            if(index < 0 || index >= TargetInventory.Size) return;
            var tempItem = TargetInventory.items[index].Copy();
            int remainder = other.TargetInventory.Add(tempItem, tempItem.count);

            if (remainder == tempItem.count) return;
            
            if (remainder > 0) TargetInventory.items[index].count = remainder;
            else {
                TargetInventory.ResetSlot(index);
                
                if (TargetInventory.IsEquipment) {
                    AdditionalDataManager.Instance.OnUnequip(tempItem, TargetInventory.targetEntity);
                }
            }
            
            if(other.TargetInventory.IsEquipment) {
                AdditionalDataManager.Instance.OnEquip(tempItem, other.TargetInventory.targetEntity);
            }
        }

        private bool IsActive() {
            return slots.Any(slot => slot.IsSelected);
        }

        public void OnPageChange(int direction) {
            if(!IsActive()) return;
            if(PageCount == 1) return;

            GameManager.Instance.eventSystem.SetSelectedGameObject(StartElement);

            if(page + direction > PageCount) page = 1;
            else if(page + direction < 1) page = PageCount;
            else page += direction;

            OnUpdate(TargetInventory);
        }
    }
}