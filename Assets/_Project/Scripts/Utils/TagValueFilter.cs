using UnityEngine;

namespace UtilsModule
{
    [CreateAssetMenu(fileName = "New Slot Filter", menuName = "SlotFilter/TagValueFilter")]
    public class TagValueFilter : SlotFilter {
        [SerializeField] TagData tag;
        [SerializeField] ComparisonType comparisonType;
        [SerializeField] float value;

        public override bool Evaluate(Item item) {
            return comparisonType switch {
                ComparisonType.Equal => item.tags.Exists(x => x.data == tag && x.value == value),
                ComparisonType.NotEqual => !item.tags.Exists(x => x.data == tag && x.value == value),
                ComparisonType.GreaterThan => item.tags.Exists(x => x.data == tag && x.value > value),
                ComparisonType.LessThan => item.tags.Exists(x => x.data == tag && x.value < value),
                _ => false,
            };
        }
    }
}