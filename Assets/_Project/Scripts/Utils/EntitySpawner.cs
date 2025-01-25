namespace UtilsModule {
    public class EntitySpawner<T> where T : Entity {
        readonly IEntityFactory<T> entityFactory;
        readonly ISpawnPointStrategy spawnPointStrategy;

        public EntitySpawner(IEntityFactory<T> entityFactory, ISpawnPointStrategy spawnPointStrategy) {
            this.entityFactory = entityFactory;
            this.spawnPointStrategy = spawnPointStrategy;
        }

        public T Spawn() {
            return entityFactory.Create(spawnPointStrategy.NextSpawnPoint());
        }
    }
}