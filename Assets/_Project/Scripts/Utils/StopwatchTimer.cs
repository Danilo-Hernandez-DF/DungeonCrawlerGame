namespace UtilsModule {
    public class StopwatchTimer: Timer {
        public StopwatchTimer() : base(0) { }
        public override void Tick(float deltaTime) {
            if(IsRunning) {
                Time += deltaTime;
            }
        }

        public void Reset() => Time = initialTime;
        public float GetTime() => Time;
    }
}