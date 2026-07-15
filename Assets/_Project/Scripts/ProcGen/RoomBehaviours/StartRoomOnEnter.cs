namespace ProcGen {
    [CreateAssetMenu(menuName = "RoomBehaviour/StartRoomOnEnter")]
    public class StartRoomOnEnter : RoomBehaviour {
        public override void OnPlayerEnter(RoomController room) {
            room.StartRoom();
        }
    }
}