using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectileTrap : MonoBehaviour
{

    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float LifeTime { get; private set; }

    [SerializeField] bool _shootReady;
    [SerializeField] float _shootQuantity;

    [Header("References")]

    [SerializeField] Transform _bulletSpawnPoint;

    [SerializeField] GameObject _proyectile;



    public void CheckShoot()
    {
        if (_shootReady) StartCoroutine(Shoot());
    } 

    IEnumerator Shoot()
    {
        _shootReady = false;

        for (int i = 0; i < _shootQuantity; i++)
        {
            Instantiate(_proyectile, _bulletSpawnPoint.position, _bulletSpawnPoint.rotation, this.transform);

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1f);

        _shootReady = true;
    }
}
