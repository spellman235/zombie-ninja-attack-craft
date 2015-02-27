using UnityEngine;

public class HatGuyTouchButton : MonoBehaviour {
	
	[HideInInspector]
	public bool isButtonDown = false;			// Indicates if button was just pressed down
	[HideInInspector]
	public bool isPressed = false;				// Indicates if button is pressed
	
	void Update() {
		// Assume button is not pressed and then attempt to proove otherwise
		isButtonDown = false;
		isPressed = false;
		
		for (int i = 0; i < Input.touchCount; ++i) {
			Touch touch = Input.GetTouch(i);
			
			// Button represents second half of screen
			if (touch.position.x > (float)Screen.width / 2.0f) {
				isButtonDown = touch.phase == TouchPhase.Began;
				isPressed = true;
				break;
			}
		}
	}
	
}
