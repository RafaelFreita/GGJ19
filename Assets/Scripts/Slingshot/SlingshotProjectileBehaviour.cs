using BitStrap;
using LUT;
using UnityEngine;

namespace TR
{
	public class SlingshotProjectileBehaviour : MonoBehaviour
	{
		public Transform currentPivot;
		public Transform targetTransform;

		[SerializeField]
		[AutoFind(typeof(Rigidbody2D))]
		private Rigidbody2D targetRigidbody;

		[Header("Drag")]
		[SerializeField]
		[Unit("m")]
		private float minDistance = 0.5f;
		[SerializeField]
		[Unit("m")]
		private float maxDistance = 4;
		[Space]
		[SerializeField]
		[Unit("N")]
		private float minForce = 0.5f;
		[SerializeField]
		[Unit("N")]
		private float maxForce = 4f;

		[Space]
		[SerializeField]
		private ForceMode2D forceMode2D;



		private void Reset()
		{
			targetRigidbody = GetComponent<Rigidbody2D>();
			targetTransform = transform;
		}

		/// <summary>
		/// Clamp the position of the 
		/// </summary>
		private void LateUpdate()
		{
			if (currentPivot)
			{
				Vector3 direction = targetTransform.position - currentPivot.position;
				direction.z = 0;
				if (direction.magnitude > maxDistance)
				{
					direction = Vector3.ClampMagnitude(direction, maxDistance);
					targetTransform.position = direction + currentPivot.position;
				}
			}
		}

		[LUT.Button]
		public void Shoot()
		{
			Vector3 direction = currentPivot.position - targetTransform.position;
			direction.z = 0;
			float magnitude = direction.magnitude;
			if (magnitude < minDistance)
			{
				// goes back to the pivot
				targetTransform.position = currentPivot.position;
			}
			else
			{
				direction /= magnitude;
				float forcePercentage = (magnitude) / maxDistance;
				Debug.Log(forcePercentage);
				float force = forcePercentage * (maxForce - minForce) + minForce;
				Debug.Log(force);

				targetRigidbody.bodyType = RigidbodyType2D.Dynamic;
				targetRigidbody.AddForce(direction * force, forceMode2D);
			}
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (!currentPivot && !targetTransform)
			{
				return;
			}

			Vector3 direction = currentPivot.position - targetTransform.position;
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
#endif
	}
}
