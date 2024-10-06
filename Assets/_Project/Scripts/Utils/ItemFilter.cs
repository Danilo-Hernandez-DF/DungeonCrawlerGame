using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Slot Filter", menuName = "SlotFilter/ItemFilter")]
    public class ItemFilter : SlotFilter {
        [SerializeField] Item value;
        [SerializeField] ComparisonType comparisonType;

        public override bool Evaluate(Item item) {
            return comparisonType switch {
                ComparisonType.Equal => item.Matches(value),
                ComparisonType.NotEqual => !item.Matches(value),
                _ => false,
            };
        }
    }
}