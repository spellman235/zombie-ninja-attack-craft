using UnityEngine;
using System.Collections;

public class Trap_Snare : PressurePlate
{

     public float duration;


     public BasicFunctions layer1;
     private BasicFunctions l1;

     // Use this for initialization
     void Start()
     {
          player = FindObjectOfType<Player>();
     }

     // Update is called once per frame
     void Update()
     {

     }

     public override void activate()
     {
          GetComponent<Renderer>().enabled = false;
          l1 = Instantiate(layer1, transform.position, transform.rotation) as BasicFunctions;
          player.setSnare(duration);
          l1.FollowPlayer();
          l1.Death(duration);
          Destroy(gameObject);
     }

     
}