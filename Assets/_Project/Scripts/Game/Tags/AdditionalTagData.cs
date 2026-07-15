namespace Game {
    [CreateAssetMenu(fileName = "AdditionalTagData", menuName = "Data/AdditionalTagData")]
    public class AdditionalTagData : ScriptableObject
    {
        public TagData[] tags;
        public ModifierEffect[] effects;
        public ModifierEffect[] additionalEffects;
        public StatusEffectData[] statusEffects;
        public StatusEffect[] additionalStatusEffects;
    }
}