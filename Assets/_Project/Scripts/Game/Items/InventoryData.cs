using Utils;

namespace Game {
    [Serializable]
    public class InventoryData : ISaveable{
        [field: SerializeField] public SerializableGuid Id { get; set; }
        public SavedItem[] items = Array.Empty<SavedItem>();
    }
}