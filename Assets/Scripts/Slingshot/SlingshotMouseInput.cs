﻿using LUT;
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
		[SerializeField]
		private Collider2D target;

		private Fireplace targetFireplace;

		[AutoFind(typeof(SlingshotProjectileBehaviour), true)]
		[SerializeField]
		private SlingshotProjectileBehaviour projectileBehaviour;
		private Vector2 lastPoint = Vector2.zero;

		public bool IsDragging
		{
			get
			{
				return target;
			}
		}
		private void Reset()
		{
			camera = FindObjectOfType<Camera>();
		}

		private void Awake()
		{
			if (!camera)
			{
				camera = FindObjectOfType<Camera>();
			}
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
			else if (Input.GetMouseButton(0))
			{
				MouseUpdate();
			}
		}

		private void MouseDown()
		{
			if (camera != null)
			{
				Vector2 point = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -camera.transform.position.z));
				target = Physics2D.OverlapPoint(point, LayerMask);
				targetFireplace = target?.GetComponent<Fireplace>();

				lastPoint = point;
			}
		}
		private void MouseUpdate()
		{
			if (!target || !camera)
			{
				return;
			}

			if (projectileBehaviour)
			{
				if (projectileBehaviour.IsMoving)
				{
					return;
				}
			}

			Vector2 point = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -camera.transform.position.z));
			target.transform.position += (Vector3)(point - lastPoint);
			lastPoint = point;

		}


		private void MouseUp()
		{
			projectileBehaviour?.Shoot();
			target = null;
		}

	}
}
