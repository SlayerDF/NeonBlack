using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour, IEntityHealth
{
    public void TakeDamage(float dmg)
    {
        Debug.Log("Player took " + dmg + "damage.");
    }
}
