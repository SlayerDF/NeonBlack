using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NeonBlack.Systems.AudioManagement
{
    internal enum PlayState
    {
        Finished,
        Starting,
        Stopping
    }

    public class NonSpatialAudio : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private AudioSource audioSource;

        #endregion

        private CancellationTokenSource cts;

        internal AudioSource Source => audioSource;

        internal PlayState State { get; set; }

        internal bool ReadyToStart => State != PlayState.Starting && (State != PlayState.Finished || !Source.isPlaying);
        internal bool ReadyToStop => State != PlayState.Stopping && (State != PlayState.Finished || Source.isPlaying);

        #region Event Functions

        private void OnDestroy()
        {
            CancelTask();
        }

        #endregion

        public async UniTask WaitFinish(CancellationToken cancellationToken = default)
        {
            cancellationToken = cancellationToken == default ? destroyCancellationToken : cancellationToken;

            // Wait transition to finish
            await UniTask.WaitUntil(() => State == PlayState.Finished, cancellationToken: cancellationToken);

            if (!Source.isPlaying || Source.loop)
            {
                return;
            }

            var clip = Source.clip;

            await UniTask.WaitWhile(() => Source.isPlaying && Source.clip == clip,
                cancellationToken: cancellationToken);
        }

        internal void CancelTask()
        {
            if (cts == null)
            {
                return;
            }

            cts.Cancel();
            cts.Dispose();
            cts = null;
        }

        internal CancellationTokenSource StartTask()
        {
            CancelTask();
            cts = new CancellationTokenSource();
            return cts;
        }
    }
}
