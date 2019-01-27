using LUT;
using System.Collections;
using UnityEngine;

namespace TR
{

	public class LightningAndThunder : MonoBehaviour
	{
		[SerializeField]
		[Range(0, 1)]
		private float probability;

		[SerializeField]
		[AutoFind(typeof(ParticleSystem), true)]
		private ParticleSystem particleSystem;
		[SerializeField]
		[FMODUnity.EventRef]
		public string sfxEvent;
		private FMOD.Studio.EventInstance sfxInstance;
		[SerializeField]
		[AutoFind(typeof(Animation),true)]
		private Animation light;

		private void Awake()
		{
			sfxInstance = FMODUnity.RuntimeManager.CreateInstance(sfxEvent);
			StartCoroutine(UpdateLoop());
		}

		private void OnDestroy()
		{
			sfxInstance.release();
		}


		private IEnumerator UpdateLoop()
		{
			while (true)
			{
				yield return new WaitForSeconds(0.1f);
				if (Random.value < probability)
				{
					FMOD.Studio.PLAYBACK_STATE state;
					sfxInstance.getPlaybackState(out state);
					if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
					{
						Play();
					}
				}
			}

		}

		private void Play()
		{
			sfxInstance.start();
			particleSystem.Play();
			light.Play();
		}
	}
}
