using System;
using UnityEngine;

namespace UtilsModule {
    public class BasicStatModifier : StatModifier {
        readonly StatType type;
        readonly Func<float, float> operation;
        readonly float finalMultiplier;

        public BasicStatModifier(StatType type, Func<float, float> operation, float duration, float finalMultiplier = 0f) : base(duration) {
            this.type = type;
            this.operation = operation;
            this.finalMultiplier = finalMultiplier;
        }

        public override void Handle(object sender, Query query)
        {
            if(query.StatType != type) return;
            query.Value = operation.Invoke(query.Value);
            query.FinalMultiplier += finalMultiplier;

            //Debug.Log($"StatModifier {id} applied to {query.statType} with value {query.value}");
        }
    }
}