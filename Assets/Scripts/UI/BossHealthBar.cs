using NeonBlack.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBlack.UI
{
    public class BossHealthBar : SceneSingleton<BossHealthBar>
    {
        private const float AnimationDuration = 1f;

        #region Serialized Fields

        [SerializeField]
        private Image fillImage;

        #endregion

        private float animationDelta;

        private float currentValue = 1f;

        #region Event Functions

        protected override void Awake()
        {
            base.Awake();
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Mathf.Approximately(fillImage.fillAmount, currentValue))
            {
                return;
            }

            fillImage.fillAmount =
                Mathf.MoveTowards(fillImage.fillAmount, currentValue, animationDelta * Time.deltaTime);

            if (fillImage.fillAmount <= 0f)
            {
                gameObject.SetActive(false);
            }
        }

        #endregion

        private void UpdateValueInternal(float value)
        {
            if (!isActiveAndEnabled)
            {
                gameObject.SetActive(true);
            }

            currentValue = value;
            animationDelta = Mathf.Abs(currentValue - fillImage.fillAmount) / AnimationDuration;
        }

        public static void UpdateValue(float value)
        {
            if (Instance == null)
            {
                return;
            }

            Instance.UpdateValueInternal(value);
        }
    }
}
