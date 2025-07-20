using UnityEngine;

namespace MatchingGame
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        [Header("Sources")]
        [SerializeField] AudioSource musicSource;
        [SerializeField] AudioSource sfxSource;

        [Header("Clips")]
        [SerializeField] AudioClip startMusic;
        [SerializeField] AudioClip flipClip;
        [SerializeField] AudioClip matchClip;
        [SerializeField] AudioClip mismatchClip;

        [Header("Volumes (0..1)")]
        [Range(0, 1f)] public float masterVolume = 1f;
        [Range(0, 1f)] public float musicVolume = 1f;
        [Range(0, 1f)] public float sfxVolume = 1f;

        [Header("SFX Pitch")]
        [SerializeField] bool randomizePitch = true;
        [SerializeField] float pitchVar = 0.1f; // ± variation

        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AudioManager>();

                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject("AudioManager");
                        instance = singletonObject.AddComponent<AudioManager>();
                    }
                }
                return instance;
            }
        }
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
            InitSources();
            ApplyVolumes();

            if (startMusic) PlayMusic(startMusic);
        }

        void InitSources()
        {
            if (!musicSource)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            if (!sfxSource)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
        }

        public void PlayMusic(AudioClip clip)
        {
            if (!clip) return;
            musicSource.clip = clip;
            musicSource.volume = masterVolume * musicVolume;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void ChangeMusic(AudioClip newClip) => PlayMusic(newClip);

        public void PlaySfx(AudioClip clip)
        {
            if (!clip) return;
            sfxSource.pitch = randomizePitch ? Random.Range(1f - pitchVar, 1f + pitchVar) : 1f;
            sfxSource.PlayOneShot(clip, masterVolume * sfxVolume);
        }

        public void PlaySfx(AudioClip clip, float pitch)
        {
            if (!clip) return;
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(clip, masterVolume * sfxVolume);
        }

        public void PlayFlip() => PlaySfx(flipClip);
        public void PlayMatch() => PlaySfx(matchClip);
        public void PlayMismatch() => PlaySfx(mismatchClip);

        public void PlayFlip(float pitch) => PlaySfx(flipClip , pitch);
        public void PlayMatch(float pitch) => PlaySfx(matchClip, pitch);
        public void PlayMismatch(float pitch) => PlaySfx(mismatchClip, pitch);

        public void SetVolumes(float master, float music, float sfx)
        {
            masterVolume = Mathf.Clamp01(master);
            musicVolume = Mathf.Clamp01(music);
            sfxVolume = Mathf.Clamp01(sfx);
            ApplyVolumes();
        }

        void ApplyVolumes()
        {
            if (musicSource.isPlaying)
                musicSource.volume = masterVolume * musicVolume;
        }

        public void MuteAll(bool mute)
        {
            musicSource.mute = mute;
            sfxSource.mute = mute;
        }
    }
}
