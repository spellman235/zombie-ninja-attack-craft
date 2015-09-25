using UnityEngine;
using System.Collections;

public class PressurePlate : MonoBehaviour {

     public Player player;

	// Use this for initialization
	void Start () {
          player = FindObjectOfType<Player>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

     public virtual void activate()
     {

     }
}
