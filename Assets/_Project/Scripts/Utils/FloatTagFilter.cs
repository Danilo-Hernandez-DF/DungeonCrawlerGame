using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Slot Filter", menuName = "SlotFilter/FloatTagFilter")]
    public class FloatTagFilter : SlotFilter {
        [SerializeField] float value;
        [SerializeField] FloatTagData tag;
        [SerializeField] ComparisonType comparisonType;

        public override bool Evaluate(Item item) {
            if(!item.tags.Exists(x => x.data == tag)) return false;
            Tag<float> foundTag = item.tags.Find(x => x.data == tag) as Tag<float>;

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