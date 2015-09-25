using UnityEngine;
using System.Collections;

public class TrapCollision : MonoBehaviour
{
     //private Trap trap;

     public void OnTriggerExit2D(Collider2D other)
     {
          if (other.gameObject.tag == "Player")
          {
               // trap.activate();
          }
     }

}