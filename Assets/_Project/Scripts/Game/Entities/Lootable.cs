using Utils;

namespace Game { 
    public class Lootable : InventoryHolder, IInteractable {
        [Header("Lootable")]
        [SerializeField] LootType lootType;
        [SerializeField] LootSetting defaultLootSetting;


        bool opened = false;
        
        bool InRange() => IInteractable.InRange(transform.position);

        protected void Awake() {
            LootSetting lootSetting = DungeonController.Instance.GetLootTable(lootType);
            if(lootSetting == null) {
                lootSetting = defaultLootSetting;
            }
            inventoryGenrator?.Init(lootSetting.weightedList, lootSetting.rolls);
            base.Awake();
            DungeonController.Instance.OnLootGenerated(this);
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