using NeonBlack.Systems.LevelState;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBlack.UI
{
    public class AlertBar : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private RectTransform barTransform;

        [SerializeField]
        private Image cooldownPointImage;

        [SerializeField]
        private Image alertFillImage;

        [SerializeField]
        private Color lowAlertColor;

        [SerializeField]
        private Color mediumAlertColor;

        [SerializeField]
        private Color highAlertColor;

        [SerializeField]
        private Color maxAlertColor;

        #endregion

        private float cooldownPoint;

        #region Event Functions

        private void OnEnable()
        {
            if (LevelState.Instantiated)
            {
                OnCooldownPointChanged(LevelState.CooldownPoint);
                OnAlertChanged(LevelState.Alert);
            }

            LevelState.CooldownPointChanged += OnCooldownPointChanged;
            LevelState.AlertChanged += OnAlertChanged;
        }

        private void OnDisable()
        {
            LevelState.CooldownPointChanged -= OnCooldownPointChanged;
            LevelState.AlertChanged -= OnAlertChanged;
        }

        #endregion

        private void OnCooldownPointChanged(float value)
        {
            cooldownPoint = value;

            var tr = cooldownPointImage.rectTransform;
            tr.offsetMax = new Vector2(-barTransform.rect.width * (1f - value), tr.offsetMax.y);
        }

        private void OnAlertChanged(float value)
        {
            cooldownPointImage.enabled = value - cooldownPoint > -0.01f;

            alertFillImage.fillAmount = value;

            if (value < cooldownPoint / 2f)
            {
                alertFillImage.color = Color.Lerp(lowAlertColor, mediumAlertColor,
                    Mathf.InverseLerp(0f, cooldownPoint / 2f, value));
            }
            else if (value < cooldownPoint)
            {
                alertFillImage.color = Color.Lerp(mediumAlertColor, highAlertColor,
                    Mathf.InverseLerp(cooldownPoint / 2f, cooldownPoint, value));
            }
            else
            {
                alertFillImage.color = Color.Lerp(highAlertColor, maxAlertColor,
                    Mathf.InverseLerp(cooldownPoint, 1f, value));
            }
        }
    }
}
