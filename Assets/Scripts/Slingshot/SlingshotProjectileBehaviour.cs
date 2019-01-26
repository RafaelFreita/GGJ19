using BitStrap;
using LUT;
using UnityEngine;

namespace TR
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class SlingshotProjectileBehaviour : MonoBehaviour
	{
		public Transform currentPivot;
		private Transform lastPivot;

		private Rigidbody2D targetRigidbody;


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
		private ForceMode2D forceMode2D;

		[Header("Collision")]
		[SerializeField]
		[LayerSelector]
		private int pivotLayer = 0;

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
			targetRigidbody.isKinematic = true;
		}

		/// <summary>
		/// Clamp the position of the 
		/// </summary>
		private void LateUpdate()
		{
			if (currentPivot)
			{
				Vector3 direction = transform.position - currentPivot.position;
				direction.z = 0;
				if (direction.magnitude > maxDistance)
				{
					direction = Vector3.ClampMagnitude(direction, maxDistance);
					transform.position = direction + currentPivot.position;
				}
			}
		}
		private void OnTriggerEnter2D(Collider2D collider)
		{
			if (collider.gameObject.layer == pivotLayer && currentPivot != collider.transform && lastPivot != collider.transform)
			{
				AssignNewPivot(collider.transform);
			}
		}


		private void AssignNewPivot(Transform newPivot)
		{
			currentPivot = newPivot;
			targetRigidbody.bodyType = RigidbodyType2D.Kinematic;
			targetRigidbody.velocity = Vector2.zero;
			transform.position = currentPivot.position;
		}
		/// <summary>
		/// Called when killed or for testing
		/// </summary>
		[LUT.Button]
		public void ResetPivot()
		{
			AssignNewPivot(lastPivot);
		}

		[LUT.Button]
		public void Shoot()
		{
			if (!currentPivot)
			{
				return;
			}
			Vector3 direction = currentPivot.position - transform.position;
			direction.z = 0;
			float magnitude = direction.magnitude;
			if (magnitude < minDistance)
			{
				// goes back to the pivot
				transform.position = currentPivot.position;
			}
			else
			{
				direction /= magnitude;
				float forcePercentage = (magnitude) / maxDistance;
				float force = forcePercentage;
				if (mapDistanceToForce)
				{
					force *= (maxForce-minForce);
					force += minForce;
				}
				else
				{
					force *= forceMultiplier;
				}

				lastPivot = currentPivot;
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

			Vector3 direction = currentPivot.position - transform.position;
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
			Gizmos.DrawLine(currentPivot.position, currentPivot.position + direction);
		}

		private void OnDrawGizmosSelected()
		{
			if (!currentPivot)
			{
				return;
			}

			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(currentPivot.position, minDistance);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(currentPivot.position, maxDistance);



		}
#endif
	}
}
