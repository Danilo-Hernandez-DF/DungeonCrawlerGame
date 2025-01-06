using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule {
    public class Status {
        public List<StatusEffect> Effects = new List<StatusEffect>();
        private CountdownTimer timer;
        public Entity Entity;

        public Status(Entity entity) { 
            this.Entity = entity;

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
            Effects.Add(effect);
            effect.Data.OnApply(Entity);
        }

        public void Tick() { 
            List<StatusEffect> toRemove = new List<StatusEffect>();

            foreach(var effect in Effects) {
                if(effect.ToRemove) {
                    toRemove.Add(effect);
                    continue;
                }
                effect.OnTick(Entity); 
            }

            foreach(var effect in toRemove) {
                Effects.Remove(effect);
            }
        }
    
        public void OnDamage(int damage) { 
            foreach(var effect in Effects) {
                effect.Data.OnDamage(Entity, damage); 
            }
        }

        public void OnHeal(int amount) { 
            foreach(var effect in Effects) {
                effect.Data.OnHeal(Entity, amount); 
            }
        }

        public void OnDeath() { 
            foreach(var effect in Effects) {
                effect.Data.OnDeath(Entity); 
            }
        }
    }
}