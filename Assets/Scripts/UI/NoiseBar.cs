using NeonBlack.Systems.LevelState;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBlack.UI
{
    public class NoiseBar : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private Image fillLeft;

        [SerializeField]
        private Image fillRight;

        [SerializeField]
        [Range(0, 1f)]
        private float fillAmount;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float blinkFrequency = 0.1f;

        #endregion

        private float blinkTimer;
        private Vector2 targetFillSize;

        #region Event Functions

        private void Update()
        {
            fillLeft.fillAmount = Mathf.Lerp(fillLeft.fillAmount, fillAmount, 0.1f);
            fillRight.fillAmount = fillLeft.fillAmount;

            if (fillAmount == 0f)
            {
                return;
            }

            if ((blinkTimer += Time.deltaTime) > blinkFrequency)
            {
                blinkTimer = 0f;
                targetFillSize = new Vector2(100f, Random.Range(5f, 25f));
            }

            fillLeft.rectTransform.sizeDelta = Vector2.Lerp(fillLeft.rectTransform.sizeDelta, targetFillSize, 0.5f);
            fillRight.rectTransform.sizeDelta = fillLeft.rectTransform.sizeDelta;
        }

        private void OnEnable()
        {
            LevelState.NoiseChanged += OnNoiseChanged;
        }

        private void OnDisable()
        {
            LevelState.NoiseChanged -= OnNoiseChanged;
        }

        #endregion

        private void OnNoiseChanged(float value)
        {
            fillAmount = value;
        }
    }
}
