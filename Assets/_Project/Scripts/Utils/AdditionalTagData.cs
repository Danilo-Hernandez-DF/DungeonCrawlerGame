using Game;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "AdditionalTagData", menuName = "Data/AdditionalTagData")]
    public class AdditionalTagData : ScriptableObject
    {
        public TagData[] tags;
        public ModifierEffect[] effects;
        public ModifierEffect[] additionalEffects;
        public StatusEffectData[] statusEffects;
        public StatusEffectData[] additionalStatusEffects;
    }
}