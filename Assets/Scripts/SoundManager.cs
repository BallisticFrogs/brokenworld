using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager INSTANCE;

    [SceneObjectsOnly] public AudioSource dragScreen;
    [SceneObjectsOnly] public AudioSource dragIsland;
    [SceneObjectsOnly] public AudioSource makeLight;

    [AssetsOnly] public AudioClip aggregateIsland;
    [AssetsOnly] public List<AudioClip> harvest;
    [AssetsOnly] public List<AudioClip> peopleCheering;
    [AssetsOnly] public List<AudioClip> peopleDying;

    private AudioSource audioSource;

    private readonly Dictionary<AudioSource, float> volumeBySource = new Dictionary<AudioSource, float>();
    private readonly Dictionary<AudioSource, Tween> tweenBySource = new Dictionary<AudioSource, Tween>();

    private void Awake()
    {
        INSTANCE = this;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        volumeBySource.Add(dragScreen, dragScreen.volume);
        volumeBySource.Add(dragIsland, dragIsland.volume);
        volumeBySource.Add(makeLight, makeLight.volume);
    }

    public void StopAllSounds()
    {
        foreach (var source in volumeBySource.Keys)
        {
            StopSustained(source);
        }

        audioSource.Stop();
    }

    public void PlaySustained(AudioSource source)
    {
        if (source.isPlaying)
        {
            // no op
        }
        else
        {
            if (tweenBySource.TryGetValue(source, out var tween))
            {
                tween.Kill();
            }

            tweenBySource[source] = source.DOFade(volumeBySource[source], 0.2f);

            source.Play();
        }
    }

    public void StopSustained(AudioSource source)
    {
        if (tweenBySource.TryGetValue(source, out var tween))
        {
            tween.Kill();
        }

        if (source.isPlaying)
        {
            tweenBySource[source] = DOTween.Sequence()
                .Append(source.DOFade(0, 0.3f))
                .AppendCallback(source.Stop);
        }
    }

    public void Play(AudioClip clip, float volume)
    {
        audioSource.PlayOneShot(clip, volume * 0.7f);
    }

    public void Play(AudioClip clip, float volume, float minDelay, float maxDelay)
    {
        var delay = Random.Range(minDelay, maxDelay);
        DOTween.Sequence()
            .AppendInterval(delay)
            .AppendCallback(() => audioSource.PlayOneShot(clip, volume * 0.7f));
    }

    public void Play(List<AudioClip> clips, float volume, float minDelay, float maxDelay)
    {
        var idx = Random.Range(0, clips.Count - 1);
        var clip = clips[idx];
        Play(clip, volume, minDelay, maxDelay);
    }
}