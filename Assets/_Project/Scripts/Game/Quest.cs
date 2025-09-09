using System;
using Localisation;
using UnityEngine;

namespace Game {
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Basic")]
    public class Quest : ScriptableObject {
        public Objective[] objectives;
        public new string name => LocalisationSystem.GetLocalisedValue(nameKey);
        public string nameKey;
        
        public virtual void OnCompletion(NPC questGiver = null)
        {
            Debug.Log($"{name}: Quest completed!");
        }
    }
}