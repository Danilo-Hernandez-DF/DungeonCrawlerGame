using System.Linq;
using UnityEngine;
using UtilsModule;

namespace Game {
    [CreateAssetMenu(fileName = "DungeonModifier", menuName = "Dungeon Modifiers/ItemOnKill")]
    public class ItemOnKillMod : DungeonModifier {
        [SerializeField] private Item item;
        public override void OnEnemyDeath(Enemy enemy) {
            PlayerDetector.GetPlayerComponent().GiveItem(new Item(item.data, tags: item.tags.Where(x => !x.inherent)
                .ToList()), item.count);
        }
    }
}