using System;

namespace UtilsModule {
    public abstract class Timer {
        protected float InitialTime;
        protected float Time {get; set;}
        public bool IsRunning {get; protected set;}

        public float Progress => Time/InitialTime;

        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };

        protected Timer(float value) {
            InitialTime = value;
            IsRunning = false;
        }

        public void Start() {
            Time = InitialTime;

            if(IsRunning) return;
            IsRunning = true;
            OnTimerStart.Invoke();
        }

        public void Stop() {
            if(!IsRunning) return;
            IsRunning = false;
            OnTimerStop.Invoke();
        }

        public void Pause() => IsRunning = false;

        public void Resume() => IsRunning = true;

        public abstract void Tick(float deltaTime);
    }
}