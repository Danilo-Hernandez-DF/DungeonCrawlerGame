namespace Game {
    [System.Serializable]
    public record class StatusEffect
    {
        public bool ToRemove { get; private set; }
        private int Ticks = 0;
        public string id;
        public DamageSource source;
        public int duration;
        public StatusEffectData Data;
        
        public StatusEffect(StatusEffectData data, DamageSource source, int duration) {
            this.Data = data;
            this.source = source;
            this.duration = duration;
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