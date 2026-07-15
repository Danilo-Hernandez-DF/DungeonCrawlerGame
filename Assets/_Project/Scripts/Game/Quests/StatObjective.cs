namespace Game {
    [CreateAssetMenu(fileName = "New Stat Objective", menuName = "Objective/Stat")]
    public class StatObjective : Objective {
        public StatisticsTracker.TrackedStat tracked;
        
        public override int GetProgress(StatisticsTracker tracker) {
            int current = tracker.GetTracked(tracked);
            return target - current;
        }
    }
}