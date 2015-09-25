using UnityEngine;
using System.Collections;

public class Trap_Blackout : PressurePlate {

     public GameObject layer1, layer2, layer3, layer4, layer5;

	// Use this for initialization
	void Start () {
          player = FindObjectOfType<Player>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

     public override void activate()
     {
         //create layers
     }
}
