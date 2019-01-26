using LUT;
using UnityEngine;

namespace TR
{
	public class SlingshotMouseInput : MonoBehaviour
	{
		[Tooltip("The camera used to calculate the ray")]
		public new Camera camera;

		[Tooltip("This stores the layers we want the raycast/overlap to hit (make sure this GameObject's layer is included!)")]
		public LayerMask LayerMask = Physics.DefaultRaycastLayers;

		[Tooltip("Current target"), InspectorReadOnly]
		public Collider2D target;

        private SlingshotProjectileBehaviour projectBehaviour;
        private Vector2 lastPoint = Vector2.zero;

		private void Reset()
		{
			camera = FindObjectOfType<Camera>();
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				MouseDown();
			}
			else if (Input.GetMouseButtonUp(0))
			{
				MouseUp();
			}
			else  if (Input.GetMouseButton(0))
			{
				MouseUpdate();
			}
		}

		private void MouseDown()
		{
			if (camera != null)
			{
				Vector2 point = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -camera.transform.position.z));
				Debug.Log(point);
				target = Physics2D.OverlapPoint(point, LayerMask);
                projectBehaviour = target?.GetComponent<SlingshotProjectileBehaviour>();
                Debug.Log(target);

				lastPoint = point;
			}
		}
		private void MouseUpdate()
		{
			if (!target || !camera)
			{
				return;
			}

			Vector2 point = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -camera.transform.position.z));
			target.transform.position += (Vector3)(point - lastPoint);
			lastPoint = point;

		}


		private void MouseUp()
		{
			target?.GetComponent<SlingshotProjectileBehaviour>()?.Shoot();
			target = null;
		}

	}
}
