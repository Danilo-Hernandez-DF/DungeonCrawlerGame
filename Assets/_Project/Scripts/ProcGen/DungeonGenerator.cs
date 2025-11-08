using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;
using UnityEngine.Tilemaps;
using UtilsModule;

namespace ProcGen {
    public class DungeonGenerator : MonoBehaviour { 
        int targetRooms = 10;
        List<RoomRequirements> specialRoomRequirements;
        [SerializeField] GOLootTable startRoomPrefab;
        [SerializeField] GOLootTable roomPrefabs;
        [SerializeField] LayerMask roomMask;
        public List<RoomController> rooms { get; private set; }
        List<RoomEntrance> entrances;
        [SerializeField] int seed;

        public void Configure(DungeonData dungeonData) {
            targetRooms = dungeonData.roomCount;
            specialRoomRequirements = dungeonData.RoomRequirements();
            startRoomPrefab = dungeonData.dungeonType.startRoomPrefab;
            roomPrefabs = dungeonData.dungeonType.roomPrefabs;
        }

        public void Generate(out RoomController initialRoom) {
            rooms = new List<RoomController>();
            entrances = new List<RoomEntrance>();

            int requiredRooms = specialRoomRequirements.Sum(req => req.count);

            Vector2 roomPos = Vector2.zero;

            var startRoom = Instantiate(startRoomPrefab.GetWeightedItem(), roomPos, Quaternion.identity);
            initialRoom = startRoom.GetComponent<RoomController>();
            AddRoom(initialRoom);

            if (targetRooms == 0)
            {
                initialRoom.Init();
                return;
            }

            targetRooms = Mathf.Max(targetRooms, requiredRooms + 4);

            for (int i = 0; i < targetRooms - requiredRooms; i++)
            {
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
        }

        bool TryGenerateRoom(GOLootTable prefabs, Vector2 roomPos, List<RoomEntrance> roomEntrances, bool addEntrances = true) {
            bool validEntrance = false;

            RoomController room = null;
            RoomEntrance targetEntrance;

            var invalidEntrances = new List<RoomEntrance>();
            RoomEntrance entrance = null;
            bool noEntrances = false;

            while(!validEntrance && !noEntrances) {
                entrance = roomEntrances[Random.Range(0, roomEntrances.Count)];
                var invalidRooms = new List<RoomController>();
                bool roomsRemaining = true;
                targetEntrance = null;

                if(invalidEntrances.Contains(entrance)) continue;

                while(roomsRemaining && !validEntrance) {
                    room = prefabs.GetWeightedItem().GetComponent<RoomController>();
                    if(invalidRooms.Contains(room)) continue;
                        
                    while(!validEntrance) {
                        targetEntrance = room.GetNextEntrance(entrance.dir.GetOpposite(), targetEntrance);
                        if(targetEntrance == null) {
                            Debug.LogError("No next entrance");
                            break;
                        }
                        roomPos = new Vector2(entrance.Pos.x - targetEntrance.Pos.x, entrance.Pos.y - targetEntrance.Pos.y);
                        if(Physics2D.OverlapBox(roomPos, room.roomSize - (Vector2.one/10), 0, roomMask) == null) validEntrance = true;
                        else break;
                    }

                    if(validEntrance) continue;
                    if(!invalidRooms.Contains(room)) invalidRooms.Add(room);
                    if(invalidRooms.Count == prefabs.GetList().Count) roomsRemaining = false;
                }

                if(validEntrance) continue;
                if(!invalidEntrances.Contains(entrance)) invalidEntrances.Add(entrance);
                if(invalidEntrances.Count == roomEntrances.Count) noEntrances = true;
            } 

            if(noEntrances) {
                Debug.LogError("No more entrances to connect to");
                return false;
            }

            var newRoom = Instantiate(room.gameObject, roomPos, Quaternion.identity).GetComponent<RoomController>();
            targetEntrance = newRoom.GetNextEntrance(entrance.dir.GetOpposite());
            targetEntrance.connectedEntrance = entrance;
            entrance.connectedEntrance = targetEntrance;
            targetEntrance.DeActivate();

            foreach(RoomEntrance otherEntrance in newRoom.doorPositions) {
                var foundEntrance = otherEntrance.GetOverlappingEntrance();
                if (!foundEntrance || room.doorPositions.Contains(foundEntrance)) continue;
                
                otherEntrance.DeActivate();
                if (roomEntrances.Contains(otherEntrance)) roomEntrances.Remove(otherEntrance);

                foundEntrance.DeActivate();

                if (!newRoom.connectedRooms.Contains(foundEntrance.room)) newRoom.connectedRooms.Add(foundEntrance.room);
                if (!foundEntrance.room.connectedRooms.Contains(newRoom)) foundEntrance.room.connectedRooms.Add(newRoom);

                foundEntrance.connectedEntrance = otherEntrance;
                otherEntrance.connectedEntrance = foundEntrance;

                if (roomEntrances.Contains(foundEntrance)) roomEntrances.Remove(foundEntrance);
            }

            AddRoom(newRoom, addEntrances);
            DungeonController.Instance.OnRoomGeneration(newRoom);
            return true;
        }

        void AddRoom(RoomController room, bool addEntrances = true) {
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
        
        public RoomRequirements(GOLootTable roomPrefabs, RoomType type, int count) {
            this.roomPrefabs = roomPrefabs;
            this.type = type;
            this.count = count;
        }
    }
    
    [System.Serializable] 
    public class AdditionalRoomRequirements
    {
        public RoomType type;
        public int count;
    }
}