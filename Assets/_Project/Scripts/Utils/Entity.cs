using System;
using System.Numerics;
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
        public Stats Stats {get; private set;}
        public Status Status {get; private set;}
        public Vector2 FacingDirection {get; protected set;}
        public GameObject Renderer;
        public GameObject shadow;
        public SpriteRenderer RendererComponent {get; private set;}
        [SerializeField] private float defaultZPos = 0;
        [SerializeField] public float animationSpeed = 1f;
        public float zPos {get; protected set;}
        public bool Grounded => zPos == 0;

        protected void Init() {
            if(entityData) Stats = new Stats(new StatsMediator(), baseStats);
            Status = new Status(this);
            Rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
            if(Renderer) RendererComponent = Renderer.GetComponent<SpriteRenderer>();
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
                shadow.transform.localScale = Vector2.one * MiscUtils.LinearMapping(zPos,
                    0, 10, 1, 0.1f);
            }
        }
    
        public void ApplyForce(Vector2 force) {
            if(Rb == null) return;
            Rb.AddForce(force);
        }

        protected virtual void OnDeath() {
            Status.OnDeath();
            //Debug.Log($"{name} has reached 0 Hp");
        }

        protected virtual void OnDamage(int damage, GameObject dmgSource = null, bool ignoreKnockback = false) {
            Status.OnDamage(damage);
            GameManager.Instance.TrackEntity(entityData, new() {damageTaken = damage});
            //Debug.Log($"{name} took {damage} damage");
        } 

        public void TakeDamage(int damage, bool ignoreDefense = false, GameObject dmgSource = null, bool ignoreKnockback = false) {
            if(!health) return;

            var isDead = !ignoreDefense ? health.TakeDamage(damage - Stats.Defense) : health.TakeDamage(damage);
            
            OnDamage(damage, dmgSource);
            if(isDead) OnDeath();
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