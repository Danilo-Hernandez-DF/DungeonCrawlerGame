using Utils;

namespace Game {
    public class NPC : MonoBehaviour, IBind<NPCData>, IInteractable {
        private NPCData data;
        public SerializableGuid Id { get; set; }

        private DialogueBox dialogueBox;
        [SerializeField] private Quest[] quests;
        [SerializeField] private int currentQuest;
        private bool questCompleted;
        [SerializeField] private int currentDialogue;
        [SerializeField] private Dialogue[] dialogueElements;

        private void Awake() {
            dialogueBox = GetComponent<DialogueBox>();
        }

        public void Bind(NPCData data) {
            this.data = data;
            this.data.Id = Id;

            this.currentQuest = data.currentQuest;
            this.questCompleted = data.questComplete;
            this.currentDialogue = data.currentDialogue;
        }

        void Update() {
            data.currentQuest = currentQuest;
            data.questComplete = questCompleted;
            data.currentDialogue = currentDialogue;

            if(questCompleted) {
                if(currentDialogue + 1 < dialogueElements.Length) currentDialogue++;
                questCompleted = false;
            }
        }
        
        bool InRange() => IInteractable.InRange(transform.position);

        public void OnInteract() {
            if(!InRange()) return;
            if(!dialogueBox.Open) dialogueBox.Init(dialogueElements[currentDialogue].elements);
            dialogueBox.Interacted(this);
        }
        
        void OnEnable() {
            GameManager.Instance.input.Interact += OnInteract;
        }

        void OnDisable() {
            GameManager.Instance.input.Interact -= OnInteract;
        }
    }
    
    [Serializable]
    public struct Dialogue {
        public DialogueElement[] elements;
    }
}