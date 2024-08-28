using UnityEngine;

namespace UtilsModule {
    public interface IEntityFactory<T> where T : Entity {
        T Create(Transform spawnPoint);
    }
}