using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastFireplace : MonoBehaviour
{
    
    public GameObject fireObject;
    public Animator animator;

	private void OnTriggerEnter2D(Collider2D collision)
    {
        fireObject.SetActive(true);
        var particleSystems = collision.gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach(var p in particleSystems)
        {
            p.Play();
        }
		animator.SetTrigger("Do");

	}
}
