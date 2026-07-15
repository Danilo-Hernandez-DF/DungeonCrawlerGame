namespace ProcGen {
    [CreateAssetMenu(menuName = "RoomBehaviour/CloseDoorsOnEnter")]
    public class CloseDoorsOnStart : RoomBehaviour { 
        public override void OnStart(RoomController room){ 
            room.CloseDoors();
        } 
    }
}
