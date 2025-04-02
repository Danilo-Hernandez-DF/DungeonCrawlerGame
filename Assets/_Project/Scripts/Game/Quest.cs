using System;
using UnityEngine;

namespace Game {
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Basic")]
    public class Quest : ScriptableObject {
        public Objective[] objectives;
        public string name;
        
        public virtual void OnCompletion(NPC questGiver = null) {
            Debug.Log($"{name}: Quest completed!");
        }
    }
}