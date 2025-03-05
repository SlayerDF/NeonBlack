using NeonBlack.Entities.Enemies.Behaviors;
using NeonBlack.Entities.Player;
using UnityEngine;

namespace NeonBlack.Entities.Enemies.Boss
{
    public class Blackboard : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private PlayerController playerController;

        [SerializeField]
        private BossEye leftEye;

        [SerializeField]
        private BossEye rightEye;

        [SerializeField]
        private Transform tempTarget;

        [Header("Behaviors")]
        [SerializeField]
        private LineOfSightByPathBehavior lineOfSightByPathBehavior;

        [SerializeField]
        private CheckVisibilityBehavior checkVisibilityBehavior;

        [SerializeField]
        private LookAtTargetBehavior lookAtTargetBehavior;

        [Header("Properties")]
        [SerializeField]
        private float focusSpeed = 5f;

        [SerializeField]
        private float alertAccumulation = 0.01f;

        [SerializeField]
        private float waitTime = 3f;

        #endregion

        internal PlayerController PlayerController => playerController;
        internal BossEye LeftEye => leftEye;
        internal BossEye RightEye => rightEye;
        internal Transform TempTarget => tempTarget;
        internal LineOfSightByPathBehavior LineOfSightByPathBehavior => lineOfSightByPathBehavior;
        internal CheckVisibilityBehavior CheckVisibilityBehavior => checkVisibilityBehavior;
        internal LookAtTargetBehavior LookAtTargetBehavior => lookAtTargetBehavior;
        internal float FocusSpeed => focusSpeed;
        internal float AlertAccumulation => alertAccumulation;
        internal float WaitTime => waitTime;

        internal Vector3 NotifiedPosition { get; set; }
    }
}
