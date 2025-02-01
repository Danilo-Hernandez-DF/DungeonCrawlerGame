using UnityEngine;
using UtilsModule;

namespace Game {
    [CreateAssetMenu(fileName = "New Entity Objective", menuName = "Objective/Entity")]
    public class EntityObjective : Objective {
        public EntityData tracked;
        public new EntityTrack target; 
        
        public override int GetProgress(StatisticsTracker tracker) {
            EntityTrack current = tracker.GetTracked(tracked);
            return target - current;
        }
    }
}