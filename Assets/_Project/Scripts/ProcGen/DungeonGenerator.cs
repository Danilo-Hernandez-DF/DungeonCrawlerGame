using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UtilsModule;

namespace ProcGen {
    public class DungeonGenerator : MonoBehaviour {
        private RoomType[,] rooms;
        int size;
        List<Vector2Int> deadEnds;
        List<Vector2Int> populatedRooms;

        [Header("Generation data")]
        [SerializeField, Min(0)] int seed;
        [SerializeField, Min(10)] int roomCount;
        [SerializeField, Min(1)] int walkers;
        [SerializeField] Vector2Int roomSize = new(16, 9);

        [Header("Input")]

        [Header("Tilemap References")]
        [SerializeField] Tilemap groundTilemap;
        [SerializeField] Tilemap objectTilemap;
        [SerializeField] Tilemap mapTilemap;

        [Header("Prefab Tilemap references")]
        [SerializeField] Tilemap roomExits;
        [SerializeField] Tilemap roomMapOutlines;
        [SerializeField] Tilemap roomMapExits;

        [Header("Prefab Room pools")]
        [SerializeField] Transform[] bossTilemaps;
        [SerializeField] Transform[] treasureTilemaps;
        [SerializeField] Transform[] shopTilemaps;
        [SerializeField] Transform[] hostileTilemaps;
        [SerializeField] Transform[] emptyRoomTilemaps;
        [SerializeField] Transform[] startTilemaps;

        [Header("Prefab references")]
        [SerializeField] GameObject roomManager;

        int radius => roomCount/2*walkers;

        WeightedTable<Transform> bossRooms;
        WeightedTable<Transform> treasureRooms;
        WeightedTable<Transform> shopRooms;
        WeightedTable<Transform> hostileRooms;
        WeightedTable<Transform> emptyRooms;
        WeightedTable<Transform> startRooms;

        Dictionary<RoomType, WeightedTable<Transform>> roomPools;

        void Awake() {
            if(seed > 0) SeededRandom.SetSeed(seed);
            else SeededRandom.SetSeed(Random.Range(1, int.MaxValue));
            seed = SeededRandom.GetSeed();

            bossRooms = new WeightedTable<Transform>(bossTilemaps.ToList(), new List<int>());
            treasureRooms = new WeightedTable<Transform>(treasureTilemaps.ToList(), new List<int>());
            shopRooms = new WeightedTable<Transform>(shopTilemaps.ToList(), new List<int>());
            hostileRooms = new WeightedTable<Transform>(hostileTilemaps.ToList(), new List<int>());
            emptyRooms = new WeightedTable<Transform>(emptyRoomTilemaps.ToList(), new List<int>());
            startRooms = new WeightedTable<Transform>(startTilemaps.ToList(), new List<int>());

            roomPools = new() {{RoomType.BossRoom, bossRooms}, {RoomType.TreasureRoom, treasureRooms}, {RoomType.ShopRoom, shopRooms},
            {RoomType.HostileRoom, hostileRooms}, {RoomType.EmptyRoom, emptyRooms}, {RoomType.StartRoom, startRooms}};
        }

        void Start() {
            //temp
            Generate();
        }

        public void Generate() {
            PopulateGrid();
            PopulateTilemaps();
            NavMeshManager.BakeNavMesh();
        }

