using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UtilsModule {
    public class RandomSpawnPointSrategy : ISpawnPointStrategy {
        List<Transform> unusedSpawnPoints;
        readonly Transform[] spawnPoints;

        public RandomSpawnPointSrategy(Transform[] spawnPoints) {
            this.spawnPoints = spawnPoints;
            unusedSpawnPoints = new List<Transform>(spawnPoints);
        }

        public Transform NextSpawnPoint() {
            if(!unusedSpawnPoints.Any()) {
                unusedSpawnPoints = new List<Transform>(spawnPoints);
            }

            var randomIndex = Random.Range(0, unusedSpawnPoints.Count);
            Transform result = unusedSpawnPoints[randomIndex];
            unusedSpawnPoints.RemoveAt(randomIndex);
            return result;
        }
    }
}