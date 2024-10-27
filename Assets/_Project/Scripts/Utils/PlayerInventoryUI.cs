using Game;
using UnityEngine;

namespace UtilsModule {
    public class PlayerInventoryUI : InventoryUI { 
        [SerializeField] private InventoryUI equipmentUI;

        protected override void Start() {
            base.Start();
            equipmentUI.targetEntity = PlayerDetector.GetPlayer().GetComponent<PlayerController>();
        }

        protected new void OnEnable() {
            GameManager.Instance.input.OpenMenu += Open;
            base.OnEnable();
        }

        protected new void OnDisable() {
            GameManager.Instance.input.OpenMenu -= Open;
            base.OnDisable();
        }
    }
}