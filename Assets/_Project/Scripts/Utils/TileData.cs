using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Tile Data", menuName = "Data/Tile Data")]
    public class TileData : ScriptableObject {
        public TileBase[] tiles;
        public int destroyable; //0 is indistructible
        public TileBaseLootTable replaceTiles;
        public TileType tileType;
        public int placementWeight = 1;
        [Header("POI")]
        public GOLootTable poiTilemaps;
        public Vector2Int poiSize; //min 3x3, both values must be odd
        [Header("Prefabs")]
        public GOLootTable prefabs;
        public Vector2Int PoiOffset => new Vector2Int(Mathf.FloorToInt(poiSize.x/2), Mathf.FloorToInt(poiSize.y/2));
        public Tilemap GetPoi() {
            return poiTilemaps == null ? null : poiTilemaps.GetWeightedItem().GetComponentInChildren<Tilemap>();
        }

        public TileBase GetTile() => replaceTiles.GetWeightedItem();  
    }

    public enum TileType {Normal, Poi, Placeholder, SpawnPoint}
}
