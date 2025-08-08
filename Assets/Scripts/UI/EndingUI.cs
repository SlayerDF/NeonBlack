using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using NeonBlack.Systems.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBlack
{
    public class EndingUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject winText;

        [SerializeField]
        private GameObject failText;

        [SerializeField]
        private GameObject scoreText;

        [SerializeField]
        private TextMeshProUGUI txt_ScoreValue;

        [SerializeField]
        private Button button;

        [SerializeField]
        private SceneReference mainMenuScene;

        private void Awake()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            SceneLoader.LoadScene(mainMenuScene).Forget();
        }

        private void ShowScreen()
        {
            gameObject.SetActive(true);
        }

        public void ShowWinScreen(float points = 0)
        {
            winText.SetActive(true);
            failText.SetActive(false);
            scoreText.SetActive(true);
            txt_ScoreValue.text = $"{points}";

            ShowScreen();
        }

        public void ShowLoseScreen()
        {
            winText.gameObject.SetActive(false);
            failText.gameObject.SetActive(true);
            scoreText.gameObject.SetActive(false);

            ShowScreen();
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}
