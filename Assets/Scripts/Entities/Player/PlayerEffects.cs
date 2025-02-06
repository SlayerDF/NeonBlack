using System;
using System.Collections.Generic;
using NeonBlack.Effects;
using NeonBlack.Systems.AudioManagement;
using UnityEngine;

namespace NeonBlack.Entities.Player
{
    public class PlayerEffects : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Components")]
        [SerializeField]
        private PlayerController playerController;

        #endregion

        private readonly List<EffectData> effects = new();

        #region Event Functions

        private void FixedUpdate()
        {
            for (var i = 0; i < effects.Count; i++)
            {
                if (!effects[i].DurationElapsed)
                {
                    continue;
                }

                effects[i].CurrentDuration -= Time.fixedDeltaTime;

                if (!effects[i].DurationElapsed)
                {
                    DeactivateEffect(effects[i]);
                }
            }
        }

        #endregion

        public void AddEffect(Effect effect)
        {
            var existingEffect = effects.Find(x => x.Effect.GetType().Name == effect.GetType().Name);

            AudioManager.Play(AudioManager.InteractionsPrefab, AudioManager.ItemPickupClip, transform.position);

            if (existingEffect == null)
            {
                var effectData = new EffectData(effect);
                effects.Add(effectData);
                ActivateEffect(effectData);
                return;
            }

            if (!existingEffect.DurationElapsed)
            {
                ActivateEffect(existingEffect);
            }

            existingEffect.CurrentDuration = effect.Duration;
        }

        private void ActivateEffect(EffectData effectData)
        {
            effectData.Effect.Activate(playerController);
            EffectAdded?.Invoke(effectData);
        }

        private void DeactivateEffect(EffectData effectData)
        {
            effectData.Effect.Deactivate(playerController);
            EffectRemoved?.Invoke(effectData);
        }

        #region Nested type: ${0}

        public class EffectData
        {
            public EffectData(Effect effect)
            {
                Effect = effect;
                CurrentDuration = effect.Duration;
            }

            public Effect Effect { get; }
            public float CurrentDuration { get; set; }
            public bool DurationElapsed => CurrentDuration > 0;
        }

        #endregion

        #region Events

        public static event Action<EffectData> EffectAdded;
        public static event Action<EffectData> EffectRemoved;

        #endregion
    }
}
