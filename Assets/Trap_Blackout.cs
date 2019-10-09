using UnityEngine;
using System.Collections;

public class Trap_Blackout : PressurePlate {

     public float duration;

     public BasicFunctions layer1, layer2, layer3, layer4, layer5;
     private BasicFunctions l1, l2, l3, l4, l5;

	// Use this for initialization
	void Start () {
          player = FindObjectOfType<Player>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

     public override void activate()
     {
          GetComponent<Renderer>().enabled = false;
          l1 = Instantiate(layer1, transform.position, transform.rotation) as BasicFunctions;
          l2 = Instantiate(layer2, transform.position, transform.rotation) as BasicFunctions;
          l3 = Instantiate(layer3, transform.position, transform.rotation) as BasicFunctions;
          l4 = Instantiate(layer4, transform.position, transform.rotation) as BasicFunctions;
          l5 = Instantiate(layer5, transform.position, transform.rotation) as BasicFunctions;

          l1.FollowPlayer();
          l2.FollowPlayer();
          l3.FollowPlayer();
          l4.FollowPlayer();
          l5.FollowPlayer();

          l1.Death(duration);
          l2.Death(duration);
          l3.Death(duration);
          l4.Death(duration);
          l5.Death(duration);
          Destroy(gameObject);
     }
}
