namespace Game
{
    public class DungeonEntrance : MonoBehaviour, IInteractable
    {
        [SerializeField] private DungeonType[] potentialDungeons;
        [SerializeField] private DungeonModifier[] modifiers;
        [SerializeField] private DungeonSelectionUI selectionUI;

        private void Awake() {
            modifiers = GameManager.GetDungeonModifiers();
        }

        public void OnInteract() {
            if (InRange()) {
                selectionUI.UpdateUI(potentialDungeons, modifiers);
                selectionUI.Open();
            }
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