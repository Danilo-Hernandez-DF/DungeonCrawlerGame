using System;
using Game;

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
            return $"{operatorType}, {type}, val:{value}, dur:{duration}";
        }
    }
}