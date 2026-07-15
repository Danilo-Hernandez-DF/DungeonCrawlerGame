namespace Game {
    public interface IInteractable {
        void OnInteract();
        
        static bool InRange(Vector3 position) {
            return Vector3.Distance(PlayerDetector.GetPlayer().transform.position, position) < 1f;
        }
    }
}