using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NeonBlack.Entities
{
    public abstract class EntityAnimation : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        protected Animator animator;

        #endregion

        public async UniTask WaitAnimationEnd(int hash, int layer, CancellationToken cancellationToken = default)
        {
            cancellationToken = cancellationToken == default ? destroyCancellationToken : cancellationToken;

            await UniTask.WaitUntil(() => AnimationStarted(hash, layer) is null or true,
                cancellationToken: cancellationToken);

            await UniTask.WaitUntil(() => AnimationEnded(hash, layer) is null or true,
                cancellationToken: cancellationToken);
        }

        public bool? AnimationStarted(int hash, int layer)
        {
            if (!animator || !animator.HasState(layer, hash))
            {
                return null;
            }

            return animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == hash;
        }

        public bool? AnimationEnded(int hash, int layer)
        {
            if (!animator || !animator.HasState(layer, hash))
            {
                return null;
            }

            var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
            if (stateInfo.shortNameHash != hash)
            {
                return null;
            }

            return stateInfo.normalizedTime >= 1f;
        }
    }
}
