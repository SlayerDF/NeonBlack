using NeonBlack.Entities.Player;
using NeonBlack.Systems.LevelState;
using UnityEngine;

namespace NeonBlack
{
    [RequireComponent(typeof(Collider))]
    public class ScoreCollectible : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private float scoreForCollection;

        #endregion

        #region Event Functions

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerController _))
            {
                return;
            }

            LevelState.IncrementScore(scoreForCollection);
            Destroy(gameObject);
        }

        #endregion
    }
}
