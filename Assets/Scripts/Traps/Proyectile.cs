using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectile : MonoBehaviour
{


    float _damage;
    float _speed;
    float _lifetime;


    void Start()
    {
        _damage = GetComponentInParent<ProyectileTrap>().Damage;
        _speed = GetComponentInParent<ProyectileTrap>().Speed;
        _lifetime = GetComponentInParent<ProyectileTrap>().LifeTime;
    }

    private void Update()
    {
        transform.position += transform.up * _speed * Time.deltaTime;


        Destroy(gameObject, _lifetime);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<IEntityHealth>() != null)
        {
            collision.gameObject.GetComponent<IEntityHealth>().TakeDamage(_damage);
        }
    }

}
