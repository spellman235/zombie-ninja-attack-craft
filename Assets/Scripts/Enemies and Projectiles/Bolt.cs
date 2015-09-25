using UnityEngine;
using System.Collections;
using CreativeSpore.RpgMapEditor;


public class Bolt : Projectile
{



     public void OnTriggerEnter2D(Collider2D other)
     {
          //if not a boss (really just the electric boss has this) then deal damage + knockback
          //Only reason it doesn't just have a normal box collider
          //and instead has an isTrigger is because of the electric guy boss fight
          if (other.gameObject.CompareTag("Player"))
          {
               Player plr = other.GetComponent<Player>();
               plr.setStun(getStun());
               Destroy(gameObject);
          }
     }
     public void OnTriggerStay2D(Collider2D other)
     {
          //if not a boss (really just the electric boss has this) then deal damage + knockback
          //Only reason it doesn't just have a normal box collider
          //and instead has an isTrigger is because of the electric guy boss fight
          if (other.gameObject.CompareTag("Player"))
          {
               Player plr = other.GetComponent<Player>();
               plr.setStun(getStun());
               Destroy(gameObject);
          }
     }



     public void OnCollisionStay2D(Collision2D other)
     {
          //if not a boss (really just the electric boss has this) then deal damage + knockback
          //Only reason it doesn't just have a normal box collider
          //and instead has an isTrigger is because of the electric guy boss fight
          if (other.gameObject.CompareTag("Player"))
          {
               Player plr = other.gameObject.GetComponent<Player>();
               plr.setStun(getStun());
               Destroy(gameObject);
          }
     }
}



