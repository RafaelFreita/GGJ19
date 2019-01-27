using UnityEngine;


namespace TR
{

	public class TrashCanTrigger : MonoBehaviour
	{

		public ParticleSystem fire;
		public GameObject virtualCamera;
		public Animator animator;


		private void OnTriggerEnter2D(Collider2D collision)
		{
			virtualCamera.SetActive(true);
			fire.gameObject.SetActive(true);
			fire.Play();
			//Destroy(collision.gameObject);
			collision.gameObject.GetComponentInChildren<ParticleSystem>().Stop(true);
			collision.gameObject.GetComponent<SlingshotProjectileBehaviour>().SetFinal();
			animator.SetTrigger("Do");

		}

	}
}
