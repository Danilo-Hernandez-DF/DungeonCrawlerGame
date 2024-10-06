using System;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Tag Data", menuName = "Data/Tag/Equipment Tag Data"), Serializable]
    public class EquipmentTagData : TagData<EquipmentSlot> { }

    public enum EquipmentSlot {
        Head,
        Body,
        Legs,
        Charm,
        Weapon,
        Any
    }
}