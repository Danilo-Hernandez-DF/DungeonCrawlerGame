using UnityEngine;

namespace UtilsModule {
    public interface ISpawnPointStrategy {
        Transform NextSpawnPoint();
    }
}