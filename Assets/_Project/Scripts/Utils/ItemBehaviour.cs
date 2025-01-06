using UnityEngine;

namespace UtilsModule {
    public abstract class ItemBehaviour : ScriptableObject {
        public BehaviourType behaviourType;
        public abstract void ExecuteBehaviour(Inventory source, int indexSource, Entity entitySource = null, int addData = 0);
    }

    public enum BehaviourType {
        OnUse,
        OnHitSelf,
        OnHitOther,
        OnPickup,
        OnKill
    }
}