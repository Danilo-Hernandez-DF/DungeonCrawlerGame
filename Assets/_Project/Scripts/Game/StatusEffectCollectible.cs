using UnityEngine;
using UtilsModule;

namespace Game {
    public class StatusEffectCollectible : Collectible { 
        [SerializeField] StatusEffectData statusEffectData;

        public override void ApplyEffect(Entity entity) {
            entity.Status.Add(new StatusEffect(statusEffectData));
            Debug.Log($"{entity.name} was given {statusEffectData.name}");
        }
    }
}