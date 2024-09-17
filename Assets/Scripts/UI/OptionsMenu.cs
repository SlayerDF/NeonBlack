using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private Slider framerateSlider;

    [SerializeField]
    private TMP_Text framerateText;

    #endregion

    private CancellationTokenSource saveTaskCts;

    #region Event Functions

    private void Start()
    {
        framerateSlider.value = PlayerPrefs.GetInt("FPS", 60);
    }

    private void OnEnable()
    {
        framerateSlider.onValueChanged.AddListener(UpdateFramerate);
    }

    private void OnDisable()
    {
        framerateSlider.onValueChanged.RemoveListener(UpdateFramerate);
    }

    #endregion

    private void UpdateFramerate(float value)
    {
        var intValue = (int)value;

        Application.targetFrameRate = intValue;
        framerateText.text = intValue.ToString();

        SaveSettings().Forget();
    }

    private async UniTask SaveSettings()
    {
        // Debounce saving
        saveTaskCts?.Cancel();
        saveTaskCts = new CancellationTokenSource();

        await UniTask.WaitForSeconds(1f, cancellationToken: saveTaskCts.Token);

        PlayerPrefs.SetInt("FPS", Application.targetFrameRate);
        PlayerPrefs.Save();
        Debug.Log("Settings saved");
    }
}
