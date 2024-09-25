using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioManager : SceneSingleton<AudioManager>
{
    #region SourceType enum

    public enum SourceType
    {
        Notification
    }

    #endregion

    #region Serialized Fields

    [Header("Properties")]
    [SerializeField]
    private float fadeSpeed = 2f;

    [Header("Sources")]
    [SerializeField]
    private AudioSource notificationsSource;

    [Header("Clips")]
    [SerializeField]
    private AudioClip dangerClip;

    #endregion

    private readonly Dictionary<AudioSource, FadeVolumeTaskContext> sourcesTaskContexts = new();

    public static AudioClip DangerClip => Instance.dangerClip;

    #region Event Functions

    protected override void OnDestroy()
    {
        base.OnDestroy();

        StopAll();
    }

    #endregion

    public static void Play(SourceType sourceType, AudioClip clip, bool loop = false)
    {
        var src = GetSource(sourceType);

        if (src.isPlaying && src.clip == clip)
        {
            return;
        }

        Instance.PlayAsync(src, clip, loop).Forget();
    }

    public static void Stop(SourceType sourceType, AudioClip clip)
    {
        var src = GetSource(sourceType);

        if (!src.isPlaying || src.clip != clip)
        {
            return;
        }

        Instance.StopAsync(src, clip).Forget();
    }

    public static void StopAll()
    {
        if (Instance == null)
        {
            return;
        }

        foreach (var (src, ctx) in Instance.sourcesTaskContexts)
        {
            ctx.CancellationTokenSource.Cancel();
            ctx.CancellationTokenSource.Dispose();
            src.Stop();
        }

        Instance.sourcesTaskContexts.Clear();
    }

    private async UniTask PlayAsync(AudioSource src, AudioClip clip, bool loop = false)
    {
        if (src.isPlaying && src.clip == clip)
        {
            return;
        }

        Instance.sourcesTaskContexts.TryGetValue(src, out var ctx);

        if (ctx.Type != FadeVolumeTaskContext.TaskType.None)
        {
            ctx.CancellationTokenSource.Cancel();
            ctx.CancellationTokenSource.Dispose();
            Instance.sourcesTaskContexts.Remove(src);
        }

        ctx = new FadeVolumeTaskContext(FadeVolumeTaskContext.TaskType.Play);
        sourcesTaskContexts.Add(src, ctx);

        if (src.isPlaying)
        {
            await FadeVolumeAsync(src, 0f, fadeSpeed, ctx.CancellationTokenSource.Token);

            if (ctx.CancellationTokenSource.IsCancellationRequested)
            {
                return;
            }
        }

        src.volume = 0f;
        src.loop = loop;
        src.clip = clip;
        src.Play();

        await FadeVolumeAsync(src, 1f, fadeSpeed, ctx.CancellationTokenSource.Token);
    }

    private async UniTask StopAsync(AudioSource src, AudioClip clip)
    {
        if (!src.isPlaying || src.clip != clip)
        {
            return;
        }

        Instance.sourcesTaskContexts.TryGetValue(src, out var ctx);

        if (ctx.Type == FadeVolumeTaskContext.TaskType.Stop)
        {
            return;
        }

        if (ctx.Type != FadeVolumeTaskContext.TaskType.None)
        {
            ctx.CancellationTokenSource.Cancel();
            ctx.CancellationTokenSource.Dispose();
            Instance.sourcesTaskContexts.Remove(src);
        }

        ctx = new FadeVolumeTaskContext(FadeVolumeTaskContext.TaskType.Stop);
        sourcesTaskContexts.Add(src, ctx);

        await FadeVolumeAsync(src, 0f, fadeSpeed, ctx.CancellationTokenSource.Token);

        if (ctx.CancellationTokenSource.IsCancellationRequested)
        {
            return;
        }

        src.Stop();
    }

    private static AudioSource GetSource(SourceType sourceType)
    {
        return sourceType switch
        {
            SourceType.Notification => Instance.notificationsSource,
            _ => throw new InvalidEnumArgumentException(nameof(sourceType), (int)sourceType, typeof(SourceType))
        };
    }

    private static async UniTask FadeVolumeAsync(AudioSource src, float targetVolume, float speed, CancellationToken ct)
    {
        while (!Mathf.Approximately(src.volume, targetVolume) && !ct.IsCancellationRequested)
        {
            src.volume = Mathf.MoveTowards(src.volume, targetVolume, speed * Time.deltaTime);

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        src.volume = targetVolume;
    }

    #region Nested type: ${0}

    private readonly struct FadeVolumeTaskContext
    {
        public enum TaskType
        {
            None,
            Play,
            Stop
        }

        public readonly TaskType Type;
        public readonly CancellationTokenSource CancellationTokenSource;

        public FadeVolumeTaskContext(TaskType type)
        {
            Type = type;
            CancellationTokenSource = new CancellationTokenSource();
        }
    }

    #endregion
}
