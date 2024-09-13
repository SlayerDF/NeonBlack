using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private SceneGroup defaultSceneGroup;

    #endregion

    public SceneGroup DefaultSceneGroup => defaultSceneGroup;
}
