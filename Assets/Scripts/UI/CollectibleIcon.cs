using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NeonBlack.UI
{
    public class CollectibleIcon : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private Image icon;

        [SerializeField]
        private Image outline;

        [SerializeField]
        private TMP_Text counterText;

        #endregion

        public Sprite Icon
        {
            set => icon.sprite = value;
        }

        public Sprite IconAlpha
        {
            set => outline.sprite = value;
        }

        public int Counter
        {
            set => counterText.text = value.ToString();
        }
    }
}
