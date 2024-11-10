using UnityEngine;

namespace UtilsModule {
    public class DestroyAfter : MonoBehaviour {
        [SerializeField] float time;
        CountdownTimer timer;

        public void Awake() {
            timer = new CountdownTimer(time);
            timer.OnTimerStop += () => Destroy(gameObject);
            timer.Start();
        }

        public void Update() => timer.Tick(Time.deltaTime);
    }
}