using System.Linq;
using Game;
using Unity.VisualScripting;
using UnityEngine;
using UtilsModule;

namespace ProcGen {
    public class Door : MonoBehaviour, IInteractable {
        private static readonly int CloseStr = Animator.StringToHash("Close");
        private static readonly int OpenStr = Animator.StringToHash("Open");
        
        Collider2D col;
        Animator anim;
        SpriteRenderer sprite;
        [SerializeField] bool isLocked = false;
        [SerializeField] Item key;
        [SerializeField] PlayerDetector playerDetector;
        void Awake() {
            col = GetComponent<Collider2D>();
            anim = GetComponent<Animator>();
            sprite = GetComponent<SpriteRenderer>();

            if(!isLocked) Open();
            else Close();
        }
        public void Open() { 
            col.enabled = false;
            anim.SetTrigger(OpenStr);
            sprite.enabled = false;
        }
        public void Close() { 
            col.enabled = true;
            anim.SetTrigger(CloseStr);
            sprite.enabled = true;
        }
        
        private bool InRange() {
            var colliders = Physics2D.OverlapCircleAll(transform.position, 1.5f);
            return colliders.Any(collider => collider.CompareTag("Player"));
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
