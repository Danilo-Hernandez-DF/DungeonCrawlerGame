using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;
using UnityEngine.Tilemaps;
using UtilsModule;

namespace ProcGen {
    public class RoomController :  MonoBehaviour { 
        [Header("Room Settings")]
        public Vector2Int roomSize;
        public DungeonType dungeonType;
        public Color roomColor;
        public RoomType roomType;
        [SerializeField] List<RoomBehaviour> roomBehaviours;
        [SerializeField] List<EnemySpawnManager> enemySpawners;
        [SerializeField] List<Lootable> lootables;
        [SerializeField] List<Collectible> collectibles;
        public List<RoomEntrance> doorPositions;
        public List<RoomController> connectedRooms;
        public bool visited;
        
        [Header("Tilemap Settings")]
        [SerializeField] Tilemap[] tilemaps;
        [SerializeField] bool replaceTiles = true;
        public Tilemap map;
        
        [Header("EnemySettings")]
        readonly List<TilemapBuilder> tilemapBuilders = new List<TilemapBuilder>();
        
        List<Door> doors;
        List<GameObject> walls;
        BoxCollider2D col;
        bool active = false;
        bool cleared = false;
        
        public List<EnemySpawnManager> EnemySpawners => enemySpawners;
        public List<Lootable> Lootables => lootables;
        public List<Collectible> Collectibles => collectibles;
        
        public void Reset() {
            active = false;
            cleared = false;
            
            foreach(EnemySpawnManager spawner in enemySpawners) {
                spawner.Reset();
            }

            foreach(Lootable loot in lootables) {
                loot.Reset();
            }
        }

        public int EnemyCount {
            get {
                return enemySpawners.Sum(spawner => spawner.EnemyCount);
            }
        }

        public bool SpawnersExhausted {
            get {
                return enemySpawners.All(spawner => spawner.Exhausted);
            }
        }

        void Awake() {
            col = GetComponent<BoxCollider2D>();
            doors = new List<Door>();
            walls = new List<GameObject>();
            map.GetComponent<Renderer>().enabled = false;
            connectedRooms = new List<RoomController>();
            doorPositions.ForEach(entrance => entrance.room = this);
        }

        public void Init() {
            map.GetComponent<Renderer>().material.SetColor("_Color", roomColor);
            
            foreach(RoomEntrance entrance in doorPositions) {
                if(entrance.Active) {
                    var wall = Instantiate(entrance.wallPrefab, entrance.transform.position, Quaternion.identity);
                    wall.transform.SetParent(this.transform);

                    var wallRender = wall.transform.GetChild(1).GetComponentInChildren<Renderer>();

                    wallRender.material.SetColor("_Color", roomColor);
                    wallRender.enabled = false;

                    walls.Add(wall);
                    
                    continue;
                }

                var door = Instantiate(entrance.doorPrefab, entrance.transform.position, Quaternion.identity)
                    .GetComponent<Door>();
                door.entrance = entrance;
                door.connectedRoom = this;
                door.secondaryRoom = entrance.connectedEntrance.room;
                var doorRender = door.transform.GetChild(0).GetComponentInChildren<Renderer>();
                doorRender.material.SetColor("_Color", door.color);
                doorRender.enabled = false;
                
                doors.Add(door);
            }
            
            foreach(Door door in doors) {
                door.gameObject.transform.SetParent(this.transform);
            }

            foreach(Tilemap tilemap in tilemaps)
                tilemapBuilders.Add(new TilemapBuilder(tilemap, this));
            
            Vector2Int startPos =  new Vector2Int(-Mathf.FloorToInt(roomSize.x/2), -Mathf.FloorToInt(roomSize.y/2));

            if(!replaceTiles) return;
            foreach (TilemapBuilder builder in tilemapBuilders) {
                foreach (GameObject child in builder.RemovePlaceholders(startPos, roomSize)) {
                    child.transform.SetParent(this.transform);
                }
            }

            //CloseDoors();
        }

        public RoomEntrance GetNextEntrance(Dir direction, RoomEntrance currentEntrance = null) {
            int startIndex = doorPositions.FindIndex(x => x == currentEntrance) >= 0 ? doorPositions.FindIndex(x => x == currentEntrance) : 0;
            for(int i = startIndex; i < doorPositions.Count; i++) {
                if(!doorPositions[i].Active) continue;
                if(doorPositions[i].dir == direction) return doorPositions[i];
            }

            return null;
        }

        public void AddSpawner(EnemySpawnManager spawner) => enemySpawners.Add(spawner);
        public void AddLoot(Lootable loot) => lootables.Add(loot);
        public void AddCollectible(Collectible collectible) => collectibles.Add(collectible);

        public void CloseDoors() => doors.ForEach(door => door.Close());
        public void OpenDoors() => doors.ForEach(door => door.Open());

        protected virtual void OnPlayerEnter() {
            if(!visited) Visit();
            
            foreach(RoomBehaviour behaviour in roomBehaviours) {
                behaviour.OnPlayerEnter(this);
            }
            
            DungeonController.Instance.OnRoomEntered(this);
        }

        protected virtual void OnPlayerExit() {
            foreach(RoomBehaviour behaviour in roomBehaviours) {
                behaviour.OnPlayerExit(this);
            }
        }
        
        private void Visit() {
            visited = true;

            map.GetComponent<Renderer>().enabled = true;
            doors.ForEach(door => door.transform.GetChild(0).GetComponentInChildren<Renderer>().enabled = true);
            walls.ForEach(wall => wall.transform.GetChild(1).GetComponentInChildren<Renderer>().enabled = true);

            foreach (var room in connectedRooms) {
                foreach (var door in room.doors) {
                    if (door.secondaryRoom == this) {
                        door.transform.GetChild(0).GetComponentInChildren<Renderer>().enabled = true;
                    }
                }
            }
        }

        public virtual void End() {
            active = false;
            cleared = true;
            foreach(RoomBehaviour behaviour in roomBehaviours) {
                behaviour.OnEnd(this);
            }
            GameManager.Instance.TrackStat(StatisticsTracker.TrackedStat.RoomsCleared, 1);
            DungeonController.Instance.OnRoomClear(this);
        }

        public virtual void StartRoom() {
            if(cleared) return;
            DungeonController.Instance.OnRoomStart(this);
            active = true;
            foreach(RoomBehaviour behaviour in roomBehaviours) {
                behaviour.OnStart(this);
            }
        }

        protected virtual void Update() {
            if(!active) return;
            foreach(RoomBehaviour behaviour in roomBehaviours) {
                behaviour.OnUpdate(this);
            }
        }

        void OnTriggerEnter2D(Collider2D other) {
            if(!other.CompareTag("Player")) return;
            
            Invoke(nameof(OnPlayerEnter), 0.7f);
        }

        void OnTriggerExit2D(Collider2D other) {
            if(!other.CompareTag("Player")) return;

            Invoke(nameof(OnPlayerExit), 0.7f);
        }
    }
}
