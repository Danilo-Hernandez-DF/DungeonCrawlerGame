using System.Linq;
using Game;
using UnityEngine;

namespace UtilsModule { 
    public class Lootable : InventoryHolder, IInteractable {
        [SerializeField] private InventoryGenrator inventoryGenrator;
        bool opened = false;
        
        bool InRange() => IInteractable.InRange(transform.position);
        
        public void OnInteract() {
            if(GameManager.Instance.Paused) return;
            if(!InRange()) return;
            if(!opened) {
                GameManager.Instance.TrackLoot(entityData, 1);

                foreach(Item item in Inventory.items) {
                    if (item.IsEmpty) continue;
                    GameManager.Instance.TrackItem(item.data, item.count);
                }
            }

            opened = true;
            
            inventoryChannel?.Invoke(Inventory);
            lootUIChannel?.Invoke(new Empty());
        }
        
        public void Reset() {
            Inventory.Clear();
            inventoryGenrator.Generate();
            DungeonController.Instance.OnLootGenerated(this);
        }

        void OnEnable() {
            GameManager.Instance.input.Interact += OnInteract;
        }

        void OnDisable() {
            GameManager.Instance.input.Interact -= OnInteract;
        }
    }
}