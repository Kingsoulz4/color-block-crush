using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSourcePool : SingletonMono<AudioSourcePool>
{
    [Header("Pool Configuration")]
    [SerializeField] private int poolSize = 8;
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
            if (!pool[i].isPlaying)
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
            selectedSource.clip = clip;
            selectedSource.volume = volume;
            selectedSource.Play();
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
        source.clip = clip;
        source.volume = volume;
        source.Play();
        lastPlayTime[roundRobinIndex] = Time.time;

        roundRobinIndex = (roundRobinIndex + 1) % poolSize;
    }

    public bool PlaySFX(AudioClip clip, float volumeScale = 1f, float pitch = 1f)
    {
        if (clip == null) return false;

        totalPlayRequests++;

        for (int i = 0; i < poolSize; i++)
        {
            if (!pool[i].isPlaying)
            {
                AudioSource source = pool[i];
                source.clip = clip;
                source.volume = volume * volumeScale;
                source.pitch = pitch;
                source.Play();
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

    private System.Collections.IEnumerator ResetPitchAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.pitch = 1f;
    }

    public void GetStatistics(out int total, out int skipped, out int activeCount)
    {
        total = totalPlayRequests;
        skipped = skippedRequests;

        activeCount = 0;
        for (int i = 0; i < poolSize; i++)
        {
            if (Time.time < lastPlayTime[i] + 2f)
            {
                activeCount++;
            }
        }
    }

    public void Mute(bool value)
    {
        foreach (var source in pool)
        {
            source.mute = value;
            Debug.Log(source.mute);
        }
    }

    private void OnDestroy()
    {
        StopAll();
    }
}

