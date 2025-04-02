using Game;
using UnityEngine;
using UtilsModule;

namespace ProcGen {
    public class Door : MonoBehaviour, IInteractable {
        Collider2D col;
        SpriteRenderer sprite;
        [SerializeField] bool isLocked = false;
        [SerializeField] Item key;
        [SerializeField] PlayerDetector playerDetector;
        void Awake() {
            col = GetComponent<Collider2D>();
            sprite = GetComponent<SpriteRenderer>();

            if(!isLocked) Open();
            else Close();
        }
        public void Open() { 
            col.enabled = false;
            sprite.enabled = false;
        }
        public void Close() { 
            col.enabled = true;
            sprite.enabled = true;
        }
        
        private bool InRange() {
            var colliders = Physics2D.OverlapCircleAll(transform.position, 1f);

            foreach(Collider2D col in colliders) {
                if(col.CompareTag("Player")) return true;
            }

            return false;
        }
        
        public void OnInteract() {
            Debug.Log("Interacting with door");
            if(GameManager.Instance.Paused) return;
            if(!InRange()) return;
            Debug.Log("Is in range");
            if(!isLocked) return;
            if (!playerDetector.PlayerComponent.playerInv.TryRemove(key, fullMatch: false)) {
                Debug.Log("You need a key to open this door");
                return;
            }
            Debug.Log("Door unlocked");
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
