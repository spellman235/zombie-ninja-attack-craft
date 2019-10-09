using UnityEngine;
using System.Collections;

public class PressurePlate : MonoBehaviour {

     public Player player;
     public bool activated;

	// Use this for initialization
	void Start () {
          player = FindObjectOfType<Player>();
          activated = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

     public virtual void activate()
     {

     }
}
