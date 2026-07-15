namespace Utils {
    public interface IDetectionStrategy {
        bool Execute(Transform player, Transform detector, CountdownTimer timer, Vector3 facingDirection = default);
    }
}
