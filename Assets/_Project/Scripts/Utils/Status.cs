using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule {
    public class Status {
        public List<StatusEffect> effects = new List<StatusEffect>();
        private CountdownTimer timer;
        public Entity entity;

        public Status(Entity entity) { 
            this.entity = entity;

            timer = new CountdownTimer(0.2f);
            timer.OnTimerStop += () => {
                Tick();
                timer.Reset(); 
                timer.Start(); 
            };
            timer.Start();
        }

        public void Update(float deltaTime) => timer.Tick(deltaTime);

        public void Add(StatusEffect effect) { 
            effects.Add(effect);
            effect.data.OnApply(entity);
        }

        public void Tick() { 
            List<StatusEffect> toRemove = new List<StatusEffect>();

            foreach(var effect in effects) {
                if(effect.toRemove) {
                    toRemove.Add(effect);
                    continue;
                }
                effect.OnTick(entity); 
            }

            foreach(var effect in toRemove) {
                effects.Remove(effect);
            }
        }
    
        public void OnDamage(int damage) { 
            foreach(var effect in effects) {
                effect.data.OnDamage(entity, damage); 
            }
        }

        public void OnHeal(int amount) { 
            foreach(var effect in effects) {
                effect.data.OnHeal(entity, amount); 
            }
        }

        public void OnDeath() { 
            foreach(var effect in effects) {
                effect.data.OnDeath(entity); 
            }
        }
    }
}