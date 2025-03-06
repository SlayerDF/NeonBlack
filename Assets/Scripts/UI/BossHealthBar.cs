using NeonBlack.Entities.Enemies.Boss;
using NeonBlack.Systems.LevelState;
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

        private float currentValue;

        #region Event Functions

        private void Awake()
        {
            OnLevelStarted();
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
            LevelState.LevelStarted += OnLevelStarted;
        }

        private void OnDisable()
        {
            BossHealth.HealthChanged -= OnBossHealthChanged;
            LevelState.LevelStarted -= OnLevelStarted;
        }

        #endregion

        private void OnLevelStarted()
        {
            currentValue = 1f;
            root.SetActive(false);
        }

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
