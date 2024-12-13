using System.Collections.Generic;
using System.Linq;
using Game;
using ProcGen;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UtilsModule {
    public class TilemapBuilder { 
        Tilemap tilemap;
        RoomController roomController;

        public TilemapBuilder(Tilemap tilemap, RoomController roomController = null) {
            this.tilemap = tilemap;
            this.roomController = roomController;
        }

        public void CopyFrom(Tilemap other, Vector2Int fromPos, Vector2Int toPos, Vector2Int areaSize) {
            List<TileBase> copyArea = GetTiles(other, fromPos, areaSize);
            SetTiles(tilemap, toPos, copyArea, areaSize);
            tilemap.RefreshAllTiles();
        }

        public List<TileBase> GetTiles(Tilemap map, Vector2Int startPos, Vector2Int areaSize) {
            List<TileBase> areaToCopy = new(); 
            
            for(int x = 0; x < areaSize.x; x++) {
                for(int y = 0; y < areaSize.y; y++) {
                    areaToCopy.Add(map.GetTile((Vector3Int)startPos + new Vector3Int(x, y)));
                }
            }

            return areaToCopy;
        }

        public List<List<TileBase>> GetTiles2D(Tilemap map, Vector2Int startPos, Vector2Int areaSize) {
            List<List<TileBase>> areaToCopy = new();

            for(int x = 0; x < areaSize.x; x++) {
                areaToCopy.Add(new List<TileBase>());
                for(int y = 0; y < areaSize.y; y++) {
                    areaToCopy[x].Add(map.GetTile((Vector3Int)startPos + new Vector3Int(x, y)));
                }
            }

            return areaToCopy;
        }

        public void SetTiles(Tilemap map, Vector2Int startPos, List<TileBase> tiles, Vector2Int areaSize) {
            int i = 0;
            for(int x = 0; x < areaSize.x; x++) {
                for(int y = 0; y < areaSize.y; y++) {
                    if(i>=tiles.Count) break;
                    map.SetTile((Vector3Int)startPos + new Vector3Int(x, y), tiles[i]);
                    i++;
                }
            }
        }

        public void RemovePlaceholders(Vector2Int pos, Vector2Int areaSize) {
            AddPOIs(pos, areaSize);
            SpawnEntities(pos, areaSize, tilemap.transform.position);
            List<TileBase> boundsArea = GetTiles(tilemap, pos, areaSize);
            List<TileBase> replaceArea = new List<TileBase>();

            foreach(TileBase tile in boundsArea) {
                TileData tileData = TilemapManager.Instance.GetTileData(tile);

                if(tileData == null) {
                    replaceArea.Add(tile);
                    continue;
                }

                if(tileData.replaceTiles == null || tileData.tileType == TileType.Normal) {
                    replaceArea.Add(tile);
                    continue;
                }

                if(tileData.tileType == TileType.Placeholder) {
                    replaceArea.Add(tileData.GetTile(true));
                }
            }

            SetTiles(tilemap, pos, replaceArea, areaSize);
        }

        public void SpawnEntities(Vector2Int pos, Vector2Int areaSize, Vector2 centerPos) {
            List<List<TileBase>> boundsArea = GetTiles2D(tilemap, pos, areaSize);

            for(int x = 0; x < areaSize.x; x++) {
                for(int y = 0; y < areaSize.y; y++) {
                    var tile = TilemapManager.Instance.GetTileData(boundsArea[x][y]);
                    if(tile == null) continue;
                    if(tile.tileType != TileType.SpawnPoint) continue;

                    tilemap.SetTile(new Vector3Int(-Mathf.FloorToInt(areaSize.x/2) + x, 
                        -Mathf.FloorToInt(areaSize.y/2) + y), tile.GetTile(true));

                    var prefab = tile.prefabs.GetWeightedItem(seeded: true);
                    Vector2 position = new Vector2Int(-Mathf.FloorToInt(areaSize.x/2) + x, 
                        -Mathf.FloorToInt(areaSize.y/2) + y) + new Vector2(0.5f, 0.5f) + centerPos;
                    var entity =GameObject.Instantiate(prefab, position, Quaternion.identity);

                    if(entity.GetComponent<EnemySpawnManager>() != null)
                        roomController?.AddSpawner(entity.GetComponent<EnemySpawnManager>());

                    if(entity.GetComponent<Collectible>() != null)
                        roomController?.AddCollectible(entity.GetComponent<Collectible>());

                    if(entity.GetComponent<Lootable>() != null)
                        roomController?.AddLoot(entity.GetComponent<Lootable>());
                }
            }
        }

        public void AddPOIs(Vector2Int pos, Vector2Int areaSize) {
            List<List<TileBase>> boundsArea = GetTiles2D(tilemap, pos, areaSize);

            for(int x = 0; x < areaSize.x; x++) {
                for(int y = 0; y < areaSize.y; y++) {
                    var tile = TilemapManager.Instance.GetTileData(boundsArea[x][y]);
                    if(tile == null) continue;
                    if(tile.tileType != TileType.POI) continue;

                    var poi = tile.GetPOI(true);
                    if(poi != null) CopyFrom(poi, Vector2Int.zero, new Vector2Int(-Mathf.FloorToInt(areaSize.x/2) + x, 
                        -Mathf.FloorToInt(areaSize.y/2) + y) - tile.poiOffset, tile.poiSize);
                }
            }
        }

        public void CopyAndRemovePlaceholders(Tilemap other, Vector2Int fromBounds, Vector2Int toBounds, Vector2Int areaSize) {
            CopyFrom(other, fromBounds, toBounds, areaSize);
            RemovePlaceholders(toBounds, areaSize);
        }
    }
}