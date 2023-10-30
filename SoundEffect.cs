using UnityEngine;
using UnityEngine.Pool;

public class SoundEffect : MonoBehaviour
{
    AudioSource audioSource;
    private ObjectPool<GameObject> soundPool;
    private SoundManager soundManager;

    void Start()
    {
        
        soundPool = SoundManager.soundObjectPool;
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (!audioSource.isPlaying)
        {
            soundPool.Release(gameObject);
        }
    }

	private void OnEnable()
	{
        soundManager = GameObject.Find("Sound Manager").GetComponent<SoundManager>();
        soundManager.soundsPlaying += 1;
	}

	private void OnDisable()
	{
        soundManager.soundsPlaying -= 1;
    }
}
