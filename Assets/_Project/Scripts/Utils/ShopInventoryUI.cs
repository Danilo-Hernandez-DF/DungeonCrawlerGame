using Game;
using UnityEngine;
using UtilsModule;

namespace _Project.Scripts.Utils {
    public class ShopInventoryUI : InventoryUI {
        [Header("Shop UI")] 
        [SerializeField] private Item currency;
        public Sprite currencyIcon;
        [SerializeField] private TagData costTag;
        public override void OnSlotPressed(int index, int action) {
            if(!open) return;
            int cost = (int)TargetInventory.items[index].GetTag(costTag).GetValue();
            if (cost > PlayerDetector.GetPlayerComponent().playerInv.GetCount(currency, false)) return;
            Debug.Log("currency is enough");
            
            switch(action) {
                case -1:
                    if(!InventorySlotUI.HeldItemEmpty && 
                       !InventorySlotUI.heldItem.Matches(TargetInventory.items[index])) return;
                    
                    if(InventorySlotUI.HeldItemEmpty) {
                        InventorySlotUI.heldItem = TargetInventory.items[index].Copy();
                    } else {
                        InventorySlotUI.heldItem.count++;
                        if(InventorySlotUI.heldItem.count > TargetInventory.items[index].data.maxCount) {
                            InventorySlotUI.heldItem.count = TargetInventory.items[index].data.maxCount;
                            break;
                        };
                    }

                    PlayerDetector.GetPlayerComponent().playerInv.TryRemove(currency, cost, false);
                    
                    break;
                case 1:
                    if(!IsChild) return;
                    var parentAsShop = Parent.GetComponent<ShopUI>();
                    if (!parentAsShop) return;
                    if (parentAsShop.ShopInventory != this) return;
                    
                    if(parentAsShop.PlayerInventory.TargetInventory
                           .AvailableCount(TargetInventory.items[index]) < 1) return;
                    
                    parentAsShop.PlayerInventory.TargetInventory.Add(TargetInventory.items[index]);
                    PlayerDetector.GetPlayerComponent().playerInv.TryRemove(currency, cost, false);

                    break;
            }

            OnUpdate(TargetInventory);
        }
    }
}