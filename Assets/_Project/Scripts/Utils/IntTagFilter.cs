using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Slot Filter", menuName = "SlotFilter/IntTagFilter")]
    public class IntTagFilter : SlotFilter {
        [SerializeField] int value;
        [SerializeField] IntTagData tag;
        [SerializeField] ComparisonType comparisonType;

        public override bool Evaluate(Item item) {
            if(!item.tags.Exists(x => x.data == tag)) return false;
            Tag<int> foundTag = item.tags.Find(x => x.data == tag) as Tag<int>;

            return comparisonType switch {
                ComparisonType.Equal => foundTag.GetValue() == value,
                ComparisonType.NotEqual => foundTag.GetValue() != value,
                ComparisonType.GreaterThan => foundTag.GetValue() > value,
                ComparisonType.LessThan => foundTag.GetValue() < value,
                _ => false,
            };
        }
    }
}