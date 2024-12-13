using UnityEngine;

namespace ProcGen {
    public class Door : MonoBehaviour { 
        Collider2D col;
        Animator anim;
        SpriteRenderer sprite;
        void Awake() {
            col = GetComponent<Collider2D>();
            anim = GetComponent<Animator>();
            sprite = GetComponent<SpriteRenderer>();

            Open();
        }
        public void Open() { 
            col.enabled = false;
            anim.SetTrigger("Open");
            sprite.enabled = false;
        }
        public void Close() { 
            col.enabled = true;
            anim.SetTrigger("Close");
            sprite.enabled = true;
        }
    }
}