        void PopulateTilemaps() {
            TilemapBuilder builder = new(groundTilemap);
            TilemapBuilder objectBuilder = new(objectTilemap);
            TilemapBuilder outlineBuilder = new(mapTilemap);

            foreach(Vector2Int roomPos in populatedRooms) {
                var roomType = rooms[roomPos.x, roomPos.y];
                Transform toCopy;

                toCopy = roomPools[roomType].GetWeightedT(seeded: true);

                Vector2Int startTile = Vector2Int.Scale(roomPos - new Vector2Int(radius, radius), roomSize) - 
                new Vector2Int(Mathf.RoundToInt(roomSize.x/2), Mathf.RoundToInt(roomSize.y/2));

                Vector3 roomWorldPos = groundTilemap.CellToWorld((Vector3Int)startTile) + new Vector3(roomSize.x/2, (roomSize.y/2) + 0.5f);
                RoomController room = Instantiate(roomManager, roomWorldPos, Quaternion.identity).GetComponent<RoomController>();
                room.col.size = roomSize - Vector2.one; 
                
                builder.CopyAndRemovePlaceholders(toCopy.GetChild(0).GetComponent<Tilemap>(), new(-8, -4), startTile, roomSize);
                objectBuilder.CopyAndRemovePlaceholders(toCopy.GetChild(1).GetComponent<Tilemap>(), new(-8, -4), startTile, roomSize);
                outlineBuilder.CopyFrom(roomMapOutlines, new(-8, -4), startTile, roomSize);

                List<Dir> exits = GetNeighbours(roomPos);
                room.exits = exits;

                if(exits.Contains(Dir.North)) {
                    builder.CopyAndRemovePlaceholders(roomExits, new(-6, 2), startTile + new Vector2Int(6, 8), new(4, 1));
                    outlineBuilder.CopyFrom(roomMapExits, new(-6, 2), startTile + new Vector2Int(6, 8), new(4, 1));
                } else {
                    builder.CopyAndRemovePlaceholders(roomExits, new(-6, 3), startTile + new Vector2Int(6, 8), new(4, 1));
                    outlineBuilder.CopyFrom(roomMapExits, new(-6, 3), startTile + new Vector2Int(6, 8), new(4, 1));
                }

                if(exits.Contains(Dir.South)) {
                    builder.CopyAndRemovePlaceholders(roomExits, new(-6, -3), startTile + new Vector2Int(6, 0), new(4, 1));
                    outlineBuilder.CopyFrom(roomMapExits, new(-6, -3), startTile + new Vector2Int(6, 0), new(4, 1));
                } else {
                    builder.CopyAndRemovePlaceholders(roomExits, new(-6, -2), startTile + new Vector2Int(6, 0), new(4, 1));
                    outlineBuilder.CopyFrom(roomMapExits, new(-6, -2), startTile + new Vector2Int(6, 0), new(4, 1));
                }

                if(exits.Contains(Dir.East)) {
                    builder.CopyAndRemovePlaceholders(roomExits, new(-2, -1), startTile + new Vector2Int(15, 3), new(1, 3));
                    outlineBuilder.CopyFrom(roomMapExits, new(-2, -1), startTile + new Vector2Int(15, 3), new(1, 3));
                } else {
                    builder.CopyAndRemovePlaceholders(roomExits, new(-1, -1), startTile + new Vector2Int(15, 3), new(1, 3));
                    outlineBuilder.CopyFrom(roomMapExits, new(-1, -1), startTile + new Vector2Int(15, 3), new(1, 3));
                }

                if(exits.Contains(Dir.West)) {
                    builder.CopyAndRemovePlaceholders(roomExits, new(-8, -1), startTile + new Vector2Int(0, 3), new(1, 3));
                    outlineBuilder.CopyFrom(roomMapExits, new(-8, -1), startTile + new Vector2Int(0, 3), new(1, 3));
                } else {
                    builder.CopyAndRemovePlaceholders(roomExits, new(-7, -1), startTile + new Vector2Int(0, 3), new(1, 3));
                    outlineBuilder.CopyFrom(roomMapExits, new(-7, -1), startTile + new Vector2Int(0, 3), new(1, 3));
                }
            }

            objectTilemap.RefreshAllTiles();
            groundTilemap.RefreshAllTiles();
            mapTilemap.RefreshAllTiles();
        }

        void PopulateGrid() {
            populatedRooms = new();
            deadEnds = new();
            size = (radius*2) + 1;
            rooms = new RoomType[size,size];

            rooms[radius, radius] = RoomType.StartRoom;
            populatedRooms.Add(new(radius, radius));

            for(int i = 0; i < walkers; i++) {
                RandomWalker(new(radius,radius), roomCount);
            }

            SelectDeadEnds();

            foreach(Vector2Int deadEnd in deadEnds) {
                if(rooms[deadEnd.x, deadEnd.y] == RoomType.StartRoom) continue;

                if(deadEnd.x != 0) if(rooms[deadEnd.x-1, deadEnd.y] == RoomType.StartRoom) continue;
                if(deadEnd.x != size-1) if(rooms[deadEnd.x+1, deadEnd.y] == RoomType.StartRoom) continue;
                if(deadEnd.y != 0) if(rooms[deadEnd.x, deadEnd.y-1] == RoomType.StartRoom) continue;
                if(deadEnd.y != size-1) if(rooms[deadEnd.x, deadEnd.y+1] == RoomType.StartRoom) continue;

                rooms[deadEnd.x, deadEnd.y] = RoomType.BossRoom;
                break;
            }

            int rand;

            do {
                rand = SeededRandom.GetRange(0, deadEnds.Count);
            } while(rooms[deadEnds[rand].x, deadEnds[rand].y] != RoomType.PlaceholderRoom);

            rooms[deadEnds[rand].x, deadEnds[rand].y] = RoomType.ShopRoom; 

            do {
                rand = SeededRandom.GetRange(0, deadEnds.Count);
            } while(rooms[deadEnds[rand].x, deadEnds[rand].y] != RoomType.PlaceholderRoom);

            rooms[deadEnds[rand].x, deadEnds[rand].y] = RoomType.TreasureRoom; 

            for(int x = 0; x < size; x++) {
                for(int y = 0; y < size; y++) {
                    if(rooms[x,y] == RoomType.PlaceholderRoom) {
                        rand = SeededRandom.GetRange(0, 10);
                        if(rand == 0) rooms[x,y] = RoomType.EmptyRoom;
                        else rooms[x,y] = RoomType.HostileRoom;
                    }
                }
            }
        }

