namespace ProcGen {
    public abstract class RoomBehaviour : ScriptableObject{ 
        public virtual void OnPlayerEnter(RoomController room) { }
        public virtual void OnPlayerExit(RoomController room) { }
        public virtual void OnStart(RoomController room) { }
        public virtual void OnEnd(RoomController room) { }
        public virtual void OnUpdate(RoomController room) { }
    }
}
