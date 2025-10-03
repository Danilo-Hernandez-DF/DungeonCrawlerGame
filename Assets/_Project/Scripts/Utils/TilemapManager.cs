using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UtilsModule {
    public class TilemapManager : Singleton<TilemapManager> {
        [SerializeField] Tilemap map;

        [SerializeField] List<TileData> tileData;
        Dictionary<TileBase, TileData> dataFromTiles;

        protected override void Awake() {
            tileData = Resources.LoadAll<TileData>("TileData").ToList();

            dataFromTiles = new Dictionary<TileBase, TileData>();

            foreach(var data in tileData) {
                foreach(var tile in data.tiles) {
                    dataFromTiles.Add(tile, data);
                }
            }

            base.Awake();
        }

        private TileBase GetTile(Vector2 pos) {
            Vector3Int gridPos = map.WorldToCell(pos);
            TileBase tile = map.GetTile(gridPos);

            return tile; 
        }

        public TileData GetTileData(TileBase tile) {
            if(tile == null || !dataFromTiles.TryGetValue(tile, out var tileData)) return null;

            return tileData;
        }

        public TileData GetTileData(Vector2 pos) {
            var tile = GetTile(pos);

            return GetTileData(tile);
        }
    }
}