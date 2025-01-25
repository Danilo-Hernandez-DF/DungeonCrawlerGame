using UnityEngine;

namespace ProcGen {
    [CreateAssetMenu(menuName = "RoomBehaviour/OpenDoorsOnClear")]
    public class OpenDoorsOnClear : RoomBehaviour {
        public override void OnEnd(RoomController room) {
            room.OpenDoors();
        }
    }
}