using Utils;

namespace Game
{
    public class ShopUI : UIBase {
        [SerializeField] InventoryUI playerInventory;
        [SerializeField] ShopInventoryUI shopInventory;
        
        public InventoryUI PlayerInventory => playerInventory;
        public ShopInventoryUI ShopInventory => shopInventory;
        
        protected override void Awake() {
            base.Awake();

            children = new List<UIBase> {
                playerInventory,
                shopInventory
            };
            
            foreach(UIBase child in children) {
                child.SetParent(this);
            }
        }
        
        protected override void OnClose() {
            if(InventorySlotUI.HeldItemEmpty) return;
            if(playerInventory.TargetInventory.AvailableCount(InventorySlotUI.heldItem) < InventorySlotUI.heldItem.count) {
                //create pickup on ground
            } else {
                playerInventory.TargetInventory.Add(InventorySlotUI.heldItem, InventorySlotUI.heldItem.count);
            }
                
            InventorySlotUI.heldItem = null;
        }
    }
}