using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCanTrigger : MonoBehaviour
{

    public ParticleSystem fire;
    public GameObject virtualCamera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        virtualCamera.SetActive(true);
        fire.gameObject.SetActive(true);
        fire.Play();
        //Destroy(collision.gameObject);
        collision.gameObject.GetComponentInChildren<ParticleSystem>().Stop(true);
    }

}
