namespace Game {
    public class OnUseBehaviour : ItemBehaviour {
        [SerializeField] bool consumesItem = false;

        public override void ExecuteBehaviour(Inventory source, int indexSource, Entity entitySource = null, int addData = 0) {
            if(consumesItem) source.RemoveAt(indexSource);
            OnUse(source, indexSource, entitySource, addData);
        }

        protected virtual void OnUse(Inventory source, int indexSource, Entity entitySource = null, int addData = 0) { }
    }
}