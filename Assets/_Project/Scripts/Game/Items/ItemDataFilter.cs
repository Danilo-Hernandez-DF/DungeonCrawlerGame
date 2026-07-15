namespace Game {
    [CreateAssetMenu(fileName = "New Slot Filter", menuName = "SlotFilter/ItemDataFilter")]
    public class ItemDataFilter : SlotFilter {
        [SerializeField] ItemData value;
        [SerializeField] ComparisonType comparisonType;

        public override bool Evaluate(Item item) {
            return comparisonType switch {
                ComparisonType.Equal => item.data == value,
                ComparisonType.NotEqual => item.data != value,
                _ => false,
            };
        }
    }
}