namespace ProcGen {
    [CreateAssetMenu(menuName = "RoomBehaviour/CompoundRoomBehaviour")]
    public class CompoundRoomBehaviour : RoomBehaviour {
        [SerializeField] List<RoomBehaviour> subBehaviours = new List<RoomBehaviour>();
        
        public override void OnPlayerEnter(RoomController room) {
            foreach(var behaviour in subBehaviours) {
                behaviour.OnPlayerEnter(room);
            }
        }
        
        public override void OnStart(RoomController room) {
            foreach(var behaviour in subBehaviours) {
                behaviour.OnStart(room);
            }
        }
        
        public override void OnEnd(RoomController room) {
            foreach(var behaviour in subBehaviours) {
                behaviour.OnEnd(room);
            }
        }
        
        public override void OnUpdate(RoomController room) {
            foreach(var behaviour in subBehaviours) {
                behaviour.OnUpdate(room);
            }
        }
        
        public override void OnPlayerExit(RoomController room) {
            foreach(var behaviour in subBehaviours) {
                behaviour.OnPlayerExit(room);
            }
        }
    }
}