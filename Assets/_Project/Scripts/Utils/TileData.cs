using UnityEngine;
using UnityEngine.Tilemaps;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Tile Data", menuName = "Data/Tile Data")]
    public class TileData : ScriptableObject {
        [SerializeField] public TileBase[] tiles;
        [SerializeField] public int destroyable; //0 is indistructible
        [SerializeField] public TileBase[] replaceTiles;
        [SerializeField] public bool isPlaceholder;
        [SerializeField] public int placementWeight = 1;
    }
}
