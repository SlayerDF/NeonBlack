using System;
using UnityEngine;

public partial class LevelState
{
    #region Serialized Fields

    [SerializeField]
    [Range(0f, 1f)]
    private float cooldownPoint = 0.8f;

    [SerializeField]
    [Range(0f, 1f)]
    private float cooldownRate = 0.01f;

    #endregion

    private float alert;

    public static float CooldownPoint => Instance.cooldownPoint;
    public static float Alert => Instance.alert;

    private void AwakeAlert()
    {
        CooldownPointChanged?.Invoke(cooldownPoint);
        AlertChanged?.Invoke(alert);
    }

    private void FixedUpdateAlert()
    {
        if (alert > cooldownPoint)
        {
            UpdateAlert(-cooldownRate * Time.fixedDeltaTime);
        }
    }

    public static event Action<float> CooldownPointChanged;
    public static event Action<float> AlertChanged;

    public static void UpdateAlert(float diff)
    {
        Instance.alert = Mathf.Clamp(Instance.alert + diff, 0f, 1f);

        AlertChanged?.Invoke(Instance.alert);
    }
}
