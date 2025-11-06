using Game;
using UnityEngine;
using UnityEngine.Tilemaps;
using UtilsModule;

namespace ProcGen {
    public class Door : MonoBehaviour, IInteractable {
        Collider2D col;
        SpriteRenderer sprite;
        TilemapRenderer tilemapRenderer;
        [SerializeField] bool isLocked = false;
        [SerializeField] ItemData key;
        public Color color;
        void Awake() {
            col = GetComponent<Collider2D>();
            sprite = GetComponent<SpriteRenderer>();
            tilemapRenderer = transform.GetChild(0).GetComponentInChildren<TilemapRenderer>();

            if(!isLocked) Open();
            else Close();
        }
        public void Open() { 
            col.enabled = false;
            sprite.enabled = false;
            tilemapRenderer.enabled = false;
        }
        public void Close() { 
            col.enabled = true;
            sprite.enabled = true;
            tilemapRenderer.enabled = true;
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
