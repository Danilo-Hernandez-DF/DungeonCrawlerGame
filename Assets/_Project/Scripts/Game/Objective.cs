using UnityEngine;

namespace Game {
    public class Objective : ScriptableObject{
        public int target;
        
        public virtual int GetProgress(StatisticsTracker tracker) {
            return 0;
        }
    }
}