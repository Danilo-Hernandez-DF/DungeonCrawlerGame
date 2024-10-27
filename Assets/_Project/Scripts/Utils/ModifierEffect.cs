using System;
using Game;

namespace UtilsModule {
    [Serializable]
    public class ModifierEffect {
        public OperatorType operatorType;
        public StatType type;
        public float value;
        public float duration;
    }
}