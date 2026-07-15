using Localisation;

namespace Game {
    [Serializable]
    public record class ModifierEffect(OperatorType operatorType, StatType type, float value, float duration)
    {
        public OperatorType operatorType = operatorType;
        public StatType type = type;
        public float value = value;
        public float duration = duration;

        public ModifierEffect(ModifierEffect toCopy) {
            operatorType = toCopy.operatorType;
            type = toCopy.type;
            value = toCopy.value;
            duration = toCopy.duration;
        }

        public override string ToString() {
            var oper = "";
            if (operatorType == OperatorType.Add) oper = "";
            else oper = "%";
            
            return $"{LocalisationSystem.GetLocalisedValue(type.ToString())}: +{value}{oper}";
        }
    }
}