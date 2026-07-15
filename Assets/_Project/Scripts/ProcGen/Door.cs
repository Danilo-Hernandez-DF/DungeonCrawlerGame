using Game;

namespace ProcGen {
    public class Door : MonoBehaviour, IInteractable {
        Collider2D col;
        SpriteRenderer sprite;
        GameObject map;
        [SerializeField] bool isLocked = false;
        [SerializeField] ItemData key;
        public Color color;
        public RoomController secondaryRoom;
        public RoomController connectedRoom;
        public RoomEntrance entrance;
        void Awake() {
            col = GetComponent<Collider2D>();
            sprite = GetComponent<SpriteRenderer>();
            map = transform.GetChild(0).gameObject;

            if(!isLocked) Open();
            else Close();
        }
        public void Open() { 
            col.enabled = false;
            sprite.enabled = false;
            map.SetActive(false);
        }
        public void Close() { 
            col.enabled = true;
            sprite.enabled = true;
            map.SetActive(true);
        }
        
        bool InRange() => IInteractable.InRange(transform.position);
        
        public void OnInteract() {
            if(GameManager.Instance.Paused) return;
            if(!isLocked) return;
            if (!InRange()) return;
            if (!PlayerDetector.GetPlayerComponent().playerInv.TryRemove(key.GetItem(), 1, fullMatch: false)) {
                //Sound/anim feedback
                return;
            }
            isLocked = false;
            Open();
        }
        
        void OnEnable() {
            GameManager.Instance.input.Interact += OnInteract;
        }

        void OnDisable() {
            GameManager.Instance.input.Interact -= OnInteract;
        }
    }
}
