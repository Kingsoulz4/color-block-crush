using UnityEngine;
using UnityEngine.Audio;

public class AudioSourcePool : SingletonMono<AudioSourcePool>
{
    [Header("Pool Configuration")]
    [SerializeField] private int poolSize = 20;
    [SerializeField] private bool allowOverride = true;

    [Header("Audio Settings")]
    [SerializeField] private float volume = 1f;
    [SerializeField] private AudioMixerGroup mixerGroup;

    private AudioSource[] pool;
    private float[] lastPlayTime;
    private int roundRobinIndex = 0;

    private int totalPlayRequests = 0;
    private int skippedRequests = 0;

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        pool = new AudioSource[poolSize];
        lastPlayTime = new float[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            GameObject audioObject = new GameObject($"PooledAudioSource_{i}");
            audioObject.transform.SetParent(transform);

            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.volume = volume;

            if (mixerGroup != null)
                source.outputAudioMixerGroup = mixerGroup;

            pool[i] = source;
            lastPlayTime[i] = -999f;
        }
    }

    public bool PlaySFX(AudioClip clip)
    {
        if (clip == null) return false;

        totalPlayRequests++;

        AudioSource selectedSource = null;
        int selectedIndex = -1;

        for (int i = 0; i < poolSize; i++)
        {
            if (Time.time >= lastPlayTime[i] + GetClipLength(pool[i]))
            {
                selectedSource = pool[i];
                selectedIndex = i;
                break;
            }
        }

        if (selectedSource == null && allowOverride)
        {
            float oldestTime = float.MaxValue;
            for (int i = 0; i < poolSize; i++)
            {
                if (lastPlayTime[i] < oldestTime)
                {
                    oldestTime = lastPlayTime[i];
                    selectedSource = pool[i];
                    selectedIndex = i;
                }
            }
        }

        if (selectedSource != null)
        {
            selectedSource.PlayOneShot(clip, volume);
            lastPlayTime[selectedIndex] = Time.time;
            return true;
        }

        skippedRequests++;
        return false;
    }

    public void PlaySFXRoundRobin(AudioClip clip)
    {
        if (clip == null) return;

        totalPlayRequests++;

        AudioSource source = pool[roundRobinIndex];
        source.PlayOneShot(clip, volume);
        lastPlayTime[roundRobinIndex] = Time.time;

        roundRobinIndex = (roundRobinIndex + 1) % poolSize;
    }

    public bool PlaySFXAdvanced(AudioClip clip, float volumeScale = 1f, float pitch = 1f)
    {
        if (clip == null) return false;

        totalPlayRequests++;

        for (int i = 0; i < poolSize; i++)
        {
            if (Time.time >= lastPlayTime[i] + GetClipLength(pool[i]))
            {
                AudioSource source = pool[i];
                source.pitch = pitch;
                source.PlayOneShot(clip, volume * volumeScale);
                lastPlayTime[i] = Time.time;

                if (pitch != 1f)
                {
                    StartCoroutine(ResetPitchAfterPlay(source, clip.length));
                }

                return true;
            }
        }

        skippedRequests++;
        return false;
    }

    public void StopAll()
    {
        for (int i = 0; i < poolSize; i++)
        {
            pool[i].Stop();
            lastPlayTime[i] = -999f;
        }
    }

    private float GetClipLength(AudioSource source)
    {
        return source.clip != null ? source.clip.length : 0f;
    }

    private System.Collections.IEnumerator ResetPitchAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.pitch = 1f;
    }
}