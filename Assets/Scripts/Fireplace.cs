using UnityEngine;

namespace TR
{
	public class Fireplace : MonoBehaviour
	{
		public Transform pivot;

		public Vector3 startPosition;

		private void Awake()
		{
			startPosition = pivot.position;
		}

		public void RestartPositionPivot()
		{
			pivot.transform.position = startPosition;
		}
	}
}
