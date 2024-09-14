using UnityEngine;

public partial class LevelState : MonoBehaviour
{
    private static LevelState _instance;

    public static bool Instantiated => _instance != null;

    #region Event Functions

    private void Awake()
    {
        _instance = this;

        AwakeAlert();
    }

    private void FixedUpdate()
    {
        FixedUpdateAlert();
    }

    #endregion
}
