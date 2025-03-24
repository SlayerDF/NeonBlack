using System.Collections.Generic;
using NeonBlack.Entities.Player;
using NeonBlack.Systems.LevelState;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBlack.UI
{
    public class EffectsBar : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private VerticalLayoutGroup effectsList;

        [SerializeField]
        private CollectibleIcon collectibleIconPrefab;

        #endregion

        private readonly Dictionary<PlayerEffects.EffectData, CollectibleIcon> effectsIcons = new();

        #region Event Functions

        private void FixedUpdate()
        {
            foreach (var (effectData, collectibleIcon) in effectsIcons)
            {
                collectibleIcon.Counter = (int)effectData.CurrentDuration;
            }
        }

        private void OnEnable()
        {
            LevelState.LevelStarted += OnLevelStarted;
            PlayerEffects.EffectAdded += OnEffectAdded;
            PlayerEffects.EffectRemoved += OnEffectRemoved;
        }

        private void OnDisable()
        {
            LevelState.LevelStarted -= OnLevelStarted;
            PlayerEffects.EffectAdded -= OnEffectAdded;
            PlayerEffects.EffectRemoved -= OnEffectRemoved;
        }

        #endregion

        private void OnEffectAdded(PlayerEffects.EffectData effectData)
        {
            var collectibleIcon = Instantiate(collectibleIconPrefab, effectsList.transform);
            collectibleIcon.Icon = effectData.Effect.Icon;
            collectibleIcon.IconAlpha = effectData.Effect.IconAlpha;
            collectibleIcon.Counter = (int)effectData.CurrentDuration;

            effectsIcons.Add(effectData, collectibleIcon);
        }

        private void OnEffectRemoved(PlayerEffects.EffectData effectData)
        {
            var collectibleIcon = effectsIcons[effectData];

            Destroy(collectibleIcon.gameObject);

            effectsIcons.Remove(effectData);
        }

        private void OnLevelStarted()
        {
            foreach (var icon in effectsIcons.Values)
            {
                Destroy(icon.gameObject);
            }

            effectsIcons.Clear();
        }
    }
}
