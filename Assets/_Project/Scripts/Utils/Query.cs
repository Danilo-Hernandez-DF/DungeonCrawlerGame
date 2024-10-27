namespace UtilsModule {
    public class Query {
        public readonly StatType statType;
        public float value;
        public float finalMultiplier;

        public Query(StatType statType, float value, float finalMultiplier = 100f) {
            this.statType = statType;
            this.value = value;
            this.finalMultiplier = finalMultiplier;
        }
    }
}