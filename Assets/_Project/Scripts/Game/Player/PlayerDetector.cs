namespace Game {
    public class PlayerDetector : MonoBehaviour {
        public static GameObject GetPlayer() {
            return GameObject.FindGameObjectWithTag("Player");
        }
        
        public static PlayerController GetPlayerComponent() => GetPlayer().GetComponent<PlayerController>();
    }
}
