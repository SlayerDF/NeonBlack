using NeonBlack.Entities.Enemies.Boss;
using UnityEngine;

namespace NeonBlack.Levels
{
    public class BossLevelController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Exit Island logic")]
        [SerializeField]
        private GameObject exitIslandRoot;

        [SerializeField]
        private Vector3 exitIslandStartPosition;

        [SerializeField]
        private Vector3 exitIslandEndPosition;

        [SerializeField]
        private AnimationCurve exitIslandMovementCurve;

        #endregion

        private bool moveIsland;
        private float moveIslandTimer;

        #region Event Functions

        private void Start()
        {
            exitIslandRoot.SetActive(false);
            exitIslandRoot.transform.position = exitIslandStartPosition;
        }

        private void Update()
        {
            if (!moveIsland)
            {
                return;
            }

            var progress = exitIslandMovementCurve.Evaluate(moveIslandTimer += Time.deltaTime);

            if (progress >= 1f)
            {
                moveIsland = false;
                return;
            }

            exitIslandRoot.transform.position = Vector3.Lerp(exitIslandStartPosition, exitIslandEndPosition, progress);
        }

        private void OnEnable()
        {
            BossHealth.Death += MoveIsland;
        }

        private void OnDisable()
        {
            BossHealth.Death -= MoveIsland;
        }

        #endregion

        [ContextMenu("Move island")]
        private void MoveIsland()
        {
            moveIsland = true;
            moveIslandTimer = 0f;
            exitIslandRoot.SetActive(true);
        }

        [ContextMenu("Hide island")]
        private void HideIsland()
        {
            exitIslandRoot.SetActive(false);
        }
    }
}
