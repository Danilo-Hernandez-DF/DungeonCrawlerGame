using Utils;

namespace Game
{
    [Serializable]
    public class NPCData : ISaveable {
        [field: SerializeField] public SerializableGuid Id { get; set; }

        public int currentQuest;
        public bool questComplete;
        public int currentDialogue;
    }
}