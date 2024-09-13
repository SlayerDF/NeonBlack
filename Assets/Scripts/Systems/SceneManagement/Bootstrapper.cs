using Eflatun.SceneReference;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private SceneGroup[] sceneGroups;

    [SerializeField]
    private SceneReference defaultScene;

    #endregion

    public SceneGroup[] SceneGroups => sceneGroups;

    public SceneReference DefaultScene => defaultScene;
}
