using UnityEngine;

public class HatGuyDiamond : MonoBehaviour {

	void OnTriggerEnter(Collider collider) {
		if (collider.tag == "Player")
			Destroy(gameObject);
	}
	
}
