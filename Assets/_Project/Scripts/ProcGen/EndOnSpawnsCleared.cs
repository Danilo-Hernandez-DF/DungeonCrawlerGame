using UnityEngine;

namespace ProcGen {
    [CreateAssetMenu(menuName = "RoomBehaviour/EndOnSpawnsCleared")]
    public class EndOnSpawnsCleared : RoomBehaviour {
        public override void OnUpdate(RoomController room) {
            if(room.SpawnersExhausted && room.EnemyCount == 0) {
                room.End();
            }
        }
    }
}