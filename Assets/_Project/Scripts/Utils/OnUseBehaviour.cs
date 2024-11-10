using UnityEngine;

namespace UtilsModule {
    public class OnUseBehaviour : ItemBehaviour {
        [SerializeField] bool consumesItem = false;

        public override void ExecuteBehaviour(Inventory source, int indexSource, Entity entitySource = null) {
            if(consumesItem) source.RemoveAt(indexSource);
            OnUse(source, indexSource, entitySource);
        }

        protected virtual void OnUse(Inventory source, int indexSource, Entity entitySource = null) { }
    }
}