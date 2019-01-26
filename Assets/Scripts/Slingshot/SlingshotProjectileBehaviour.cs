using BitStrap;
using LUT;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TR
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class SlingshotProjectileBehaviour : MonoBehaviour
	{

		private Transform currentPivot;
		private Fireplace lastFireplace;
		private Fireplace currentFireplace;

		private Rigidbody2D targetRigidbody;

		[SerializeField]
		private LayerMask killerMask;
		private ParticleSystem[] particleSystems;

		[Header("Drag")]
		[SerializeField]
		[Unit("m")]
		private float minDistance = 0.5f;
		[SerializeField]
		[Unit("m")]
		private float maxDistance = 4;

		[Header("Force")]
		[SerializeField]
		private bool mapDistanceToForce = false;
		[SerializeField]
		[ShowIf("mapDistanceToForce", false)]
		private float forceMultiplier = 2f;
		[ShowIf("mapDistanceToForce")]
		[SerializeField]
		private float minForce = 0.5f;
		[ShowIf("mapDistanceToForce")]
		[SerializeField]
		private float maxForce = 4f;

		[Space]
		[SerializeField]
		private ForceMode2D forceMode2D = ForceMode2D.Impulse;

		[Header("Collision")]
		[SerializeField]
		[LayerSelector]
		private int pivotLayer = 0;

		[Header("Events")]
		public UnityEvent onRespawn = new UnityEvent();

		public bool IsMoving
		{
			get
			{
				return targetRigidbody.bodyType == RigidbodyType2D.Dynamic;
			}
		}
		private void Reset()
		{
			pivotLayer = LayerMask.NameToLayer("Pivot");
		}

		private void Awake()
		{
			targetRigidbody = GetComponent<Rigidbody2D>();
			particleSystems = GetComponentsInChildren<ParticleSystem>();
		}

		/// <summary>
		/// Clamp the position of the 
		/// </summary>
		private void LateUpdate()
		{
			if (currentPivot)
			{
				Vector3 direction = currentPivot.position - transform.position;
				direction.z = 0;
				if (direction.magnitude > maxDistance)
				{
					direction = Vector3.ClampMagnitude(direction, maxDistance);
					currentPivot.position = direction + transform.position;
				}
			}
		}
		private void OnTriggerEnter2D(Collider2D collider)
		{
			if (collider.gameObject.layer == pivotLayer)
			{
				var fireplace = collider.GetComponent<Fireplace>();
				if (fireplace == currentFireplace)
				{
					return;
				}
				else
				{
					AssignNewPivot(fireplace);
				}

			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			Debug.Log(collision);
			if (((1 << collision.gameObject.layer) & killerMask) != 0)
			{

				ResetPivot();
			}
		}

		private void AssignNewPivot(Fireplace fireplace)
		{
			if (fireplace && fireplace.pivot)
			{

				currentFireplace = fireplace;
				currentPivot = fireplace.pivot.transform;
				targetRigidbody.bodyType = RigidbodyType2D.Kinematic;
				targetRigidbody.velocity = Vector2.zero;
				targetRigidbody.angularVelocity = 0;
				transform.position = currentPivot.position;
			}
		}
		/// <summary>
		/// Called when killed or for testing
		/// </summary>
		[LUT.Button]
		public void ResetPivot()
		{
			foreach (var particle in particleSystems)
			{
				var emission = particle.emission;
				emission.enabled = false;
			}
			lastFireplace.RestartPositionPivot();
			AssignNewPivot(lastFireplace);
			onRespawn.Invoke();
			StartCoroutine(WaitForProjectileReturn());
		}

		private IEnumerator WaitForProjectileReturn()
		{
			yield return null;
			foreach (var particle in particleSystems)
			{
				var emission = particle.emission;
				emission.enabled = true;
			}
		}


		[LUT.Button]
		public void Shoot()
		{
			if (!currentPivot)
			{
				return;
			}
			Vector3 direction = transform.position - currentPivot.position;
			direction.z = 0;
			float magnitude = direction.magnitude;
			if (magnitude < minDistance)
			{
				// goes back to the pivot
				currentFireplace.RestartPositionPivot();
				currentPivot.position = transform.position;
			}
			else
			{
				direction /= magnitude;
				float forcePercentage = (magnitude) / maxDistance;
				float force = forcePercentage;
				if (mapDistanceToForce)
				{
					force *= (maxForce - minForce);
					force += minForce;
				}
				else
				{
					force *= forceMultiplier;
				}
				Debug.Log(force);
				currentFireplace?.RestartPositionPivot();
				lastFireplace = currentFireplace;
				currentPivot = null;
				targetRigidbody.bodyType = RigidbodyType2D.Dynamic;
				targetRigidbody.AddForce(direction * force, forceMode2D);
			}
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (!currentPivot)
			{
				return;
			}

			Vector3 direction = transform.position - currentPivot.position;
			direction.z = 0;

			if (direction.magnitude < minDistance)
			{
				Gizmos.color = Color.cyan;
			}
			else if (direction.magnitude > maxDistance)
			{
				Gizmos.color = Color.blue;
			}
			else
			{
				Gizmos.color = Color.red;
			}
			direction = Vector3.ClampMagnitude(direction, maxDistance);
			Gizmos.DrawLine(transform.position, transform.position + direction);
		}

		private void OnDrawGizmosSelected()
		{
			if (!transform)
			{
				return;
			}

			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(transform.position, minDistance);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, maxDistance);



		}
#endif
	}
}
