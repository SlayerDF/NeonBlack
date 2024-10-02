using TMPro;
using UnityEngine;

namespace NeonBlack.UI
{
    public class ButtonRename : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private TMP_Text text;

        #endregion

        #region Event Functions

#if UNITY_EDITOR
        private void OnValidate()
        {
            text.text = name;
        }
#endif

        #endregion
    }
}
