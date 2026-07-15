using Localisation;

namespace Game {
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Basic")]
    public class Quest : ScriptableObject {
        public Objective[] objectives;
        public StatisticsTracker tracker;
        public Sprite icon;
        public new string name => LocalisationSystem.GetLocalisedValue(nameKey);
        public string nameKey;
        
        public virtual void OnCompletion(NPC questGiver = null)
        {
            Debug.Log($"{name}: Quest completed!");
        }
        
        public float GetPercentage() {
            float total = 0;
            foreach (Objective objective in objectives) {
                total += objective.GetPercentage(tracker);
            }
            return total / objectives.Length;
        }
    }
}