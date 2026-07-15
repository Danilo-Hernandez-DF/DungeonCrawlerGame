using Game;

namespace ProcGen {
    [CreateAssetMenu(menuName = "RoomBehaviour/StartSpawnersOnEnter")]
    public class StartSpawnersOnStart : RoomBehaviour {
        public override void OnStart(RoomController room) {
            foreach(EnemySpawnManager spawner in room.EnemySpawners) {
                spawner.Activate();
            }
        }
    }
}