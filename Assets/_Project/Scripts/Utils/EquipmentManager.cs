using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;

namespace UtilsModule {
    public class EquipmentManager : Singleton<EquipmentManager>, IEffector {
        List<AdditionalTagData> tagData;
        Dictionary<TagData, AdditionalTagData> dataFromTag;
        private ModifierEffect modifierEffect;
        private List<ModifierEffect> queuedEffects;
        private string id;

        protected override void Awake() {
            tagData = Resources.LoadAll<AdditionalTagData>("AdditionalTagData").ToList();

            dataFromTag = new Dictionary<TagData, AdditionalTagData>();

            foreach(var data in tagData) {
                foreach(var tag in data.tags) {
                    dataFromTag.Add(tag, data);
                }
            }

            base.Awake();
        }

        public void ApplyEffect(Entity entity) {
            StatModifier modifier = modifierEffect.operatorType switch {
                OperatorType.Add => new BasicStatModifier(modifierEffect.type, v => v + modifierEffect.value, 0),
                OperatorType.Multiply => new BasicStatModifier(modifierEffect.type, v => v, 0, modifierEffect.value),
                _ => throw new ArgumentOutOfRangeException()
            };

            entity.Stats.Mediator.AddModifier(modifier, id);
        }

        public void RemoveEffects(Entity entity) {
            entity.Stats.Mediator.RemoveModifiers(id);
        }

        void ApplyQueuedEffects(Entity entity) {
            foreach(var effect in queuedEffects) {
                modifierEffect = effect;
                ApplyEffect(entity);
            }
        }

        public void Visit<T> (T visitable) where T : Component, IVisitable {
            if(visitable is Entity entity) {
                ApplyQueuedEffects(entity);
            }
        }

        public void OnEquip(Item item, Entity entity) {
            id = item.data.name;
            queuedEffects = GetEffectsFromItem(item);
            ApplyQueuedEffects(entity);
        }

        public void OnUnequip(Item item, Entity entity) {
            id = item.data.name;
            RemoveEffects(entity);
        }

        List<ModifierEffect> GetEffectsFromItem(Item item) {
            var effects = new List<ModifierEffect>();

            foreach(var tag in item.tags) {
                if(dataFromTag.TryGetValue(tag.data, out AdditionalTagData data)) {
                    foreach(var effect in data.effects) {
                        var tempEffect = effect;

                        if(tag.data is TagData<float>) {
                            tempEffect.value = ((Tag<float>)tag).GetValue();
                        } else if(tag.data is TagData<int>) {
                            tempEffect.value = ((Tag<int>)tag).GetValue();
                        }

                        effects.Add(tempEffect);
                    }
                }
            }

            return effects;
        }
    }
}