using UnityEngine;
using UtilsModule;

namespace Game {
    public class ConeDetectionStrategy : IDetectionStrategy {
        readonly float detectionAngle;
        readonly float detectionRadius;
        readonly float innerDetectionradius;

        public ConeDetectionStrategy(float detectionAngle, float detectionRadius, float innerDetectionradius) {
            this.detectionAngle = detectionAngle;
            this.detectionRadius = detectionRadius;
            this.innerDetectionradius = innerDetectionradius;
        }

        public bool Execute(Transform player, Transform detector, CountdownTimer timer, Vector3 facingDirection = default) {
            if(timer.IsRunning) return false;

            var directionToPlayer = player.position - detector.position;
            var angleToPlayer = Vector3.Angle(directionToPlayer, facingDirection.normalized);

            if((!(angleToPlayer < detectionAngle / 2f) || !(directionToPlayer.magnitude < detectionRadius))
                && !(directionToPlayer.magnitude < innerDetectionradius))
                return false;

            timer.Start();
            return true;
        }
    }
}
