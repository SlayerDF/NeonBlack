using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField]
    private SimpleTrap trap;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<IEntityHealth>() != null)
        {
            Debug.Log("Pressure detected");
            Interact();
        }
    }

    public void Interact()
    {
        trap.Shoot();
    }
}
