namespace Utils {
    public class LinearSpawnPointStrategy : ISpawnPointStrategy {
        int index = 0;
        readonly Transform[] spawnPoints;

        public LinearSpawnPointStrategy(Transform[] spawnPoints) {
            this.spawnPoints = spawnPoints;
        }

        public Transform NextSpawnPoint() {
            Transform result = spawnPoints[index];
            index = (index + 1) % spawnPoints.Length;
            return result;
        }
    }
}