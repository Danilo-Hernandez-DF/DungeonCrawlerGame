namespace UtilsModule {
    public class StatusEffect {
        public bool ToRemove { get; private set; }
        private int Ticks = 0;
        public string id;
        public StatusEffectData Data { get; private set; }
        public StatusEffect(StatusEffectData data) { 
            this.Data = data; 
        }

        public void OnTick(Entity entity) {
            Ticks++;
            if(Ticks >= Data.duration) OnExpire(entity);

            Data.OnTick(entity, Ticks);
        }

        private void OnExpire(Entity entity) {
            Data.OnExpire(entity);
            ToRemove = true; 
        }
    }
}