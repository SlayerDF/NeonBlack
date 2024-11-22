using UnityEngine;

namespace NeonBlack.Systems.AudioManagement
{
    public partial class AudioManager
    {
        #region Serialized Fields

        [Header("SFX clips")]
        [SerializeField]
        private AudioClip dangerClip;

        [SerializeField]
        private AudioClip enemyAlertedClip;

        [SerializeField]
        private AudioClip playerFootstepsClip;

        [SerializeField]
        private AudioClip enemyFootstepsClip;

        [SerializeField]
        private AudioClip simpleEnemyShootClip;

        [SerializeField]
        private AudioClip playerHitClip;

        [SerializeField]
        private AudioClip playerHitResultClip;

        [SerializeField]
        private AudioClip playerDeathMusicClip;

        [SerializeField]
        private AudioClip shardCollectedClip;

        [SerializeField]
        private AudioClip itemPickupClip;

        [Header("Music clips")]
        [SerializeField]
        private AudioClip demoLevelMusicClip;

        #endregion

        public static AudioClip DangerClip => Instance.dangerClip;
        public static AudioClip EnemyAlertedClip => Instance.enemyAlertedClip;
        public static AudioClip PlayerFootstepsClip => Instance.playerFootstepsClip;
        public static AudioClip EnemyFootstepsClip => Instance.enemyFootstepsClip;
        public static AudioClip SimpleEnemyShootClip => Instance.simpleEnemyShootClip;
        public static AudioClip PlayerHitClip => Instance.playerHitClip;
        public static AudioClip PlayerHitResultClip => Instance.playerHitResultClip;
        public static AudioClip PlayerDeathMusicClip => Instance.playerDeathMusicClip;
        public static AudioClip ShardCollectedClip => Instance.shardCollectedClip;
        public static AudioClip ItemPickupClip => Instance.itemPickupClip;
        public static AudioClip DemoLevelMusicClip => Instance.demoLevelMusicClip;
    }
}
