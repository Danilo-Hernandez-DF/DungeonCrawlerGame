using UnityEngine.Events;

namespace Utils {
    public abstract class EventListener<T> : MonoBehaviour {
        [SerializeField] EventChannel<T> eventChannel;
        [SerializeField] UnityEvent<T> unityEvent;

        protected void Awake() {
            eventChannel.Register(this);
        }

        protected void OnDestroy() {
            eventChannel.Deregister(this);
        }

        public void Raise(T value) => unityEvent?.Invoke(value);
    }

    public abstract class EventListener<T, T2> : MonoBehaviour {
        [SerializeField] EventChannel<T, T2> eventChannel;
        [SerializeField] UnityEvent<T, T2> unityEvent;

        protected void Awake() {
            eventChannel.Register(this);
        }

        protected void OnDestroy() {
            eventChannel.Deregister(this);
        }

        public void Raise(T value, T2 value2) => unityEvent?.Invoke(value, value2);
    }

    public class EventListener : EventListener<Empty> { }
}