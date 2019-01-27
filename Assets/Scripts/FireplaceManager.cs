using UnityEngine;

namespace TR
{

	public class FireplaceManager : MonoBehaviour
	{

		[FMODUnity.EventRef]
		public string musicEvent;

		public FireplaceManager Instance
		{
			private set;
			get;
		}

		private void Awake()
		{
			if (!Instance)
			{
				Instance = this;
			}
		}

		public Fireplace lastFireplace;
		public Fireplace currentFireplace;


		public void ChangeFireplace(Fireplace current)
		{

		}
	}
}
