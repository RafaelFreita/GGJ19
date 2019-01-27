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
		private Transform currentPivot = null;
		private Fireplace lastFireplace = null;
		private Fireplace currentFireplace = null;

		private Rigidbody2D targetRigidbody = null;


		[Header("Life")]
		[SerializeField]
		private LayerMask killerMask = new LayerMask();
		private ParticleSystem[] particleSystems = new ParticleSystem[0];
		private bool isDeathCountdownOn = false;
		private Timer timer;
		[SerializeField]
		private Timer.Duration duration = new Timer.Duration(3);

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

		[Header("Music")]
		[FMODUnity.EventRef]
		public string musicEvent;
		private FMOD.Studio.EventInstance musicInstance;
		[SerializeField]
		[Unit("s")]
		private float secondsToTransitionBetweenMusics = 1;
		public FMOD.Studio.ParameterInstance cozynessParameter;
		private float currentCozyness = 1;
		[FMODUnity.EventRef]
		public string rainEvent;
		private FMOD.Studio.EventInstance rainInstance;
		public FMOD.Studio.ParameterInstance coveredParameter;

		[Header("SFX")]

		[FMODUnity.EventRef]
		public string sfxFadingEvent;
		private FMOD.Studio.EventInstance sfxFadingInstance;
		[FMODUnity.EventRef]
		public string sfxThrowingEvent;
		private FMOD.Studio.EventInstance sfxThrowingInstance;
		[FMODUnity.EventRef]
		public string sfxEnterFireplaceEvent;
		private FMOD.Studio.EventInstance sfxEnterFireplaceInstance;
		[FMODUnity.EventRef]
		public string sfxHittingEvent;
		private FMOD.Studio.EventInstance sfxHittingInstance;


		[Header("Events")]
		public UnityEvent onRespawn = new UnityEvent();

		private Vector3 scale = Vector3.zero;

		public Transform CurrentPivot { get => currentPivot; }

		private void Reset()
		{
			pivotLayer = LayerMask.NameToLayer("Pivot");
		}

		private void Awake()
		{
			scale = transform.localScale;
			targetRigidbody = GetComponent<Rigidbody2D>();
			particleSystems = GetComponentsInChildren<ParticleSystem>();
			GetSounds();
		}

		private void GetSounds()
		{
			musicInstance = FMODUnity.RuntimeManager.CreateInstance(musicEvent);
			musicInstance.getParameter("cozy", out cozynessParameter);
			cozynessParameter.setValue(currentCozyness);
			musicInstance.start();

			rainInstance = FMODUnity.RuntimeManager.CreateInstance(rainEvent);
			rainInstance.getParameter("covered", out coveredParameter);
			coveredParameter.setValue(currentCozyness);
			rainInstance.start();


			sfxFadingInstance = FMODUnity.RuntimeManager.CreateInstance(sfxFadingEvent);
			sfxThrowingInstance = FMODUnity.RuntimeManager.CreateInstance(sfxThrowingEvent);
			sfxEnterFireplaceInstance = FMODUnity.RuntimeManager.CreateInstance(sfxEnterFireplaceEvent);
			sfxHittingInstance = FMODUnity.RuntimeManager.CreateInstance(sfxHittingEvent);

		}
		public void Update()
		{
			if (timer.isRunning && !currentPivot)
			{
				if (timer.Update(duration))
				{
					OnDeath();
				}
				transform.localScale = scale * (1 - timer.GetProgress(duration));
			}

			if (currentFireplace)
			{
				if (currentFireplace.type == Fireplace.Type.Cold)
				{
					currentCozyness -= secondsToTransitionBetweenMusics * Time.deltaTime;
				}
				else
				{
					currentCozyness += secondsToTransitionBetweenMusics * Time.deltaTime;
				}
				currentCozyness = Mathf.Clamp01(currentCozyness);
				coveredParameter.setValue(currentCozyness);
				cozynessParameter.setValue(currentCozyness);
			}
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

		private Collider2D lastCollider = null;
		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (timer.isRunning)
			{
				FMOD.Studio.PLAYBACK_STATE playbackState;
				sfxHittingInstance.getPlaybackState(out playbackState);
				if (collision.collider != lastCollider || playbackState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
				{
					sfxHittingInstance.start();
				}
			}
			else
			{
				sfxHittingInstance.start();
			}
			Debug.Log(collision);
			if (((1 << collision.gameObject.layer) & killerMask) != 0)
			{
				if (!timer.isRunning)
				{
					sfxFadingInstance.start();
					timer.Start();
				}
			}
			lastCollider = collision.collider;
		}


		private void OnDestroy()
		{
			musicInstance.release();
			sfxFadingInstance.release();
			sfxThrowingInstance.release();
			sfxEnterFireplaceInstance.release();
			sfxHittingInstance.release();
		}

		public void OnDeath()
		{
			timer.Stop();
			ResetPivot();
		}

		public bool IsMoving
		{
			get
			{
				return targetRigidbody.bodyType == RigidbodyType2D.Dynamic;
			}
		}


		private void AssignNewPivot(Fireplace fireplace)
		{
			if (fireplace && fireplace.pivot)
			{
				currentFireplace = fireplace;
				lastFireplace?.SetActive(false);
				currentFireplace.SetActive(true);
				currentPivot = fireplace.pivot.transform;

				targetRigidbody.bodyType = RigidbodyType2D.Kinematic;
				targetRigidbody.velocity = Vector2.zero;
				targetRigidbody.angularVelocity = 0;
				transform.position = currentPivot.position;
				transform.rotation = Quaternion.identity;

				timer.Stop();
				StartCoroutine(WaitForProjectileReturn());

				sfxEnterFireplaceInstance.start();
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
		}

		private IEnumerator WaitForProjectileReturn()
		{
			yield return null;
			foreach (var particle in particleSystems)
			{
				var emission = particle.emission;
				emission.enabled = true;

			}
			transform.localScale = scale;
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
				sfxThrowingInstance.start();

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
