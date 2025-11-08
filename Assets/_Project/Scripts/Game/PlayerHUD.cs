using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtilsModule;

namespace Game
{
    public class PlayerHUD: Singleton<PlayerHUD>
    {
        [SerializeField] private Image healthBarFill;
        [SerializeField] private Image healthBarBg;
        [SerializeField] private List<EnemyMarker> enemyMarkers;
        [SerializeField] private Image objectiveDisplay;
        [SerializeField] private Image objectiveProgress;
        [SerializeField] private List<Image> modifierIcons;
        [SerializeField] private Transform modifierIconHolder;
        private PlayerController player;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject modIconGO;
        [SerializeField] private GameObject enemyMarkerGO;
        
        private new void Awake() {
            base.Awake();
            player = PlayerDetector.GetPlayerComponent();
        }

        public void Init() {
            modifierIcons = new List<Image>();
            enemyMarkers = new List<EnemyMarker>();

            for (int i = modifierIconHolder.childCount - 1; i >= 0; i--) {
                Destroy(modifierIconHolder.GetChild(i).gameObject);
            }
            
            foreach (DungeonModifier modifier in DungeonController.Instance.modifiers) {
                var icon = Instantiate(modIconGO, modifierIconHolder).GetComponent<Image>();
                icon.sprite = modifier.icon;
                modifierIcons.Add(icon);
            }

            if (DungeonController.Instance.completionQuest) {
                objectiveDisplay.sprite = DungeonController.Instance.completionQuest.icon;
                objectiveDisplay.gameObject.SetActive(true);
                objectiveProgress.gameObject.SetActive(true);
            }
            else {
                objectiveDisplay.gameObject.SetActive(false);
                objectiveProgress.gameObject.SetActive(false);
            }
        }
        
        private void Update() {
            healthBarFill.fillAmount = (float)player.health.currentHealth / player.health.maxHealth;
            objectiveProgress.fillAmount = 1 - DungeonController.Instance.objectivePercentage;
        }
        
        public void EnableEnemyMarker(Enemy enemy, Dir direction) {
            foreach (var marker in enemyMarkers) {
                if(marker.enemy == enemy) return;
            }

            foreach (var marker in enemyMarkers) {
                if(marker.enemy != null) continue;
                marker.SetEnemy(enemy);
                marker.Init(direction);
                return;
            }
            
            var newMarker = Instantiate(enemyMarkerGO, transform).GetComponent<EnemyMarker>();
            newMarker.SetEnemy(enemy);
            newMarker.Init(direction);
            enemyMarkers.Add(newMarker);
        }
        
        public void DisableEnemyMarker(Enemy enemy) {
            foreach (var marker in enemyMarkers) {
                if(marker.enemy == enemy) {
                    marker.SetEnemy(null);
                    return;
                }
            }
        }
    }
}