using System.Collections;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private SimpleTrap trap;

    [SerializeField]
    private LayerMask activatedBy;

    [SerializeField]
    private Transform visuals;

    [SerializeField]
    private Vector3 visualsPressedPosition;

    [SerializeField]
    private float activateTime = 0.5f;

    #endregion

    private bool activated;

    private int collisionsCount;
    private Vector3 originalPosition;

    #region Event Functions

    private void Awake()
    {
        originalPosition = visuals.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((activatedBy.value & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        collisionsCount++;

        if (collisionsCount == 1 && !activated)
        {
            StartCoroutine(Activate());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((activatedBy.value & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        collisionsCount--;

        if (collisionsCount == 0)
        {
            StartCoroutine(Deactivate());
        }
    }

    #endregion

    private IEnumerator Activate()
    {
        activated = true;

        var elapsedTime = 0f;
        while (elapsedTime < activateTime)
        {
            elapsedTime += Time.deltaTime;

            visuals.localPosition =
                Vector3.Lerp(visuals.localPosition, visualsPressedPosition, elapsedTime / activateTime);

            yield return null;
        }

        trap.Shoot();
    }

    private IEnumerator Deactivate()
    {
        var elapsedTime = 0f;
        while (elapsedTime < activateTime)
        {
            elapsedTime += Time.deltaTime;

            visuals.localPosition = Vector3.Lerp(visuals.localPosition, originalPosition, elapsedTime / activateTime);

            yield return null;
        }

        activated = false;
    }
}
