using Utils;

namespace Game {
    public class Shop : InventoryHolder, IInteractable
    {
        [SerializeField] private InventoryGenerator generator;
        
        bool InRange() => IInteractable.InRange(transform.position);
        
        public void OnInteract() {
            if(GameManager.Instance.Paused) return;
            if(!InRange()) return;
            
            inventoryChannel?.Invoke(Inventory);
            lootUIChannel?.Invoke(new Empty());
        }

        public override void OnGenerate() {
            foreach(Item item in Inventory.items) {
                item.count = 1;
            }
        }

        void OnEnable() {
            GameManager.Instance.input.Interact += OnInteract;
        }

        void OnDisable() {
            GameManager.Instance.input.Interact -= OnInteract;
        }
    }
}