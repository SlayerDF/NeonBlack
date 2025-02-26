using System;
using NeonBlack.Entities.Enemies.Behaviors;
using NeonBlack.Entities.Player;
using UnityEngine;
using UnityEngine.AI;

namespace NeonBlack.Entities.Enemies.SimpleEnemy
{
    [Serializable]
    public class Blackboard : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private BossBrain bossBrain;

        [SerializeField]
        private EnemyHealth enemyHealth;

        [SerializeField]
        private Collider enemyCollider;

        [SerializeField]
        private EnemyAnimation enemyAnimation;

        [SerializeField]
        private NavMeshAgent navAgent;

        [Header("Behaviors")]
        [SerializeField]
        private LineOfSightBehavior lineOfSightBehavior;

        [SerializeField]
        private CheckVisibilityBehavior checkVisibilityBehavior;

        [SerializeField]
        private PatrolBehavior patrolBehavior;

        [SerializeField]
        private GoToBehavior goToBehavior;

        [SerializeField]
        private PlayerDetectionBehavior playerDetectionBehavior;

        [SerializeField]
        private LookAtTargetBehavior lookAtTargetBehavior;

        [SerializeField]
        private ShootPlayerBehavior shootPlayerBehavior;

        [Header("Properties")]
        [SerializeField]
        private float attackDelay = 1f;

        [SerializeField]
        private float notifyBossDelay = 3f;

        [SerializeField]
        private float distractedRotationSpeed = 180f;

        [Header("Visuals")]
        [SerializeField]
        private MeshRenderer lineOfSightVisuals;

        [SerializeField]
        [ColorUsage(true, true)]
        private Color highAlertColor;

        [SerializeField]
        [ColorUsage(true, true)]
        private Color lowAlertColor;

        #endregion

        internal BossBrain BossBrain => bossBrain;
        internal Collider EnemyCollider => enemyCollider;
        internal EnemyHealth EnemyHealth => enemyHealth;
        internal EnemyAnimation EnemyAnimation => enemyAnimation;
        internal NavMeshAgent NavAgent => navAgent;
        internal LineOfSightBehavior LineOfSightBehavior => lineOfSightBehavior;
        internal CheckVisibilityBehavior CheckVisibilityBehavior => checkVisibilityBehavior;
        internal PatrolBehavior PatrolBehavior => patrolBehavior;
        internal GoToBehavior GoToBehavior => goToBehavior;
        internal PlayerDetectionBehavior PlayerDetectionBehavior => playerDetectionBehavior;
        internal LookAtTargetBehavior LookAtTargetBehavior => lookAtTargetBehavior;
        internal ShootPlayerBehavior ShootPlayerBehavior => shootPlayerBehavior;
        internal float AttackDelay => attackDelay;
        internal float NotifyBossDelay => notifyBossDelay;
        internal float DistractedRotationSpeed => distractedRotationSpeed;
        internal MeshRenderer LineOfSightVisuals => lineOfSightVisuals;
        internal Color HighAlertColor => highAlertColor;
        internal Color LowAlertColor => lowAlertColor;

        internal PlayerController PlayerController { get; set; }
        internal Vector3 LastSeenPlayerPosition { get; set; }
        internal GameObject DistractionGameObject { get; set; }
        internal float DistractionTime { get; set; }
        internal SimpleEnemyBrain wakeUpAllyTarget { get; set; }
    }
}
