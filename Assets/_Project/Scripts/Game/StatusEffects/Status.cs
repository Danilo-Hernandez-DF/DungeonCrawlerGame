using Utils;

namespace Game {
    public class Status {
        private readonly List<StatusEffect> Effects = new List<StatusEffect>();
        private readonly CountdownTimer timer;
        private readonly Entity Entity;

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

        public void Add(StatusEffect effect, DamageSource source, int duration, string id = "") { 
            if(Effects.Exists(x => x.Data == effect.Data)) return;
            effect.id = id;
            Effects.Add(effect);
            effect.Data.OnApply(Entity, source, duration);
        }

        public void RemoveStatus(string id)
        {
            List<StatusEffect> toRemove = new List<StatusEffect>();

            foreach (var effect in Effects)
            {
                if (effect.id == id) toRemove.Add(effect);
            }

            foreach (var effect in toRemove)
            {
                Effects.Remove(effect);
                effect.Data.OnExpire(Entity, effect.source);Effects.Remove(effect);
            }
        }

        public void Tick()
        {
            List<StatusEffect> toRemove = new List<StatusEffect>();

            foreach (var effect in Effects)
            {
                if (effect.ToRemove)
                {
                    toRemove.Add(effect);
                    continue;
                }
                effect.OnTick(Entity);
            }

            foreach (var effect in toRemove)
            {
                Effects.Remove(effect);
            }
        }
    
        public void OnDamage(int damage) { 
            foreach(var effect in Effects) {
                effect.Data.OnDamage(Entity, damage, effect.source); 
            }
        }

        public void OnHeal(int amount) { 
            foreach(var effect in Effects) {
                effect.Data.OnHeal(Entity, amount, effect.source); 
            }
        }

        public void OnDeath() { 
            foreach(var effect in Effects) {
                effect.Data.OnDeath(Entity, effect.source); 
            }
        }
    }
}