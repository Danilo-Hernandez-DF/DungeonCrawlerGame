namespace UtilsModule {
    public class StatusEffect {
        public bool toRemove { get; protected set; }
        protected int ticks = 0;
        public StatusEffectData data { get; private set; }
        public StatusEffect(StatusEffectData data) { 
            this.data = data; 
        }

        public void OnTick(Entity entity) {
            ticks++;
            if(ticks >= data.duration) OnExpire(entity);

            data.OnTick(entity, ticks);
        }

        public void OnExpire(Entity entity) {
            data.OnExpire(entity);
            toRemove = true; 
        }
    }
}