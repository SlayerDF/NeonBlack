using System;
using System.Threading;
using UnityEngine;

namespace Systems.AudioManagement
{
    public class AudioSourceContainer : IDisposable
    {
        private CancellationTokenSource cts;

        public AudioSourceContainer(AudioSource audioSource)
        {
            Source = audioSource;
        }

        internal AudioSource Source { get; }

        internal PrepareState State { get; set; }

        public void Dispose()
        {
            CancelTask();
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

        internal enum PrepareState
        {
            Finished,
            Starting,
            Stopping
        }
    }
}
