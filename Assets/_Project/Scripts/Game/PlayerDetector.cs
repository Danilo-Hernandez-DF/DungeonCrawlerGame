using UnityEngine;
using UnityEngine.AI;
using UtilsModule;

namespace Game {
    public class PlayerDetector : MonoBehaviour {
        [SerializeField] float detectionAngle = 60f;
        [SerializeField] float detectionRadius = 10f;
        [SerializeField] float innerDetectionradius = 5f;
        [SerializeField] float detectionCooldown = 1f;
        [SerializeField] float attackRange = 2f;

        Vector3 facingDirection;

        public Transform Player {get; private set;}
        public Health PlayerHealth {get; private set;}
        CountdownTimer detectionTimer;

        IDetectionStrategy detectionStrategy;

        void Awake() {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
            PlayerHealth = Player.GetComponent<Health>();
        }

        void Start() {
            detectionTimer = new CountdownTimer(detectionCooldown);
            detectionStrategy = new ConeDetectionStrategy(detectionAngle, detectionRadius, innerDetectionradius);
        }

        void Update() => detectionTimer.Tick(Time.deltaTime);

        public bool CanDetectPlayer(Vector3 facingDirection = default) {
            this.facingDirection = facingDirection;
            return detectionTimer.IsRunning || detectionStrategy.Execute(Player, transform, detectionTimer, facingDirection);
        }

        public bool CanAttackPlayer() {
            var directionToPlayer = Player.position - transform.position;
            return directionToPlayer.magnitude <= attackRange;
        }

        public void SetDetectionStrategy(IDetectionStrategy detectionStrategy) => this.detectionStrategy = detectionStrategy;

        void OnDrawGizmos() {
            Gizmos.color = Color.green;

            // Draw a spheres for the radii
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            Gizmos.DrawWireSphere(transform.position, innerDetectionradius);

            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(transform.position, attackRange);
            

            // Calculate our cone directions
            Vector3 forwardConeDirection = Quaternion.Euler(0, 0, detectionAngle / 2) * facingDirection * detectionRadius;
            Vector3 backwardConeDirection = Quaternion.Euler(0, 0, -detectionAngle / 2) * facingDirection * detectionRadius;

            Gizmos.color = Color.yellow;

            // Draw lines to represent the cone
            Gizmos.DrawLine(transform.position, transform.position + forwardConeDirection);
            Gizmos.DrawLine(transform.position, transform.position + backwardConeDirection);
        }
    }
}
