using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastFireplace : MonoBehaviour
{

    public GameObject fireObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        fireObject.SetActive(true);
        var particleSystems = collision.gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach(var p in particleSystems)
        {
            p.Play();
        }
    }
}
