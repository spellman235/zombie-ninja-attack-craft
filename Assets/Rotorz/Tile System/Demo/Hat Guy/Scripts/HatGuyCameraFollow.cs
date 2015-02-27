using UnityEngine;

public class HatGuyCameraFollow : MonoBehaviour {
	
	// Transform of target object, defaults to player
	public Transform target;
	// Time in seconds for smoothing
	public float smoothTime = 0.5f;
	// Distance from target
	public float distance = 5.0f;
	
	// Local copy of transform to reduce component lookups
	Transform _transform;
	// Velocity of camera smoothing
	Vector3 _smoothVelocity;
	
	void Start() {
		_transform = transform;
		
		if (target == null) {
			// Point at player by default
			GameObject player = GameObject.FindWithTag("Player");
			if (player != null)
				target = player.transform;
		}
	}
	
	void Update() {
		if (target != null) {
			// Point camera towards target
			Vector3 targetPosition = target.position;
			targetPosition.z -= distance;
			
			_transform.position = Vector3.SmoothDamp(_transform.position, targetPosition, ref _smoothVelocity, smoothTime);
		}
	}
}
