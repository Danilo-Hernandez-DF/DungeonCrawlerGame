using UnityEngine;
using UnityEngine.Serialization;

namespace UtilsModule {
    public class PersistentSingleton<T> : MonoBehaviour where T : Component {
        [FormerlySerializedAs("AutoUnparentOnAwake")] public bool autoUnparentOnAwake = true;

        private static T instance;

        private static bool HasInstance => instance != null;
        public static T TryGetInstance() => HasInstance ? instance : null;

        public static T Instance {
            get {
                if(instance) return instance;
                instance = FindAnyObjectByType<T>();
                if(instance) return instance;
                var go = new GameObject(typeof(T).Name + " Auto-Generated");
                instance = go.AddComponent<T>();

                return instance;
            }
        }

        protected virtual void Awake() {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton() {
            if (!Application.isPlaying) return;

            if(autoUnparentOnAwake) {
                transform.SetParent(null);
            }

            if(instance == null) {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            } else {
                if(instance != this) {
                    Destroy(gameObject);
                }
            }
        }
    }
}
