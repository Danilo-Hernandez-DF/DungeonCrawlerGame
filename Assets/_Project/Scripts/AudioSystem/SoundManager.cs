using UnityEngine.Pool;
using UnityEngine.Audio;
using Game;
using Utils;

namespace AudioSystem
{
    public class SoundManager : PersistentSingleton<SoundManager>
    {
        IObjectPool<SoundEmitter> soundEmitterPool;
        readonly List<SoundEmitter> activeSoundEmitters = new();
        public readonly Queue<SoundEmitter> FrequentSoundEmitters = new();
        [SerializeField] SoundEmitter soundEmitterPrefab;
        [SerializeField] bool collectionCheck;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxPoolSize = 100;
        [SerializeField] int maxSoundInstances = 30;
        [Header("Audio")]
        [SerializeField] AudioMixerGroup masterMixer;

        void Start()
        {
            if(GameManager.Instance.globalSettings.masterVolume == 0) masterMixer.audioMixer.SetFloat("MasterVolume", -80);
            else masterMixer.audioMixer.SetFloat("MasterVolume", ToSoundRange(GameManager.Instance.globalSettings.masterVolume));

            if (GameManager.Instance.globalSettings.musicVolume == 0) masterMixer.audioMixer.SetFloat("MusicVolume", -80);
            else masterMixer.audioMixer.SetFloat("MusicVolume", ToSoundRange(GameManager.Instance.globalSettings.musicVolume));
            
            if (GameManager.Instance.globalSettings.sfxVolume == 0) masterMixer.audioMixer.SetFloat("SFXVolume", -80);
            else masterMixer.audioMixer.SetFloat("SFXVolume", ToSoundRange(GameManager.Instance.globalSettings.sfxVolume));
            
            if (GameManager.Instance.globalSettings.uiVolume == 0) masterMixer.audioMixer.SetFloat("UIVolume", -80);
            else masterMixer.audioMixer.SetFloat("UIVolume", ToSoundRange(GameManager.Instance.globalSettings.uiVolume));
            InitializePool();
        }

        public int ToSoundRange(int value) => Mapping.LinearMapping(value, 0, 100, -20, -5);
        public int FromSoundRange(int value) => Mapping.LinearMapping(value, -20, -5, 0, 100);

        public SoundBuilder CreateSound() => new SoundBuilder(this);

        public bool CanPlaySound(SoundData data)
        {
            if (!data.frequentSound) return true;

            if (FrequentSoundEmitters.Count >= maxSoundInstances && FrequentSoundEmitters.TryDequeue(out var soundEmitter))
            {
                try
                {
                    soundEmitter.Stop();
                    return true;
                }
                catch
                {
                    Debug.Log("Emitter already released");
                }
                return false;
            }
            return true;
        } 

        public SoundEmitter Get()
        {
            return soundEmitterPool.Get();
        }

        public void ReturnToPool(SoundEmitter soundEmitter)
        {
            soundEmitterPool.Release(soundEmitter);
        }

        SoundEmitter CreateSoundEmitter()
        {
            var soundEmitter = Instantiate(soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }

        void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            activeSoundEmitters.Add(soundEmitter);
        }

        void OnReturnedToPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(false);
            activeSoundEmitters.Remove(soundEmitter);
        }

        void OnDestroyPoolObject(SoundEmitter soundEmitter)
        {
            Destroy(soundEmitter.gameObject);
        }

        void InitializePool()
        {
            soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize);
        }
    }
}