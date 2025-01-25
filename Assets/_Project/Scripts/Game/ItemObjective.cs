using System;
using UnityEngine;
using UtilsModule;

namespace Game {
    [CreateAssetMenu(fileName = "New Item Objective", menuName = "Objective/Item")]
    public class ItemObjective : Objective {
        public ItemData tracked;
        
        public override int GetProgress(StatisticsTracker tracker) {
            int current = tracker.GetTracked(tracked);
            return target - current;
        }
    }
}