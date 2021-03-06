﻿using UnityEngine;
using UnityEngine.Events;

namespace TR
{
	public class Fireplace : MonoBehaviour
	{
		public enum Type
		{
			Cozy,
			Cold
		}
		public Transform pivot;

		public Vector3 startPosition;

		public UnityEventBool onActive;

		public Type type = Type.Cozy;

		private void Awake()
		{
			startPosition = pivot.position;
			SetActive(false);
		}

		public void RestartPositionPivot()
		{
			pivot.transform.position = startPosition;
		}


		public void SetActive(bool b)
		{
			onActive.Invoke(b);
		}
	}
}
