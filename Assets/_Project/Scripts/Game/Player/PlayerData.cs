using Utils;

namespace Game {
    [Serializable]
    public class PlayerData : ISaveable {
        [field: SerializeField] public SerializableGuid Id { get; set; }
    }
}