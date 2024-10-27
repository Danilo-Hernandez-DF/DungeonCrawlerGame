using System;

namespace UtilsModule {
    public class BasicStatModifier : StatModifier {
        readonly StatType type;
        readonly Func<float, float> operation;
        float finalMultiplier;

        public BasicStatModifier(StatType type, Func<float, float> operation, float duration, float finalMultiplier = 0f) : base(duration) {
            this.type = type;
            this.operation = operation;
            this.finalMultiplier = finalMultiplier;
        }

        public override void Handle(object sender, Query query) {
            if(query.statType == type) {
                query.value = operation.Invoke(query.value);
                query.finalMultiplier += finalMultiplier;
            }
        }
    }
}