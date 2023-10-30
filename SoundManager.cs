using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class SoundManager : MonoBehaviour
{
    public static ObjectPool<GameObject> soundObjectPool;

    public GameObject soundGameObject;
    public AudioMixer mixer;
    public AudioClip ambienceSound;
    public float soundCooldown;


    public int soundsPlaying;
    public int maxSoundsPlaying;
    private GameObject poolChild;

    void Start()
    {
        soundObjectPool = new ObjectPool<GameObject>(() => Instantiate(soundGameObject), (obj) => obj.SetActive(true),
                                                        (obj) => obj.SetActive(false), (obj) => Destroy(obj), false);

        poolChild = new GameObject("Sound Pool");
        PlaySound(ambienceSound, true);
    }

    public void PlaySound(AudioClip sound)
    {
        if (soundsPlaying > maxSoundsPlaying)
            return;

        GameObject soundObject = soundObjectPool.Get();
        soundObject.transform.parent = poolChild.transform;
        AudioSource audioSource = soundObject.GetComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups(sound.name)[0];
        audioSource.PlayOneShot(sound);
    }

    public void PlaySound(AudioClip sound, Vector2 pitchRange, Vector2 volumeRange)
    {
        if (soundsPlaying > maxSoundsPlaying)
            return;

        GameObject soundObject = soundObjectPool.Get();
        soundObject.transform.parent = poolChild.transform;
        AudioSource audioSource = soundObject.GetComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups(sound.name)[0];
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.volume = Random.Range(volumeRange.x, volumeRange.y);
        audioSource.PlayOneShot(sound);
    }

    public void PlaySound(AudioClip sound, bool isLoop)
    {
        if (soundsPlaying > maxSoundsPlaying)
            return;

        GameObject soundObject = soundObjectPool.Get();
        soundObject.transform.parent = poolChild.transform;
        AudioSource audioSource = soundObject.GetComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups(sound.name)[0];
        audioSource.loop = isLoop;
        audioSource.clip = sound;
        audioSource.Play();
    }
}
