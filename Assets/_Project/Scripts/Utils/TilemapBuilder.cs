using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UtilsModule {
    public class TilemapBuilder {
        Tilemap tilemap;

        public TilemapBuilder(Tilemap tilemap) {
            this.tilemap = tilemap;
        }

        public void CopyFrom(Tilemap other, Vector2Int fromPos, Vector2Int toPos, Vector2Int areaSize) {
            List<TileBase> copyArea = GetTiles(other, fromPos, areaSize);
            SetTiles(tilemap, toPos, copyArea, areaSize);
            //Debug.Log($"replaced {copyArea.Count} tiles for room at x:{toPos.x}, y:{toPos.y}");
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
            List<TileBase> boundsArea = GetTiles(tilemap, pos, areaSize);
            List<TileBase> replaceArea = new List<TileBase>();

            foreach(TileBase tile in boundsArea) {
                TileData tileData = TilemapManager.Instance.GetTileData(tile);

                if(tileData == null) {
                    replaceArea.Add(null);
                    continue;
                }
                
                TileBase[] replaceTiles = tileData.replaceTiles;
                if(replaceTiles?.Length < 1 || !tileData.isPlaceholder) {
                    replaceArea.Add(tile);
                    continue;
                }
                
                WeightedTable<TileBase> weightedTable = new WeightedTable<TileBase>(replaceTiles.ToList(), replaceTiles.Select(i => 
                    TilemapManager.Instance.GetTileData(i).placementWeight).ToList());
                replaceArea.Add(weightedTable.GetWeightedT());
            }

            SetTiles(tilemap, pos, replaceArea, areaSize);
        }

        public void CopyAndRemovePlaceholders(Tilemap other, Vector2Int fromBounds, Vector2Int toBounds, Vector2Int areaSize) {
            CopyFrom(other, fromBounds, toBounds, areaSize);
            RemovePlaceholders(toBounds, areaSize);
        }
    }
}