        void SelectDeadEnds() {
            foreach(Vector2Int roomPos in populatedRooms) {
                if(IsDeadEnd(roomPos)) deadEnds.Add(roomPos);
            }

            if(deadEnds.Count < 4) {
                AddDeadEnds(4-deadEnds.Count);
            }
        }

        bool IsDeadEnd(Vector2Int pos) {
            return GetNeighbours(pos).Count == 1;
        }

        List<Dir> GetNeighbours(Vector2Int pos) {
            List<Dir> neighbours = new List<Dir>();
            if(pos.x != 0) if(rooms[pos.x-1, pos.y] != RoomType.NoRoom) neighbours.Add(Dir.West);
            if(pos.x != size-1) if(rooms[pos.x+1, pos.y] != RoomType.NoRoom) neighbours.Add(Dir.East);
            if(pos.y != 0) if(rooms[pos.x, pos.y-1] != RoomType.NoRoom) neighbours.Add(Dir.South);
            if(pos.y != size-1) if(rooms[pos.x, pos.y+1] != RoomType.NoRoom) neighbours.Add(Dir.North);

            return neighbours;
        }

        void AddDeadEnds(int count) {
            int added = 0;

            var tempPopulated = new List<Vector2Int>(populatedRooms);

            foreach(Vector2Int roomPos in tempPopulated) {
                if(deadEnds.Contains(roomPos)) continue;
                if(rooms[roomPos.x, roomPos.y] == RoomType.StartRoom) continue;

                if(roomPos.x != 0) if(!deadEnds.Contains(new(roomPos.x-1, roomPos.y)) && IsDeadEnd(new(roomPos.x-1, roomPos.y))) {
                    PopulateRoom(roomPos.x-1, roomPos.y);
                    deadEnds.Add(new(roomPos.x-1, roomPos.y));
                    added++;
                    //Debug.Log("Room Added");
                }

                if(roomPos.x != size-1) if(!deadEnds.Contains(new(roomPos.x+1, roomPos.y)) && IsDeadEnd(new(roomPos.x+1, roomPos.y))) {
                    PopulateRoom(roomPos.x+1, roomPos.y);
                    deadEnds.Add(new(roomPos.x+1, roomPos.y));
                    added++;
                    //Debug.Log("Room Added");
                }

                if(roomPos.y != 0) if(!deadEnds.Contains(new(roomPos.x, roomPos.y-1)) && IsDeadEnd(new(roomPos.x, roomPos.y-1))) {
                    PopulateRoom(roomPos.x, roomPos.y-1);
                    deadEnds.Add(new(roomPos.x, roomPos.y-1));
                    added++;
                    //Debug.Log("Room Added");
                }

                if(roomPos.y != size-1) if(!deadEnds.Contains(new(roomPos.x, roomPos.y+1)) && IsDeadEnd(new(roomPos.x, roomPos.y+1))) {
                    PopulateRoom(roomPos.x, roomPos.y+1);
                    deadEnds.Add(new(roomPos.x, roomPos.y+1));
                    added++;
                    //Debug.Log("Room Added");
                }

                if(added >= count) break;
            }
        }

        void RandomWalker(Vector2Int startPos, int steps) {
            Vector2Int currentPos = startPos;
            Vector2Int lastPos = startPos;
            Vector2Int dir;

            for(int i = 0; i < steps; i++) {
                if(SeededRandom.GetRange(0, 2) == 0) {
                    dir = SeededRandom.GetRange(0, 2) == 0? new(0, 1):  new(0, -1);
                } else {
                    dir = SeededRandom.GetRange(0, 2) == 0? new(1, 0):  new(-1, 0);
                }

                bool success = true;

                do {
                    currentPos += dir;

                    if(currentPos.x >= size || currentPos.x < 0 || currentPos.y >= size || currentPos.y < 0) {
                        i--;
                        currentPos = lastPos;
                        success = false;
                        break;
                    }
                } while(rooms[currentPos.x, currentPos.y] != RoomType.NoRoom);

                if(success) {
                    lastPos = currentPos;
                } else continue;

                PopulateRoom(currentPos.x, currentPos.y);
            }
        }

        void PopulateRoom(int x, int y) {
            rooms[x, y] = RoomType.PlaceholderRoom;
            populatedRooms.Add(new(x, y));
        }

        void Refresh() {
            NavMeshManager.Instance.ClearData();
            groundTilemap.ClearAllTiles();
            mapTilemap.ClearAllTiles();

            MyUtils.ClearLogConsole();
            Generate();
        }
    }
}
