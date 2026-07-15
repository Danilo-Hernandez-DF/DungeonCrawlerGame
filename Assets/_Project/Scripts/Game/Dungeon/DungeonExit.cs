

namespace Game
{
    public class DungeonExit: MonoBehaviour, IInteractable
    {
        [SerializeField] DungeonExitUI exitUI;
        
        public void OnInteract() {
            if (!InRange()) return;
            exitUI.UpdateUI();
            exitUI.Open();
            Debug.Log("Interacted with exit");
        }
        
        public void Init(DungeonData nextLevel, DungeonType dungeonType) {
            exitUI.Init(nextLevel, dungeonType);
        }
        
        void OnEnable() {
            GameManager.Instance.input.Interact += OnInteract;
        }

        void OnDisable() {
            GameManager.Instance.input.Interact -= OnInteract;
        }
        
        private bool InRange() => IInteractable.InRange(transform.position);
    }
}