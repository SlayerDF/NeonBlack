using UnityEngine;

namespace Systems.AudioManagement
{
    public partial class AudioManager
    {
        #region Serialized Fields

        [Header("Clips")]
        [SerializeField]
        private AudioClip dangerClip;

        [SerializeField]
        private AudioClip enemyAlertedClip;

        #endregion

        public static AudioClip DangerClip => Instance.dangerClip;
        public static AudioClip EnemyAlertedClip => Instance.enemyAlertedClip;
    }
}
