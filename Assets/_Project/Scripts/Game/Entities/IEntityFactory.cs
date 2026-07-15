namespace Game {
    public interface IEntityFactory<out T> where T : Entity {
        T Create(Transform spawnPoint);
    }
}