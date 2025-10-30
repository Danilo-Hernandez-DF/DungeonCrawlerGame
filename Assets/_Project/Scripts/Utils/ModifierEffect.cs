using System;
using Game;
using Localisation;

namespace UtilsModule {
    [Serializable]
    public class ModifierEffect {
        public OperatorType operatorType;
        public StatType type;
        public float value;
        public float duration;

        public ModifierEffect(OperatorType operatorType, StatType type, float value, float duration) {
            this.operatorType = operatorType;
            this.type = type;
            this.value = value;
            this.duration = duration;
        }

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