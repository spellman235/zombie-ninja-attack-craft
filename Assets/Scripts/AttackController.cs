﻿using UnityEngine;
using System.Collections;

public class AttackController : MonoBehaviour
{

     Animator animator;
     PlayerMoveController moveController;
     public BoxCollider2D attackCollider;

     public bool isAttacking;
     private bool alreadyAttacked;
     private bool canShoot = true;
     private Vector2 playerPosition;

     public Projectile PlayerArrow;
     public BombScript PlayerBomb;
     public float TimeBetweenShots;
     public int Ammo;
     public int Grenades;

     // Use this for initialization
     void Start()
     {
          isAttacking = false;
          animator = GetComponent<Animator>(); ;
          moveController = GetComponent<PlayerMoveController>();
          attackCollider = FindObjectOfType<ColliderInteractions>().GetComponent<BoxCollider2D>();
          attackCollider.enabled = false;
     }

     public void SetPlayerProjectileAmmo(int amount)
     {
          Ammo += amount;
     }

     public void SetPlayerGrenadeAmmo(int amount)
     {
          Grenades += amount;
     }

     // Update is called once per frame
     void Update()
     {
          playerPosition = moveController.transform.position;

          //Play attacking animations
          if (isAttacking)
          {
               moveController.canDash = false;
               animator.SetBool("IsAttacking", true);
          }
          else
          {
               animator.SetBool("IsAttacking", false);
          }

          //Move and rotate the collider relative to where the player is facing
          if (moveController.facing.x > 0)
          {
               attackCollider.transform.position = new Vector2(playerPosition.x + 0.25f, playerPosition.y - 0.10f);
               attackCollider.transform.localEulerAngles = new Vector3(0, 0, 90);
          }
          else if (moveController.facing.x < 0)
          {
               attackCollider.transform.position = new Vector2(playerPosition.x - 0.25f, playerPosition.y - 0.10f);
               attackCollider.transform.localEulerAngles = new Vector3(0, 0, 90);
          }
          else if (moveController.facing.y > 0)
          {
               attackCollider.transform.position = new Vector2(playerPosition.x, playerPosition.y + 0.15f);
               attackCollider.transform.localEulerAngles = new Vector3(0, 0, 0);
          }
          else if (moveController.facing.y < 0)
          {
               attackCollider.transform.position = new Vector2(playerPosition.x, playerPosition.y - 0.25f);
               attackCollider.transform.localEulerAngles = new Vector3(0, 0, 0);
          }

          //Debug.Log(alreadyAttacked);
     }

     public void Attack()
     {
          if (CanAttack())
          {
               GameManager.Notifications.PostNotification(this, "PlayerWhipAttack");
               //Set attack flags so it doesnt interfere with other animations
               isAttacking = true;
               moveController.isDashing = false;
               moveController.canDash = false;

               //Activate the attack collider so whatever was in the collider gets hurt. This collider has a DealDamageToEnemy script
               attackCollider.GetComponent<Collider2D>().enabled = true;
          }
     }

     public void ShootProjectile(Projectile currentProjectile)
     {
          if (Ammo > 0 && canShoot == true)
          {
               GameManager.Notifications.PostNotification(this, "PlayerProjectileAttack");
               Ammo--;
               //Set Attack Flags
               isAttacking = true;
               moveController.isDashing = false;
               moveController.canDash = false;
               canShoot = false;


               //Instantiate an arrow depending on which direction the player is facing
               if (moveController.facing.x > 0)
               {
                    Projectile projectile = Instantiate(currentProjectile, new Vector2(transform.position.x + 0.25f, transform.position.y), transform.rotation) as Projectile;
                    projectile.Shoot(0, new Vector2(1, 0));
               }
               else if (moveController.facing.x < 0)
               {
                    Projectile projectile = Instantiate(currentProjectile, new Vector2(transform.position.x - 0.25f, transform.position.y), transform.rotation) as Projectile;
                    projectile.Shoot(180, new Vector2(-1, 0));
               }
               else if (moveController.facing.y > 0)
               {
                    Projectile projectile = Instantiate(currentProjectile, new Vector2(transform.position.x, transform.position.y + 0.25f), transform.rotation) as Projectile;
                    projectile.Shoot(90, new Vector2(0, 1));
               }
               else if (moveController.facing.y < 0)
               {
                    Projectile projectile = Instantiate(currentProjectile, new Vector2(transform.position.x, transform.position.y - 0.25f), transform.rotation) as Projectile;
                    projectile.Shoot(-90, new Vector2(0, -1));
               }
               StartCoroutine(ResetCanShoot());
               FinishedAttacking();
          }
     }

     public void ThrowBomb(int damage = 1)
     {
          if(Grenades > 0)
          {
               Grenades--;
               isAttacking = true;
               moveController.isDashing = false;
               moveController.canDash = false;

               BombScript Bomb = Instantiate(PlayerBomb, new Vector2(transform.position.x, transform.position.y), Quaternion.identity) as BombScript;


          }
          FinishedAttacking();
     }

     public void FinishedAttacking()
     {
          //Reset variables
          isAttacking = false;
          alreadyAttacked = false;
          moveController.canDash = true;
          attackCollider.GetComponent<Collider2D>().enabled = false;
     }

     public bool CanAttack()
     {
          //Check if the player is allowed to attack
          if (isAttacking || moveController.isDashing)
          {
               return false;
          }
          else
          {
               return true;
          }
     }

     private IEnumerator ResetCanShoot()
     {
          yield return new WaitForSeconds(TimeBetweenShots);
          canShoot = true;
     }
}

