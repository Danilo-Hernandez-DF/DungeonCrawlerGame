namespace Game {
    [CreateAssetMenu(fileName = "AdditionalItemData", menuName = "Data/AdditionalItemData")]
    public class AdditionalItemData : ScriptableObject {
        public ItemData item;
        public ItemBehaviour[] behaviours;
    }
}