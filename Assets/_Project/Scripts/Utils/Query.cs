namespace UtilsModule {
    public class Query {
        public readonly StatType StatType;
        public float Value;
        public float FinalMultiplier;

        public Query(StatType statType, float value, float finalMultiplier = 100f) {
            this.StatType = statType;
            this.Value = value;
            this.FinalMultiplier = finalMultiplier;
        }
    }
}