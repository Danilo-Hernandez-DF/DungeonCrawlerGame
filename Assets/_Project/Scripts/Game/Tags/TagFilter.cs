namespace Game
{
    [CreateAssetMenu(fileName = "New Slot Filter", menuName = "SlotFilter/TagFilter")]
    public class TagFilter : SlotFilter {
        [SerializeField] TagData tag;
        [SerializeField] ComparisonType comparisonType;

        public override bool Evaluate(Item item) {
            return comparisonType switch {
                ComparisonType.Equal => item.tags.Exists(x => x.data == tag),
                ComparisonType.NotEqual => !item.tags.Exists(x => x.data == tag),
                _ => false,
            };
        }
    }
}