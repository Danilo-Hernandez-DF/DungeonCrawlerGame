namespace UtilsModule {
    public class StatusEffect {
        public bool ToRemove { get; private set; }
        private int Ticks = 0;
        public string id;
        public DamageSource source;
        public int duration;
        public StatusEffectData Data { get; private set; }
        public StatusEffect(StatusEffectData data, DamageSource source) { 
            this.Data = data; 
            this.source = source;
        }

        public void OnTick(Entity entity) {
            Ticks++;
            if(Ticks >= duration && duration != 0) OnExpire(entity);

            Data.OnTick(entity, Ticks, source);
        }

        private void OnExpire(Entity entity) {
            Data.OnExpire(entity, source);
            ToRemove = true; 
        }
    }
}