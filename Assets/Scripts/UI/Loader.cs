using UnityEngine;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private Image image;

    [SerializeField]
    [Range(0f, 10f)]
    private float fillSpeed = 0.3f;

    [SerializeField]
    [Range(0f, 720f)]
    private float rotationSpeed = 150f;

    #endregion

    #region Event Functions

    private void Update()
    {
        image.fillAmount = (image.fillAmount + fillSpeed * Time.unscaledDeltaTime) % 1f;
        transform.Rotate(0f, 0f, -rotationSpeed * Time.unscaledDeltaTime);
    }

    #endregion
}
