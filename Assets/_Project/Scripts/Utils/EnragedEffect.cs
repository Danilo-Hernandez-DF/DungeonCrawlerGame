using System;
using Game;
using UnityEngine;

namespace UtilsModule
{
    [CreateAssetMenu(menuName = "Data/StatusEffectData/Enraged")]
    public class EnragedEffect : StatusEffectData
    {
        [SerializeField] private float atkMult = 20;
        [SerializeField] private float defMod = -5;
        [SerializeField] private float speedMult = 10;
        public override void OnApply(Entity entity, DamageSource source) { 
            ApplyEffect(entity, new ModifierEffect(OperatorType.Multiply, StatType.Attack, atkMult, duration));
            ApplyEffect(entity, new ModifierEffect(OperatorType.Add, StatType.Defense, defMod, duration));
            ApplyEffect(entity, new ModifierEffect(OperatorType.Multiply, StatType.Speed, speedMult, duration));
        }
        
        protected override void OnRemove(Entity entity, DamageSource source) { 
            ApplyEffect(entity, new ModifierEffect(OperatorType.Multiply, StatType.Speed, -speedMult, duration));
        }
    }
}