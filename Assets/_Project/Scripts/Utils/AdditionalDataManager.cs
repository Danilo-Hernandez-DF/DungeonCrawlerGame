using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;

namespace UtilsModule {
    public class AdditionalDataManager : Singleton<AdditionalDataManager>, IEffector {
        List<AdditionalTagData> tagData;
        List<AdditionalItemData> itemData;
        Dictionary<TagData, AdditionalTagData> dataFromTag;
        Dictionary<ItemData, AdditionalItemData> dataFromItem;
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

            itemData = Resources.LoadAll<AdditionalItemData>("AdditionalItemData").ToList();

            dataFromItem = new Dictionary<ItemData, AdditionalItemData>();

            foreach(var data in itemData) {
                dataFromItem.Add(data.item, data);
            }

            base.Awake();
        }

        public void ApplyEffect(Entity entity, ModifierEffect modifierEffect) {
            BasicStatModifier modifier = modifierEffect.operatorType switch {
                OperatorType.Add => new BasicStatModifier(modifierEffect.type, v => v + modifierEffect.value, 0),
                OperatorType.Multiply => new BasicStatModifier(modifierEffect.type, v => v, 0, modifierEffect.value),
                _ => throw new ArgumentOutOfRangeException()
            };

            entity.Stats.Mediator.AddModifier(modifier, id);
        }

        private void RemoveEffects(Entity entity) {
            entity.Stats.Mediator.RemoveModifiers(id);
        }

        void ApplyQueuedEffects(Entity entity)
        {
            foreach (var modifierEffect in queuedEffects.Select(effect => new ModifierEffect(effect))) {
                ApplyEffect(entity, modifierEffect);
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

            //foreach(ModifierEffect effect in queuedEffects) Debug.Log(effect.ToString());
        }

        public void OnUnequip(Item item, Entity entity) {
            id = item.data.name;
            RemoveEffects(entity);
        }

        public void TriggerItemBehaviour(Inventory source, int indexSource, BehaviourType behaviourType, Entity entitySource = null, int addData = 0) {
            ItemBehaviour[] behaviours = dataFromItem[source.items[indexSource].data].behaviours;
            foreach(var behaviour in behaviours) {
                if(behaviour.behaviourType == behaviourType) behaviour.ExecuteBehaviour(source, indexSource, entitySource, addData);
            }
        }

        private List<ModifierEffect> GetEffectsFromItem(Item item) {
            var effects = new List<ModifierEffect>();

            foreach(var tag in item.tags)
            {
                if(!dataFromTag.TryGetValue(tag.data, out AdditionalTagData data)) continue;
                foreach(var effect in data.effects) {
                    var tempEffect = new ModifierEffect(effect);

                    tempEffect.value = tag.Type switch {
                        TagData.TagType.Float => tag.GetValue(),
                        TagData.TagType.Int => Mathf.FloorToInt(tag.GetValue()),
                        _ => tempEffect.value
                    };

                    effects.Add(tempEffect);
                }
            }

            return effects;
        }

        public List<ModifierEffect> GetAdditionalsFromItem(Item item) {
            var effects = new List<ModifierEffect>();

            foreach(var tag in item.tags)
            {
                if(!dataFromTag.TryGetValue(tag.data, out AdditionalTagData data)) continue;
                foreach(var effect in data.additionalEffects) {
                    var tempEffect = new ModifierEffect(effect);

                    tempEffect.value = tag.Type switch {
                        TagData.TagType.Float => tag.GetValue(),
                        TagData.TagType.Int => Mathf.FloorToInt(tag.GetValue()),
                        _ => tempEffect.value
                    };

                    effects.Add(tempEffect);
                }
            }

            return effects;
        }

        public List<StatusEffectData> GetStatusFromItem(Item item) {
            var effects = new List<StatusEffectData>();

            foreach(var tag in item.tags)
            {
                if(!dataFromTag.TryGetValue(tag.data, out AdditionalTagData data)) continue;
                effects.AddRange(data.statusEffects);
            }

            return effects;
        }
    }
}