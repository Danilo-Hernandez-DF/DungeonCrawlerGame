using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Tile Data", menuName = "Data/Tile Data")]
    public class TileData : ScriptableObject {
        [SerializeField] public TileBase[] tiles;
        [SerializeField] public int destroyable; //0 is indistructible
        [SerializeField] public TileBaseLootTable replaceTiles;
        [SerializeField] public TileType tileType;
        [SerializeField] public int placementWeight = 1;
        [Header("POI")]
        [SerializeField] public GOLootTable poiTilemaps;
        [SerializeField] public Vector2Int poiSize; //min 3x3, both values must be odd
        [Header("Prefabs")]
        [SerializeField] public GOLootTable prefabs;
        public Vector2Int PoiOffset => new Vector2Int(Mathf.FloorToInt(poiSize.x/2), Mathf.FloorToInt(poiSize.y/2));
        public Tilemap GetPoi(bool seeded = false) {
            if(poiTilemaps == null) return null;
            return poiTilemaps.GetWeightedItem(seeded: seeded).GetComponentInChildren<Tilemap>();
        }

        public TileBase GetTile(bool seeded = false) => replaceTiles.GetWeightedItem(seeded: seeded);  
    }

    public enum TileType {Normal, Poi, Placeholder, SpawnPoint}
}
