namespace Utils {
    public class DestroyAfter : MonoBehaviour {
        public float time;
        CountdownTimer timer;

        public void Awake() {
            timer = new CountdownTimer(time);
            timer.OnTimerStop += () => Destroy(gameObject);
            timer.Start();
        }

        public void Update() => timer.Tick(Time.deltaTime);
    }
}