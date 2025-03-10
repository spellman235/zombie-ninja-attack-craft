﻿using System;
using UnityEngine;


     public class BasicEnemy : Enemy
     {
          public BasicAttack LRAttack, UDAttack;
          private BasicAttack attackCollider;
          private AnimationController animationController;
          private Health health;

          [HideInInspector]
          public bool isAgro, canAttack, isAttacking, canChangeDirection;
          private Vector2 attackDirection;
          public Vector2 distance, speed, facing, distanceFromPoint, point, up, down, left, right;
          private double idleTime, attackDelay;
          private Vector3 someVec;

          //private Animator animator;


          public void Start()
          {
               //animator = GetComponent<Animator>();

               moveController = GetComponent<EnemyMoveController>();
               animationController = GetComponent<AnimationController>();
               health = GetComponent<Health>();
               player = FindObjectOfType<Player>();

               //rigidbody2D.mass = 10;
               distance = new Vector2(0, 0);
               speed = new Vector2(0, 0);
               isAgro = false;
               attackDelay = 3;

               rnd = new System.Random(Guid.NewGuid().GetHashCode());
               t = 3 + rnd.Next(0, 3000) / 1000f;

               facing = new Vector2(0, 0);
               point = new Vector2(100, 100);
               distanceFromPoint = new Vector2(100, 100);
               up = new Vector2(0, 0.4f);
               left = new Vector2(-0.4f, 0);
               right = new Vector2(0.4f, 0);
               down = new Vector2(0, -0.4f);
               canAttack = true;

          }

          public void Update() 
          {
               // Debug.Log("LR: " + LRAttack.GetComponent<SpriteRenderer>().bounds.size.x);
               // Debug.Log("UD: " + UDAttack.GetComponent<SpriteRenderer>().bounds.size.x);
               checkInvincibility();
               if (checkStun())
               {
                    stunTimer -= Time.deltaTime;
                    moveController.Move(0, 0);
               }
               else
               {
                    rnd = new System.Random();
                    if (player != null)
                    {
                         distance = player.transform.position - transform.position;
                         distanceFromPoint = distance + up;
                    }

                    if (distanceFromPoint.magnitude > (distance + left).magnitude)
                    {
                         distanceFromPoint = distance + left;
                    }
                    if (distanceFromPoint.magnitude > (distance + right).magnitude)
                    {
                         distanceFromPoint = distance + right;
                    }
                    if (distanceFromPoint.magnitude > (distance + down).magnitude)
                    {
                         distanceFromPoint = distance + down;
                    }
                    float xSp = distanceFromPoint.normalized.x;
                    float ySp = distanceFromPoint.normalized.y;
                    direction = new Vector2(xSp, ySp);
                    if (distance.magnitude <= AgroRange)
                    {
                         isAgro = true;
                         //animator.SetBool("isCharging", true);
                    }
                    if (distance.magnitude > AgroRange)
                    {
                         isAgro = false;
                    }

                    if (isAgro)
                    {
                         if (canAttack)
                         {
                              if(distanceFromPoint.magnitude < 0.07f && animationController.isAttacking == false) {
                                   animationController.isAttacking = true;
                                   moveController.Move(0, 0);
                              }else if(animationController.isAttacking == true)
                              {
                                   moveController.Move(0, 0);
                              }
                              else if (distanceFromPoint.magnitude > 0.25f )
                              {

                                   moveController.Move(direction / 6f);
                                   //distance.normalized?
                              }
                             
                         }
                         else
                         {

                              moveController.Move(direction / 6f);
                         }

                    }
                    else
                    {
                         if (idleTime > 0.4)
                         {
                              someVec = idle(t, rnd);
                              t = someVec.z;
                              idleTime = 0;
                         }
                         moveController.Move(someVec.x, someVec.y);
                    }

                    idleTime += Time.deltaTime;
                    t -= Time.deltaTime;
                    
               }
          }


          public bool getAgro()
          {
               return isAgro;
          }

          public float currentHp()
          {
               return health.currentHealth;
          }

          //Called by attack animation when spear is being thrusted forward
          public void Attack()
          {
               Debug.Log("Started");
               if (Math.Abs(attackDirection.x) > Math.Abs(attackDirection.y))
               {
                    if (attackDirection.x > 0)
                    {
                         attackCollider = Instantiate(LRAttack, transform.position + new Vector3(0.5f, 0, 0), Quaternion.identity) as BasicAttack;
                    }
                    else
                    {
                         attackCollider = Instantiate(LRAttack, transform.position + new Vector3(-0.5f, 0, 0), Quaternion.identity) as BasicAttack;
                    }
                    attackCollider.transform.parent = gameObject.transform;
               }
               else
               {
                    if(attackDirection.y > 0)
                    {
                         attackCollider = Instantiate(UDAttack, transform.position + new Vector3(0, 0.5f, 0), UDAttack.transform.rotation) as BasicAttack;
                    }
                    else
                    {
                         attackCollider = Instantiate(UDAttack, transform.position + new Vector3(0, -0.5f, 0), UDAttack.transform.rotation) as BasicAttack;
                    }

                    attackCollider.transform.parent = gameObject.transform;
               }
               //Destroy(attackCollider.gameObject);
          }

          //Called by attacking animation at end of animatn
          public void DoneAttacking()
          {
               Debug.Log("Finished");
               canAttack = true;
               animationController.isAttacking = false;
               Destroy(attackCollider.gameObject);
          }
          
          public void InititalDirection()
          {
               attackDirection = new Vector2(moveController.facing.x, moveController.facing.y);
               Debug.Log(attackDirection.x + " " + attackDirection.y);
          }

          //Called by Rest animation after animation finishes (Rest animation is idle but in it's own animation, so it can call methods seperately)
          public void DoneResting()
          {
               canAttack = true;
               //animator.setBool(rest,false)
               //sets to walking as it is the default animation.
          }

          public override void onDeath()
          {
               Destroy(attackCollider);
          }
     }
