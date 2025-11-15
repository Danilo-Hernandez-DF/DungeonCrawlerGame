using System;
using _Project.Scripts.Utils;
using Game;
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
        public Stats Stats {get; private set;}
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
        protected bool init;
        protected bool hasDied = false;

        public virtual void Init()
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
            init = true;
        }

        public void Update() {
            if (!init) return;
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

        protected virtual void OnDamage(int damage, DamageSource dmgSource, bool ignoreKnockback = false) {
            Vector2 hitDirection = Vector2.zero;
            if (dmgSource.source)
            {
                hitDirection = (transform.position - dmgSource.source.transform.position).normalized;
            }

            var eulerAngles = dmgSource.statusEffect ? Quaternion.Euler(90, 0, 0) :
                Quaternion.Euler(0, 0, Mathf.Atan2(hitDirection.y, hitDirection.x) * Mathf.Rad2Deg - 90);
            var hitEffect = Instantiate(GameManager.Instance.hitEffectPrefab, transform.position, eulerAngles);
            hitEffect.transform.SetParent(transform);
            var particleSystem = hitEffect.GetComponentInChildren<ParticleSystemRenderer>();
            particleSystem.material = GameManager.Instance.entityHitmaterial;
            

            if (RendererComponent)
            {
                RendererComponent.sharedMaterial = GameManager.Instance.entityHitmaterial;
                RendererComponent.sharedMaterial.SetTexture("_Texture2D", RendererComponent.sprite.texture);
                particleSystem.material.SetTexture("_Texture2D", GameManager.Instance.partcleHitTexture.texture);
                
                RendererComponent.sharedMaterial.SetColor("_Color", dmgSource.color);
                particleSystem.material.SetColor("_Color", dmgSource.color);
                
                Invoke(nameof(ResetMaterial), 0.1f);
            }

            Status.OnDamage(damage);
            GameManager.Instance.TrackEntity(entityData, new() {damageTaken = damage});
        } 
        
        protected void ResetMaterial() {
            RendererComponent.sharedMaterial = defaultMaterial;
        }
        
        public void Die() {
            OnDeath();
        }

        public void TakeDamage(int damage, DamageSource dmgSource, bool ignoreDefense = false, bool ignoreKnockback = false)
        {
            if (!health) return;

            var died = !ignoreDefense ? health.TakeDamage(damage - Stats.Defense) : health.TakeDamage(damage);

            OnDamage(damage, dmgSource);
            if (died) hasDied = true;
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
    
    public struct DamageSource {
        public readonly GameObject source;
        public Entity entity => source?.GetComponent<Entity>();
        public StatusEffectData statusEffect;
        public Color color => statusEffect?.dmgColor ?? _color;
        private Color _color;
        
        public DamageSource(GameObject source) {
            this.source = source;
            this.statusEffect = null;
            _color = Color.white;
        }
        
        public void SetColor(Color _color) {
            this._color = color;
        }
    }
}