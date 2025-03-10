﻿using UnityEngine;
using System.Collections;

public class DealDamageToPlayer : MonoBehaviour
{

     public int damageAmount;

     public void OnCollisionStay2D(Collision2D other)
     {
          //Check for player collision
          if (other.gameObject.tag == "Player")
          {
               //Find components necessary to take damage and knockback
               GameObject playerObject = other.gameObject;
               Player player = playerObject.GetComponent<Player>();
               Health playerHealth = playerObject.GetComponent<Health>();

               //Take damage if the player isnt already currently invincible
               if (!player.isInvincible)
               {
                    //Deal damage, knockback, set the invinicility flag
                    playerHealth.CalculateKnockback(other, transform.position);
                    playerHealth.TakeDamage(damageAmount);
                    player.isInvincible = true;
               }

               //Destroy gameobject if its a projectile
               if (GetComponent<Projectile>() || GetComponent<Homer>())
               {
                    Destroy(gameObject);
               }


          }
     }
     public void OnTriggerStay2D(Collider2D other)
     {
          //Check for player collision
          if (other.gameObject.tag == "Player")
          {
               //Find components necessary to take damage and knockback
               GameObject playerObject = other.gameObject;
               Player player = playerObject.GetComponent<Player>();
               Health playerHealth = playerObject.GetComponent<Health>();


               //Take damage if the player isnt already currently invincible
               if (!player.isInvincible)
               {
                    if (GetComponent<Projectile>())
                    {
                         player.setStun(GetComponent<Projectile>().stun);
                    }
                    //Deal damage, knockback, set the invinicility flag
                    playerHealth.CalculateKnockback(other, transform.position);
                    playerHealth.TakeDamage(damageAmount);
                    player.isInvincible = true;

               }

               else if(GetComponent<ElectricWall>())
               {
                    playerHealth.CalculateKnockback(other, transform.position);
               }

               //Destroy gameobject if its a projectile
               if (GetComponent<Projectile>() || GetComponent<Homer>())
               {
                    Destroy(gameObject);

               }
          }

          //   else if (CompareTag("snakeBall"))
          //     {
          //       if (other.CompareTag("snakeBall"))
          //      {
          //          gameObject.GetComponent<ExplosionCheck>().explode();
          //      }
          //  }

     }
}
