namespace Utils {
    public abstract class EventChannel<T> : ScriptableObject {
        readonly HashSet<EventListener<T>>  observers = new();

        public void Invoke(T value) {
            foreach(var observer in observers) {
                observer.Raise(value);
            }
        }

        public void Register(EventListener<T> observer) => observers.Add(observer);

        public void Deregister(EventListener<T> observer) => observers.Remove(observer);
    }

    public abstract class EventChannel<T, T2> : ScriptableObject {
        readonly HashSet<EventListener<T, T2>>  observers = new();

        public void Invoke(T value, T2 value2) {
            foreach(var observer in observers) {
                observer.Raise(value, value2);
            }
        }

        public void Register(EventListener<T, T2> observer) => observers.Add(observer);

        public void Deregister(EventListener<T, T2> observer) => observers.Remove(observer);
    }
    public readonly struct Empty {}
    
    [CreateAssetMenu(menuName = "Events/EventChannel")]
    public class EventChannel : EventChannel<Empty> { }
}