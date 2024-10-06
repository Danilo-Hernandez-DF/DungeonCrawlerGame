using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Slot Filter", menuName = "SlotFilter/EquipmentTagFilter")]
    public class EquipmentTagFilter : SlotFilter {
        [SerializeField] EquipmentSlot value;
        [SerializeField] EquipmentTagData tag;
        [SerializeField] ComparisonType comparisonType;

        public override bool Evaluate(Item item) {
            if(!item.tags.Exists(x => x.data == tag)) return false;
            Tag<EquipmentSlot> foundTag = item.tags.Find(x => x.data == tag) as Tag<EquipmentSlot>;

            return comparisonType switch {
                ComparisonType.Equal => foundTag.GetValue() == value,
                ComparisonType.NotEqual => foundTag.GetValue() != value,
                _ => false,
            };
        }
    }
}