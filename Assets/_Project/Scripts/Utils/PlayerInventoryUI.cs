using Game;

namespace UtilsModule {
    public class PlayerInventoryUI : InventoryUI { 
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