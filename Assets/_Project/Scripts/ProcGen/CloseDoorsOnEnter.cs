using UnityEngine;

namespace ProcGen {
    [CreateAssetMenu(menuName = "RoomBehaviour/CloseDoorsOnEnter")]
    public class CloseDoorsOnEnter : RoomBehaviour { 
        public override void OnPlayerEnter(RoomController room){ 
            room.CloseDoors();
        } 
    }
}
