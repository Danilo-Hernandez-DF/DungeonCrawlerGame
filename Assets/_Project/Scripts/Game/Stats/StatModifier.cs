using Utils;

namespace Game {
    public abstract class StatModifier : IDisposable {
        public string ID;
        public bool MarkedForRemoval { get; set; }
        public event Action<StatModifier> OnDispose = delegate {};
        readonly CountdownTimer timer;
        protected StatModifier(float duration) {
            if(duration <= 0) return;

            timer = new CountdownTimer(duration);
            timer.OnTimerStop += () => MarkedForRemoval = true;
            timer.Start();
        }

        public void Update(float deltaTime) => timer?.Tick(deltaTime);
        public abstract void Handle(object sender, Query query);

        public void Dispose() {
            OnDispose.Invoke(this);
        }
    }
}