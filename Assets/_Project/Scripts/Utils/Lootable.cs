using System.Linq;
using Game;
using UnityEngine;

namespace UtilsModule {
    public class Lootable : InventoryHolder, IInteractable {
        [SerializeField] private InventoryGenrator inventoryGenrator;
        bool opened = false;
        private bool InRange() {
            var colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
            return colliders.Any(collider => collider.CompareTag("Player"));
        }

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