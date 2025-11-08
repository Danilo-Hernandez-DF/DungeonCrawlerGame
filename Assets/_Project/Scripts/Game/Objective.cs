using UnityEngine;

namespace Game {
    public class Objective : ScriptableObject{
        public int target;
        
        public virtual int GetProgress(StatisticsTracker tracker) {
            return 0;
        }
        
        public float GetPercentage(StatisticsTracker tracker) {
            return (float)GetProgress(tracker) / target;
        }
    }
}