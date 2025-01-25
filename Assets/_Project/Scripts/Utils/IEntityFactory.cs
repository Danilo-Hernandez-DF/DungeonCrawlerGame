using UnityEngine;

namespace UtilsModule {
    public interface IEntityFactory<out T> where T : Entity {
        T Create(Transform spawnPoint);
    }
}