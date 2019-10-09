using UnityEngine;
using System.Collections;

public class TrapCollision : MonoBehaviour
{
     private PressurePlate trap;

     public void OnTriggerExit2D(Collider2D other)
     {
          
          if (other.gameObject.tag == "Player")
          {
               trap = GetComponent<PressurePlate>();
               //Make trap, pressureplate is a trap
               trap.activate();
          }
     }

}