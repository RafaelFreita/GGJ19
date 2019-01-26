using LUT;
using UnityEngine;

namespace TR
{
	public class ArrowSlingshot : MonoBehaviour
	{
		private SlingshotProjectileBehaviour slingshotProjectileBehaviour;
		private SlingshotMouseInput slingshotMouseInput;

		public Transform arrowHead;
		[AutoFind(typeof(LineRenderer),true)]
		public LineRenderer lineRenderer;


		private void Start()
		{
			if (!slingshotProjectileBehaviour)
			{
				slingshotProjectileBehaviour = FindObjectOfType<SlingshotProjectileBehaviour>();
			}
			if (!slingshotMouseInput)
			{
				slingshotMouseInput = FindObjectOfType<SlingshotMouseInput>();
			}
		}

		private void Update()
		{
			if (slingshotMouseInput.IsDragging)
			{
				arrowHead.gameObject.SetActive(true);
				lineRenderer.gameObject.SetActive(true);

				Vector3 handlePos = slingshotProjectileBehaviour.transform.position;
				handlePos.z = 0;
				lineRenderer.SetPosition(0, handlePos);

				Vector3 pivotPos = slingshotProjectileBehaviour.CurrentPivot.position;
				pivotPos.z = 0;

				Vector3 direction = handlePos- pivotPos;

				Vector3 arrowPos = handlePos + direction;
				
				lineRenderer.SetPosition(1, arrowPos);
				arrowHead.transform.position = arrowPos;

				float angle = Vector2.SignedAngle(Vector2.right, direction);
				arrowHead.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
			}
			else
			{
				arrowHead.gameObject.SetActive(false);
				lineRenderer.gameObject.SetActive(false);
			}
		}
	}
}
