using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour, Interact
{





    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("detected");

        if (other.gameObject.GetComponent<IEntityHealth>() != null)
        {
            Debug.Log("Pressure detected");
            Interact();
        }
    }


    public void Interact()
    {
        GetComponentInParent<ProyectileTrap>().CheckShoot();
    }
}
