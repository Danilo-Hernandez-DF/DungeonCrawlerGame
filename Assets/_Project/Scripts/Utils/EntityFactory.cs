using UnityEngine;

namespace UtilsModule {
    public class EntityFactory<T> : IEntityFactory<T> where T : Entity {
        readonly EntityData[] data;

        public EntityFactory(EntityData[] data) {
            this.data = data;
        }

        public T Create(Transform spawnPoint) {
            EntityData entityData = data[Random.Range(0, data.Length)];
            GameObject instance = Object.Instantiate(entityData.prefab, spawnPoint.position, Quaternion.identity);
            return instance.GetComponent<T>();
        }
    }
}