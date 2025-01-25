using UnityEngine;

namespace Game {
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quest")]
    public class Quest : ScriptableObject {
        public Objective[] objectives;
        public string name;
        
        public virtual void OnCompletion() {
            Debug.Log($"{name}: Quest completed!");
        }
    }
}