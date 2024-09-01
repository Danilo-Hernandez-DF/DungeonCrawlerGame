using UnityEngine;
using UtilsModule;

namespace Game {
    public interface IDetectionStrategy {
        bool Execute(Transform player, Transform detector, CountdownTimer timer, Vector3 facingDirection = default);
    }
}
