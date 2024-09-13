using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SubscribableCollider : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private Collider coll;

    #endregion

    #region Event Functions

    private void OnEnable()
    {
        coll.enabled = true;
    }

    private void OnDisable()
    {
        coll.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        TriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerExit?.Invoke(other);
    }

    #endregion

    public event Action<Collider> TriggerEnter;
    public event Action<Collider> TriggerExit;
}
