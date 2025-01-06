using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UtilsModule;

namespace ProcGen {
    public class DungeonGenerator : MonoBehaviour { 
        [SerializeField] int targetRooms = 10;
        [SerializeField] List<RoomRequirements> specialRoomRequirements;
        [SerializeField] GameObject startRoomPrefab;
        [SerializeField] GOLootTable roomPrefabs;
        [SerializeField] GOLootTable bossRooms;
        [SerializeField] LayerMask roomMask;
        Dictionary<Vector2, RoomController> roomPositions;
        List<RoomController> rooms;
        List<RoomEntrance> entrances;
        [SerializeField] int seed = 0;

        void Start() => Generate();

        public void Generate() {
            SeededRandom.SetSeed(seed > 0 ? seed : Random.Range(0, int.MaxValue));
            seed = SeededRandom.GetSeed();

            rooms = new List<RoomController>();
            roomPositions = new Dictionary<Vector2, RoomController>();
            entrances = new List<RoomEntrance>();

            int requiredRooms = 0;
            foreach(var requirement in specialRoomRequirements) {
                requiredRooms += requirement.count;
            }

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

        bool TryGenerateRoom(GOLootTable roomPrefabs, Vector2 roomPos, List<RoomEntrance> entrances, bool addEntrances = true) {
            bool validEntrance = false;

            RoomController room = null;
            RoomEntrance targetEntrance = null;

            var invalidEntrances = new List<RoomEntrance>();
            RoomEntrance entrance = null;
            bool noEntrances = false;

            do {
                entrance = entrances[SeededRandom.GetRange(0, entrances.Count)];
                var invalidRooms = new List<RoomController>();
                bool roomsRemaining = true;
                targetEntrance = null;

                if(invalidEntrances.Contains(entrance)) continue;

                do {
                    room = roomPrefabs.GetWeightedItem(seeded: true).GetComponent<RoomController>();
                    if(invalidRooms.Contains(room)) continue;
                        
                    do {
                        targetEntrance = room.GetNextEntrance(entrance.dir.GetOpposite(), targetEntrance);
                        if(targetEntrance == null) {
                            Debug.LogError("No next entrance");
                            break;
                        }
                        roomPos = new Vector2(entrance.Pos.x - targetEntrance.Pos.x, entrance.Pos.y - targetEntrance.Pos.y);
                        if(Physics2D.OverlapBox(roomPos, room.roomSize - (Vector2.one/10), 0, roomMask) == null) validEntrance = true;
                        else {
                            //Debug.Log("Overlapping with existing room");
                            break;
                        }
                    } while(!validEntrance);

                    if(!validEntrance) {
                        if(!invalidRooms.Contains(room)) invalidRooms.Add(room);
                        if(invalidRooms.Count == roomPrefabs.GetList().Count) roomsRemaining = false;
                    }
                } while(roomsRemaining && !validEntrance);

                if(!validEntrance) {
                    if(!invalidEntrances.Contains(entrance)) invalidEntrances.Add(entrance);
                    if(invalidEntrances.Count == entrances.Count) noEntrances = true;
                }
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
                    if(entrances.Contains(otherEntrance)) entrances.Remove(otherEntrance);
                }

                foreach(RoomEntrance foundEntrance in foundEntrances) {
                    foundEntrance.DeActivate();
                    if(entrances.Contains(foundEntrance)) entrances.Remove(foundEntrance);
                }
            }

            AddRoom(newRoom, addEntrances);
            return true;
        }

        void AddRoom(RoomController room, bool addEntrances = true) {
            roomPositions.Add(room.transform.position, room);
            rooms.Add(room);

            if(addEntrances) {
                foreach(RoomEntrance entrance in room.doorPositions) {
                    if(!entrance.Active) continue;
                    entrances.Add(entrance);
                }
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