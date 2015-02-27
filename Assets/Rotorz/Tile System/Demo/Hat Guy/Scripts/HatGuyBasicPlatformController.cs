using UnityEngine;

public class HatGuyBasicPlatformController : MonoBehaviour {
	
	// Virtual joystick for movement
	public HatGuyJoystick virtualJoystick;
	// Virtual jump button
	public HatGuyTouchButton virtualButton;
	
	// Walking speed
	public float walkSpeed = 3.5f;
	// Minimum speed to apply when walking
	public float minimumWalkSpeed = 1.5f;
	// Force to apply upon jumping
	public float jumpForce = 5.9f;
	// Speed to turn from side to side
	public float turnSpeed = 3.5f;
	
	// Animation to use for idle
	public AnimationClip idleAnimation;
	// Speed scale of idle animation
	public float idleAnimationSpeed = 1.0f;
	// Animation to use for walking
	public AnimationClip walkAnimation;
	// Speed scale of walk animation
	public float walkAnimationSpeed = 1.0f;
	
	// Indicates if player is facing right
	bool _facingRight = true;
	// Indicates if player is turning
	bool _turning = false;
	
	private Quaternion _leftRotation = Quaternion.Euler(0.0f, 270.0f, 0.0f);
	private Quaternion _rightRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
	private float _rotationAmount;
	
	// Velocity from previous frame
	private float _lastVelocity;
	
	// Local copy of transform, rigidbody and animation to reduce component lookups
	Transform _transform;
	Rigidbody _rigidbody;
	Animation _animation;
	
	void Start() {
		_transform = transform;
		_rigidbody = rigidbody;
		_animation = animation;
		
		if (idleAnimation != null)
			_animation[ idleAnimation.name ].speed = idleAnimationSpeed;
		if (walkAnimation != null)
			_animation[ walkAnimation.name ].speed = walkAnimationSpeed;
		
		// Automatically rotate player to initial direction
		if (_facingRight) {
			_transform.rotation = _rightRotation;
			_rotationAmount = +1.0f;
		}
		else {
			_transform.rotation = _leftRotation;
			_rotationAmount = -1.0f;
		}
		
		GameObject go = GameObject.Find("Virtual Joystick");
		if (go != null) {
			virtualJoystick = go.GetComponent<HatGuyJoystick>();
			virtualButton = go.GetComponent<HatGuyTouchButton>();
		}
		
#if !(UNITY_IPHONE || UNITY_ANDROID)
		// Virtual joystick not needed for non-touch devices!
		if (virtualJoystick != null)
			Destroy(virtualJoystick.gameObject);
#endif
	}
	
	void Update() {
		Vector3 eulerAngles = _transform.eulerAngles;
		
		// Velocity will be zero unless proven otherwise
		float velocity = 0.0f;
		
		float horizontal = Input.GetAxis("Horizontal");
#if (UNITY_IPHONE || UNITY_ANDROID)
		if (virtualJoystick != null)
			horizontal += virtualJoystick.position.x;
#endif
		
		if (horizontal != 0.0f) {
			// Does player need to turn around?
			if (horizontal > 0.0f && eulerAngles.y != 90.0f) {
				// Begin turning if not facing right direction
				if (!_facingRight) {
					_turning = true;
					_facingRight = true;
				}
			}
			else if (horizontal < 0.0f && eulerAngles.y != -90.0f) {
				// Begin turning if not facing right direction
				if (_facingRight) {
					_turning = true;
					_facingRight = false;
				}
			}
			
			// Only attempt to move player if there is nothing in the way!
			if (!Physics.Raycast(_transform.position, _transform.forward, 0.4f)) {
				// Calculate movement velocity
				velocity = Mathf.Max(minimumWalkSpeed, Mathf.Abs(horizontal) * walkSpeed) * Time.deltaTime;
				if (horizontal < 0.0f)
					velocity = -velocity;
				
				// Move player forwards
				_transform.Translate(velocity, 0.0f, 0.0f, Space.World);
			}
		}
		
		if (_turning) {
			float t, rotateSpeed = turnSpeed * Time.deltaTime;
			
			// Perform smooth rotation
			if (_facingRight) {
				// Rotate from left to right
				t = Mathf.Min(1.0f, (_rotationAmount + 1.0f) / 2.0f + rotateSpeed);
				_rotationAmount = t * 2.0f - 1.0f;
				_transform.rotation = Quaternion.Lerp(_leftRotation, _rightRotation, t);
			}
			else {
				// Rotate from right to left
				t = Mathf.Min(1.0f, (2.0f - (_rotationAmount + 1.0f)) / 2.0f + rotateSpeed);
				_rotationAmount = (2.0f - (t * 2.0f)) - 1.0f;
				_transform.rotation = Quaternion.Lerp(_rightRotation, _leftRotation, t);
			}
			
			if (t == 1.0f)
				_turning = false;
		}
		
#if (UNITY_IPHONE || UNITY_ANDROID)
		bool jumpButtonDown = virtualButton.isButtonDown;
#else
		bool jumpButtonDown = Input.GetButtonDown("Fire1");
#endif
		
		// Jump?
		if (jumpButtonDown) {
			// Can only jump if is touching ground!
			if (Physics.Raycast(_transform.position, Vector3.down, 0.5f)) {
				// Apply force to rigidbody
				_rigidbody.AddForce(0.0f, jumpForce, 0.0f, ForceMode.VelocityChange);
			}
		}
		
		// If player has just began walking, start animation
		if (_lastVelocity == 0.0f && velocity != 0.0f) {
			// Smooth transition from idle animation
			_animation.CrossFade(walkAnimation.name, 0.2f);
		}
		// Use idle animation if just stopped walking
		else if (_lastVelocity != 0.0f && velocity == 0.0f) {
			_animation.CrossFade(idleAnimation.name, 0.2f);
		}
		
		_lastVelocity = velocity;
	}
}
