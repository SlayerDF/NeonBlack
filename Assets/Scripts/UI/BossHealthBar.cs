using NeonBlack.Entities.Enemies.Boss;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBlack.UI
{
    public class BossHealthBar : MonoBehaviour
    {
        private const float AnimationDuration = 1f;

        #region Serialized Fields

        [SerializeField]
        private Image fillImage;

        [SerializeField]
        private GameObject root;

        #endregion

        private float animationDelta;

        private float currentValue = 1f;

        #region Event Functions

        private void Awake()
        {
            root.SetActive(false);
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

        private void OnEnable()
        {
            BossHealth.HealthChanged += OnBossHealthChanged;
        }

        private void OnDisable()
        {
            BossHealth.HealthChanged -= OnBossHealthChanged;
        }

        #endregion

        private void OnBossHealthChanged(float health, float maxHealth)
        {
            if (!root.activeInHierarchy)
            {
                root.SetActive(true);
            }

            currentValue = health / maxHealth;
            animationDelta = Mathf.Abs(currentValue - fillImage.fillAmount) / AnimationDuration;
        }
    }
}
