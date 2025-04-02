using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;
using UtilsModule;

namespace ProcGen {
    public class DungeonGenerator : MonoBehaviour { 
        [SerializeField] int targetRooms = 10;
        [SerializeField] List<RoomRequirements> specialRoomRequirements;
        [SerializeField] GameObject startRoomPrefab;
        [SerializeField] GOLootTable roomPrefabs;
        [SerializeField] GOLootTable bossRooms;
        [SerializeField] LayerMask roomMask;
        //Dictionary<Vector2, RoomController> roomPositions;
        public List<RoomController> rooms { get; private set; }
        List<RoomEntrance> entrances;
        [SerializeField] int seed;

        //void Start() => Generate();

        public void Generate() {
            SeededRandom.SetSeed(seed > 0 ? seed : Random.Range(0, int.MaxValue));
            seed = SeededRandom.GetSeed();

            rooms = new List<RoomController>();
            //roomPositions = new Dictionary<Vector2, RoomController>();
            entrances = new List<RoomEntrance>();

            int requiredRooms = specialRoomRequirements.Sum(req => req.count);

            targetRooms = targetRooms >= requiredRooms + 4 ? targetRooms : requiredRooms + 4;
            Vector2 roomPos = Vector2.zero;

            var startRoom = Instantiate(startRoomPrefab, roomPos, Quaternion.identity);
            AddRoom(startRoom.GetComponent<RoomController>());

            for(int i = 0; i < targetRooms - requiredRooms; i++) {
                TryGenerateRoom(roomPrefabs, roomPos, entrances);
            }

            foreach(RoomRequirements requirement in specialRoomRequirements) {
                for(int i = 0; i < requirement.count; i++) {
                    TryGenerateRoom(requirement.roomPrefabs, roomPos, entrances, false);
                }
            }

            //Generate boss rooms

            foreach(RoomController room in rooms) {
                room.Init();
            }

            NavMeshManager.BakeNavMesh();
        }

        bool TryGenerateRoom(GOLootTable prefabs, Vector2 roomPos, List<RoomEntrance> roomEntrances, bool addEntrances = true) {
            bool validEntrance = false;

            RoomController room = null;
            RoomEntrance targetEntrance;

            var invalidEntrances = new List<RoomEntrance>();
            RoomEntrance entrance;
            bool noEntrances = false;

            do {
                entrance = roomEntrances[SeededRandom.GetRange(0, roomEntrances.Count)];
                var invalidRooms = new List<RoomController>();
                bool roomsRemaining = true;
                targetEntrance = null;

                if(invalidEntrances.Contains(entrance)) continue;

                do {
                    room = prefabs.GetWeightedItem(seeded: true).GetComponent<RoomController>();
                    if(invalidRooms.Contains(room)) continue;
                        
                    do {
                        targetEntrance = room.GetNextEntrance(entrance.dir.GetOpposite(), targetEntrance);
                        if(targetEntrance == null) {
                            Debug.LogError("No next entrance");
                            break;
                        }
                        roomPos = new Vector2(entrance.Pos.x - targetEntrance.Pos.x, entrance.Pos.y - targetEntrance.Pos.y);
                        if(Physics2D.OverlapBox(roomPos, room.roomSize - (Vector2.one/10), 0, roomMask) == null) validEntrance = true;
                        else break;
                    } while(!validEntrance);

                    if(validEntrance) continue;
                    if(!invalidRooms.Contains(room)) invalidRooms.Add(room);
                    if(invalidRooms.Count == prefabs.GetList().Count) roomsRemaining = false;
                } while(roomsRemaining && !validEntrance);

                if(validEntrance) continue;
                if(!invalidEntrances.Contains(entrance)) invalidEntrances.Add(entrance);
                if(invalidEntrances.Count == roomEntrances.Count) noEntrances = true;
            } while(!validEntrance && !noEntrances);

            if(noEntrances) {
                Debug.LogError("No more entrances to connect to");
                return false;
            }

            var newRoom = Instantiate(room.gameObject, roomPos, Quaternion.identity).GetComponent<RoomController>();
            targetEntrance = newRoom.GetNextEntrance(entrance.dir.GetOpposite());
            targetEntrance.DeActivate();

            foreach(RoomEntrance otherEntrance in newRoom.doorPositions) {
                var foundEntrances = otherEntrance.GetOverlappingEntrances();
                if(foundEntrances.Count > 0) {
                    otherEntrance.DeActivate();
                    if(roomEntrances.Contains(otherEntrance)) roomEntrances.Remove(otherEntrance);
                }

                foreach(RoomEntrance foundEntrance in foundEntrances) {
                    foundEntrance.DeActivate();
                    if(roomEntrances.Contains(foundEntrance)) roomEntrances.Remove(foundEntrance);
                }
            }

            AddRoom(newRoom, addEntrances);
            DungeonController.Instance.OnRoomGeneration(newRoom);
            return true;
        }

        void AddRoom(RoomController room, bool addEntrances = true) {
            //roomPositions.Add(room.transform.position, room);
            rooms.Add(room);

            if(!addEntrances) return;
            foreach(var entrance in room.doorPositions) {
                if(!entrance.Active) continue;
                entrances.Add(entrance);
            }
        }
    }

    [System.Serializable]
    public class RoomRequirements {
        public GOLootTable roomPrefabs;
        public RoomType type;
        public int count;
    }
}