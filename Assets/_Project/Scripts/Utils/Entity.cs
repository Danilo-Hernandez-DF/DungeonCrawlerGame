using System;
using _Project.Scripts.Utils;
using Game;
using Unity.VisualScripting;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace UtilsModule {
    public abstract class Entity : MonoBehaviour, IVisitable {
        public EntityData entityData;
        public bool isTileEntity;
        public BaseStats baseStats => entityData.stats;
        public Health health;
        protected Rigidbody2D Rb {get; private set;}
        protected Collider2D Collider {get; private set;}
        public Stats Stats { get; private set; }
        public Status Status {get; private set;}
        public Vector2 FacingDirection {get; protected set;}
        public GameObject Renderer;
        public GameObject shadow;
        protected Animator anim;
        public SpriteRenderer RendererComponent { get; private set; }
        [SerializeField] private float defaultZPos = 0;
        public float zPos {get; protected set;}
        public bool Grounded => zPos == 0;
        Material defaultMaterial;

        protected void Init()
        {
            if (entityData) Stats = new Stats(new StatsMediator(), baseStats);
            Status = new Status(this);
            Rb = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
            health = GetComponent<Health>();
            if (Renderer)
            {
                RendererComponent = Renderer.GetComponent<SpriteRenderer>();
                defaultMaterial = RendererComponent.sharedMaterial;
            }
            anim = GetComponent<Animator>();
            health?.Init(Stats.Health);
            FacingDirection = Vector2.right;
        }

        protected virtual void Awake() {
            Init();
        }

        public void Update() {
            if(isTileEntity) return;
            Stats.Mediator.Update(Time.deltaTime);
            Status.Update(Time.deltaTime);
        }

        void LateUpdate() {
            if(!RendererComponent) return;
            RendererComponent.sortingOrder = -Mathf.CeilToInt(transform.position.y * 100);
            
            if (isTileEntity) return;
            if(Renderer) {
                Renderer.transform.localPosition = new Vector2(Renderer.transform.localPosition.x, zPos);
            }

            if(shadow) {
                shadow.transform.localScale = Vector2.one * MyUtils.LinearMapping(zPos,
                    0, 10, 1, 0.1f);
            }
        }
    
        public void ApplyForce(Vector2 force) {
            if(Rb == null) return;
            Rb.AddForce(force, ForceMode2D.Impulse);
        }

        protected virtual void OnDeath() {
            Status.OnDeath();
        }

        protected virtual void OnDamage(int damage, GameObject dmgSource = null, bool ignoreKnockback = false) {
            Vector2 hitDirection = Vector2.zero;
            if (dmgSource)
            {
                hitDirection = (transform.position - dmgSource.transform.position).normalized;
            }

            var hitEffect = Instantiate(GameManager.Instance.hitEffectPrefab, transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(hitDirection.y, hitDirection.x) * Mathf.Rad2Deg - 90));
            hitEffect.transform.SetParent(transform);

            if (RendererComponent)
            {
                RendererComponent.sharedMaterial = GameManager.Instance.entityHitmaterial;
                RendererComponent.sharedMaterial.SetVector("_HitDirection", hitDirection);
                RendererComponent.sharedMaterial.SetTexture("_Texture2D", RendererComponent.sprite.texture);
                RendererComponent.sharedMaterial.SetColor("_Color", Color.white);

                Invoke(nameof(ResetMaterial), 0.1f);
            }

            Status.OnDamage(damage);
            GameManager.Instance.TrackEntity(entityData, new() {damageTaken = damage});
        } 
        
        protected void ResetMaterial() {
            RendererComponent.sharedMaterial = defaultMaterial;
        }

        public void TakeDamage(int damage, bool ignoreDefense = false, GameObject dmgSource = null, bool ignoreKnockback = false)
        {
            if (!health) return;

            var isDead = !ignoreDefense ? health.TakeDamage(damage - Stats.Defense) : health.TakeDamage(damage);

            OnDamage(damage, dmgSource);
            if (isDead) OnDeath();
        }

        public void Heal(int amount) {
            if(!health) return;

            health.Heal(amount);
            Status.OnHeal(amount);
        }

        public void Accept(IVisitor visitor) {
            if(!isTileEntity) visitor.Visit(this);
        }
    }
}