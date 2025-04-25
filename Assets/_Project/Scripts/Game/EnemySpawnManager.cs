using System.Collections.Generic;
using UnityEngine;
using UtilsModule;

namespace Game {
    public class EnemySpawnManager : EntitySpawnManager {
        [SerializeField] int targetEnemies;
        [SerializeField] EntityData[] enemyData;
        [SerializeField] int enemiesPerWave;
        [SerializeField] int maxEnemies;
        [SerializeField] float spawnInterval;
        int spawnedEnemies = 0;
        public bool Exhausted { get; protected set; } = false;
        List<GameObject> enemies;
        EntitySpawner<Enemy> spawner;
        CountdownTimer spawnTimer;
        public int EnemyCount => enemies.Count;


        protected override void Awake() {
            base.Awake();

            spawner = new EntitySpawner<Enemy>(new EntityFactory<Enemy>(enemyData), SpawnPointStrategy);
            enemies = new List<GameObject>();

            spawnTimer = new CountdownTimer(spawnInterval);
            spawnTimer.OnTimerStop += () => {
                if(enemies.Count + enemiesPerWave <= maxEnemies) {
                    if(enemiesPerWave + spawnedEnemies <= targetEnemies) {
                        for(int i = 0; i < enemiesPerWave; i++) {
                            Spawn();
                        }

                        if (spawnedEnemies == targetEnemies) {
                            spawnTimer.Stop();
                            Exhausted = true;
                            return;
                        }
                    } else if(spawnedEnemies < targetEnemies) {
                        for(int i = 0; i < targetEnemies - spawnedEnemies; i++) {
                            Spawn();
                        }

                        spawnTimer.Stop();
                        Exhausted = true;
                        return;
                    }
                }
                spawnTimer.Start();
            };
        }

        public void Activate() => spawnTimer.Start();
        public void Deactivate() => spawnTimer.Stop();
        public void Reset() {
            spawnedEnemies = 0;
            Exhausted = false;
            Activate();
        }

        void Update() {
            for(int i = 0; i < enemies.Count; i++) {
                if(enemies[i]) continue;
                enemies.RemoveAt(i);
                i--;
            }

            spawnTimer.Tick(Time.deltaTime);        
        }

        public override void Spawn() {
            spawnedEnemies++;
            enemies.Add(spawner.Spawn().gameObject);
        }
    }
}