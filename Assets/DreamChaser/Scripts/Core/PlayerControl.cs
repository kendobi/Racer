using UnityEngine;
using System.Collections;

/// <summary>
/// The player controller
/// </summary>
public class PlayerControl : MonoBehaviour 
{
	public enum MobileInputMode
	{
		Touch,
		Accelerometer
	}

	[Tooltip("The starting speed of the vehicle - in m/s")]
	public float speed = 80.0f;
	[Tooltip("The steering speed of the vehicle")]
	public float steerSpeed = 8.0f;
	[Tooltip("The amount the vehicle tilts when turning - in degrees")]
	public float tiltAngle = 30.0f;
	[Tooltip("The speed at which the vehicle turns")]
	public float tiltSpeed = 10.0f;

	public float slide = 20.0f;

	[Tooltip("The Collision radius of the vehicle")]
	public float collisionRadius = 0.5f;
	[Tooltip("A prefab to instantiate when the vehicle crashes")]
	public GameObject collisionParticles;
	[Tooltip("The default prefab to use for the vehicle model")]
	public GameObject defaultShipPrefab;
	private Animator animator;

	[Header("Mobile Options")]
	[Tooltip("What input method to use on mobile devices")]
	public MobileInputMode inputMode = MobileInputMode.Accelerometer;
	[Tooltip("How sensitive the turning is to changes in the device orientation")]
	[Range(1.0f, 10.0f)]
	public float accelerometerSensitivity = 4.0f;

	[Tooltip("Make the player invincible - useful for testing")]
	public bool isInvincible = false;

	private float steer = 0.0f;				// current steering value
	private float tilt = 0.0f;				// current tilt angle
	private float speedMultiplier = 1.0f;	// speed multiplier for speedboosts/slow motion effects
	private bool crashed = false;			// have we crashed?
	private float startingSpeed;			// initial speed at the start of the game

	private GameObject shipModel;			// our vehicle model

	private int powerupLayer;				// layer that powerup collectables are on

	/// <summary>
	/// Get the current steering value of the vehicle (between -1 and 1)
	/// </summary>
	/// <value>The current steering value</value>
	public float Steer		{ get { return steer; } }

	/// <summary>
	/// Get the current speed of the vehicle in m/s
	/// </summary>
	/// <value>The current speed</value>
	public float Speed		{ get { return crashed ? 0.0f : speed * speedMultiplier; } }

	/// <summary>
	/// Get or set the current speed multiplier.
	/// </summary>
	/// <value>The current speed multiplier</value>
	public float SpeedMultiplier	
	{
		get { return speedMultiplier; }
		set { speedMultiplier = value; }
	}

	void Awake()
	{
		powerupLayer = LayerMask.NameToLayer("Powerup");


		// remember our starting speed.
		startingSpeed = speed;

		// create the default vehicle model
		SetShipModel(defaultShipPrefab);

		Reset();
	}

	public void Reset()
	{
		steer = 0.0f;
		tilt = 0.0f;
		speedMultiplier = 1.0f;
		crashed = false;
		speed = startingSpeed;

	}

	public void SetShipModel(GameObject shipPrefab)
	{
		// destroy the current model if it exists
		if(shipModel != null)
			Destroy (shipModel);

		// instantiate a new vehicle model, and parent it to this object
		shipModel = Instantiate(shipPrefab, transform.position, transform.rotation) as GameObject;
		shipModel.transform.parent = transform;
		animator = shipModel.GetComponent<Animator>();
	}

	private float GetSteerInput()
	{
		if(!Application.isMobilePlatform)
		{
			// Get GamePad/Keyboard input.
			return Input.GetAxis("Horizontal");
		}
		else if(inputMode == MobileInputMode.Touch)
		{
			// Touch the left side of the screen to turn left, right side to turn right
			if(Input.touchCount > 0)
				return Input.GetTouch(0).position.x < (0.5f * Screen.width) ? -1.0f : 1.0f;
			else
				return 0.0f;
		}
		else
		{
			// Use the orientation of the device as the steering value
			return accelerometerSensitivity * Input.acceleration.x;
		}
	}

	void Update () 
	{
		// get the steering input
		float steerValue = GetSteerInput();


		// vary the steering speed with the speed multiplier (but only a little, otherwise there's no benefit to slow motion)
		float steerSpd = steerSpeed * (Mathf.Lerp(speedMultiplier, 1.0f, 0.01f));
		// smoothly lerp our current steer value towards the target input value
		steer = Mathf.Lerp (steer, steerSpd * steerValue, tiltSpeed * Time.deltaTime);
		/*
		float steerValAnim = steer/20;

		if (steerValAnim < 0.0f) {
			steerValAnim = Mathf.Abs (steer/20);

		}
		if (steerValAnim > 0.0f) {
			steerValAnim = (0.5f + (steer / 20));
		} 
		print (steer);

		animator.SetFloat ("steerValue", steerValAnim);*/

		// tilt the vehicle as we steer
		float targetTilt = -steerValue * tiltAngle;
		tilt = Mathf.Lerp (tilt, targetTilt, tiltSpeed * Time.deltaTime);
		Vector3 rot = transform.eulerAngles;
		rot.z = tilt;
		transform.eulerAngles = rot;
		print (tilt);

		//move player side to side while turning
		transform.position = new Vector3 ((tilt / slide), 11.0f, 0.0f);



		// check for collisions (use a slightly longer distance than we actually travelled to make sure we don't miss
		// any collisions - if we test the exact distance we've moved then we occasionaly fly straight through obstacles).
		// Note that because it is the scenery that moves, not the player, we can't use concave mesh colliders because they
		// must always be static. Simple Box colliders are usually just fine, we don't need very accurate collision volumes.
		float distMoved = 1.25f * Speed * Time.deltaTime;
		float extraOffset = collisionRadius;
		RaycastHit hit;
		if(Physics.SphereCast(transform.position - extraOffset * Vector3.forward, collisionRadius, Vector3.forward, out hit, distMoved + 2.0f * extraOffset))
		{
			if(hit.collider.gameObject.layer == powerupLayer)
			{
				// inform the powerup than we've collected it so it can do its thing.
				PowerupCollectable powerup = hit.collider.gameObject.GetComponent<PowerupCollectable>();
				if(powerup != null)
					powerup.OnCollected();
			}
			else if(!isInvincible)
			{
				// We've crashed!
				// instantiate our crash particles
				GameObject particles = Instantiate(collisionParticles, transform.position, Quaternion.identity) as GameObject;
				// destroy the particles object after 2 seconds.
				Destroy (particles, 2.0f);
				
				SoundManager.PlaySfx("Explosion");
				
				// come to a complete stop and hide the player model
				crashed = true;
				steer = 0.0f;
				gameObject.SetActive(false);
				
				// inform the game manager of the incident
				GameManager.OnGameOver();
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		// draw a preview of our collision sphere.
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, collisionRadius);
	}
}
