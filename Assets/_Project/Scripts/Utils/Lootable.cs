using Game;
using KBCore.Refs;
using UnityEngine;

namespace UtilsModule {
    public class Lootable : InventoryHolder, IInteractable {
        [SerializeField, Self] private InventoryGenrator inventoryGenrator;
        [SerializeField] private float interactRange = 1f;
        [SerializeField] EventChannel lootUIChannel;
        public bool InRange() {
            var colliders = Physics2D.OverlapCircleAll(transform.position, interactRange);
            foreach(var collider in colliders) {
                if(collider.CompareTag("Player")) {
                    return true;
                }
            }

            return false;
        }

        void OnValidate() => this.ValidateRefs();

        public void OnInteract() {
            if(GameManager.Instance.Paused) return;
            if(!InRange()) return;

            inventoryChannel?.Invoke(inventory);
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