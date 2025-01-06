using Game;
using UnityEngine;

namespace UtilsModule {
    public class Lootable : InventoryHolder, IInteractable {
        [SerializeField] private InventoryGenrator inventoryGenrator;
        [SerializeField] EventChannel lootUIChannel;
        public bool InRange() {
            var colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
            foreach(var collider in colliders) {
                if(collider.CompareTag("Player")) {
                    return true;
                }
            }

            return false;
        }

        public void OnInteract() {
            if(GameManager.Instance.Paused) return;
            if(!InRange()) return;

            inventoryChannel?.Invoke(Inventory);
            lootUIChannel?.Invoke(new Empty());
        }

        void OnEnable() {
            GameManager.Instance.input.Interact += OnInteract;
        }

        void OnDisable() {
            GameManager.Instance.input.Interact -= OnInteract;
        }
    }
}