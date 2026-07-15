using Utils;

namespace Game {
    public class LootingInventoryUI : UIBase {
        [SerializeField] InventoryUI playerInventory;
        [SerializeField] InventoryUI otherInventory;
        
        public InventoryUI PlayerInventory => playerInventory;
        public InventoryUI OtherInventory => otherInventory;

        protected override void Awake() {
            base.Awake();

            children = new List<UIBase> {
                playerInventory,
                otherInventory
            };
            
            foreach(UIBase child in children) {
                child.SetParent(this);
            }
        }

        protected override void OnClose() {
            if(InventorySlotUI.HeldItemEmpty) return;
            if(playerInventory.TargetInventory.AvailableCount(InventorySlotUI.heldItem) < InventorySlotUI.heldItem.count) {
                otherInventory.TargetInventory.Add(InventorySlotUI.heldItem, InventorySlotUI.heldItem.count);
            } else {
                playerInventory.TargetInventory.Add(InventorySlotUI.heldItem, InventorySlotUI.heldItem.count);
            }
                
            InventorySlotUI.heldItem = null;
        }
    }
}