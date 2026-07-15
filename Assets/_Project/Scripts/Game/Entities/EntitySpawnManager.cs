using Utils;

namespace Game {
    public abstract class EntitySpawnManager : MonoBehaviour {
        [SerializeField] protected SpawnPointStrategyType spawnPointStrategyType = SpawnPointStrategyType.Linear;
        [SerializeField] protected Transform[] spawnPoints;
        protected ISpawnPointStrategy SpawnPointStrategy;

        protected enum SpawnPointStrategyType {
            Linear,
            Random
        }

        protected virtual void Awake() {
            SpawnPointStrategy = spawnPointStrategyType switch {
                SpawnPointStrategyType.Linear => new LinearSpawnPointStrategy(spawnPoints),
                SpawnPointStrategyType.Random => new RandomSpawnPointSrategy(spawnPoints),
                _ => SpawnPointStrategy
            };
        }

        public abstract void Spawn();
    }
}