using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game {
    public class PlayerDetector : MonoBehaviour {
        [SerializeField] float detectionAngle = 60f;
        public float detectionRadius = 10f;
        [SerializeField] float innerDetectionradius = 5f;
        [SerializeField] float detectionCooldown = 1f;
        [SerializeField] float rememberTime = 1f;
        [SerializeField] bool showDebug = false;
        float attackRange;

        Vector3 facingDirection;

        public Transform Player;
        public PlayerController PlayerComponent => Player.GetComponent<PlayerController>();
        public bool RemembersPlayerPosition => rememberTimer.IsRunning;
        public Health PlayerHealth {get; private set;}
        CountdownTimer detectionTimer;
        CountdownTimer rememberTimer;

        IDetectionStrategy detectionStrategy;

        public void Init(float attackRange) {
            this.attackRange = attackRange;
        }

        void Awake() {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
            PlayerHealth = Player.GetComponent<Health>();
        }

        void Start() {
            detectionTimer = new CountdownTimer(detectionCooldown);
            detectionTimer.OnTimerStop += () => {
                rememberTimer.Reset(rememberTime);
                rememberTimer.Start();
            };
            rememberTimer = new CountdownTimer(rememberTime);
            detectionStrategy = new ConeDetectionStrategy(detectionAngle, detectionRadius, innerDetectionradius);
        }

        void Update() => detectionTimer.Tick(Time.deltaTime);

        public bool CanDetectPlayer(Vector3 facingDirection = default) {
            this.facingDirection = facingDirection;
            return detectionTimer.IsRunning || detectionStrategy.Execute(Player, transform, detectionTimer, facingDirection);
        }

        public bool HasLineOfSight(Vector2 position) {
            RaycastHit2D hit = Physics2D.Raycast(position, DirectionToPlayerNormalized, attackRange, LayerMask.GetMask("Player", "Wall"));
            Debug.Log(hit.collider?.name);
            return hit.collider && hit.transform.CompareTag("Player");
        }

        public static GameObject GetPlayer() {
            return GameObject.FindGameObjectWithTag("Player");
        }
        
        public static PlayerController GetPlayerComponent() => GetPlayer().GetComponent<PlayerController>();

        public bool CanAttackPlayer() {
            return DirectionToPlayer.magnitude <= attackRange;
        }
        
        public Vector2 DirectionToPlayer => Player.position - transform.position;
        public Vector2 DirectionToPlayerNormalized => DirectionToPlayer.normalized;
        public float DistanceToPlayer => Vector3.Distance(transform.position, Player.position);

        public void SetDetectionStrategy(IDetectionStrategy detectionStrategy) => this.detectionStrategy = detectionStrategy;

        void OnDrawGizmosSelected() {
            if (!showDebug) return;
            
            // Calculate our cone directions
            Vector3 forwardConeDirection = Quaternion.Euler(0, 0, detectionAngle / 2) * facingDirection * detectionRadius;
            Vector3 backwardConeDirection = Quaternion.Euler(0, 0, -detectionAngle / 2) * facingDirection * detectionRadius;

            Gizmos.color = Color.yellow;

            // Draw lines to represent the cone
            Gizmos.DrawLine(transform.position, transform.position + forwardConeDirection);
            Gizmos.DrawLine(transform.position, transform.position + backwardConeDirection);
            
            Gizmos.color = Color.blue;
            
            // Draw lines to represent the line of sight
            Gizmos.DrawLine(transform.position, transform.position + (detectionRadius * facingDirection));
            
            Gizmos.color = Color.green;

            // Draw a spheres for the radii
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            Gizmos.DrawWireSphere(transform.position, innerDetectionradius);

            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(transform.position, attackRange);
            
            // Draw a line to the player
            Gizmos.color = Color.magenta;
            if (Player) {
                Gizmos.DrawLine(transform.position, transform.position + (Vector3)(DirectionToPlayerNormalized * attackRange));
            }
        }
    }
}
