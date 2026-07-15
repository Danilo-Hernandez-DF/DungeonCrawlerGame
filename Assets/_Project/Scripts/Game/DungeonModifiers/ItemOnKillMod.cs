using Localisation;
using ProcGen;
using Utils;

namespace Game {
    [CreateAssetMenu(fileName = "DungeonModifier", menuName = "Dungeon Modifiers/ItemOnKill")]
    public class ItemOnKillMod : DungeonModifier {
        [SerializeField] private Item item;
        [SerializeField] private float chance;
        [SerializeField] private int count;
        [SerializeField] private EntityData eligibleEnemy;
        [SerializeField] private TagData eligibleTag;
        private Counter counter;
        [SerializeField] private int maxItems = 1;
        
        [SerializeField] private StatusEffectData eligibleStatusSource;
        [SerializeField] private EntityData eligibleEntitySource;
        
        [SerializeField] private Color specialTextColor;
        private readonly string noEnemyKey = "no_enemy";
        private readonly string anything = "anything";
        
        public override void Reset() {
            if (counter == null) counter = new Counter(maxItems);
            else counter.Reset();
        }

        public override string Description() {
            var descriptionKey = "description_item_on_kill";
            var description = LocalisationSystem.GetLocalisedValue(descriptionKey);
            
            var enemyStr = LocalisationSystem.GetLocalisedValue(noEnemyKey);
            if(eligibleEnemy) {
                enemyStr = eligibleEnemy.nameKey;
            }
            else if(eligibleTag) {
                enemyStr = enemyStr.Replace("{}", eligibleTag.name);
                enemyStr = enemyStr.TrimEnd(' ');
            }
            else {
                enemyStr = enemyStr.Replace("{}", "");
                enemyStr = enemyStr.TrimEnd(' ');
            }
            //{} enemy
            //un enemigo {}

            string hexColor = ColorUtility.ToHtmlStringRGBA(specialTextColor);
            var text0 = $"<color=#{hexColor}>{enemyStr}</color>";
            
            string sourceStr = "";
            if(eligibleEntitySource) {
                sourceStr = eligibleEntitySource.nameKey;
            }
            else if(eligibleStatusSource) {
                sourceStr = eligibleStatusSource.nameKey;
            }
            else {
                sourceStr = LocalisationSystem.GetLocalisedValue(anything);
            }
            var text1 = $"<color=#{hexColor}>{sourceStr}</color>";
            
            var text2 = $"<color=#{hexColor}>{item.data.name}</color>";
            
            description = description.Replace("{0}", text0);
            description = description.Replace("{1}", text1);
            description = description.Replace("{2}", text2);
            description = description.Replace("{3}", $"<color=#{hexColor}>{count}</color>");
            
            if (chance < 100) {
                description = description.Replace("~", "");
            }
            else {
                description = description.Split("~")[0];
            }
            
            //Whenever {0} is killed by {1}, the player obtains {2} x{3}~, {4}% of the time.
            //Cuando {0} es derrotado por {1}, el jugador obtiene {2} x{3}~, {4}% de las veces.
            
            return description;
        }
        
        public override List<ItemData> GetItems() {
            return new List<ItemData> { item.data };
        }

        public override void OnRoomEntered(RoomController room) {
            counter.Reset();
        }

        public override void OnEnemyDeath(Enemy enemy, DamageSource source) {
            if (counter.ReachedMax()) return;
            
            var matchesSource = eligibleEntitySource == source.entity?.entityData || eligibleEntitySource == null;
            var matchesStatusSource = eligibleStatusSource == source.statusEffect || eligibleStatusSource == null;
            
            if (!matchesSource && !matchesStatusSource) return;

            var matchesEnemy = eligibleEnemy == enemy.entityData || eligibleEnemy == null;
            var matchesTag = enemy.entityData.tags.Exists(x => x.data == eligibleTag) || eligibleTag == null;

            if (!matchesEnemy && !matchesTag) return;
            
            if (Random.Range(0f, 100f) > chance) return;
            
            PlayerDetector.GetPlayerComponent().GiveItem(new Item(item.data, addTags: item.tags), count);
            counter.Count();
            Debug.Log("Item given");
        }
    }
}