namespace Utils
{
    public class DisableAfter : MonoBehaviour
    {
        CountdownTimer timer;
        public float time;
        public GameObject toDisable;
        void Start()
        {
            timer = new CountdownTimer(time);
            timer.OnTimerStop += () => {
                toDisable.SetActive(false);
                
            };
            timer.Start();
        }
        
        private void Update()
        {
            timer.Tick(Time.deltaTime);
        }
    }
}