//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;

namespace AssemblyCSharp
{
     public class AxeMan : Enemy
     {
          public Axe ax, axeObject;

          private Health health;

          private bool isAgro;

          private Vector2 distance, speed, facing;
          private double temp, idleTime;
          private Vector3 someVec;

          //private Animator animator;


          public void Start()
          {
               //animator = GetComponent<Animator>();

               moveController = GetComponent<EnemyMoveController>();
               health = GetComponent<Health>();
               player = FindObjectOfType<Player>();
               //rigidbody2D.mass = 10;
               distance = new Vector2(0, 0);
               speed = new Vector2(0, 0);
               isAgro = false;

               rnd = new System.Random(Guid.NewGuid().GetHashCode());
               t = 3 + rnd.Next(0, 3000) / 1000f;

               //temp is the number for exponential speed when running away
               temp = 1.0000001;

               facing = new Vector2(0, 0);

               ax = Instantiate(axeObject, transform.position, transform.rotation) as Axe;
               ax.setAxeMan(this);

          }

          public void Update()
          {
               checkInvincibility();
               rnd = new System.Random();
               if (checkStun())
               {
                    stunTimer -= Time.deltaTime;
                    moveController.Move(0, 0);
               }
               else if (player != null)
               {
                    findPos();
                    //basic aggression range formula
                    distance = player.transform.position - transform.position;
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
                         //Axe ax = Instantiate(axeObject, transform.position, transform.rotation) as Axe;
                         float xSp = player.transform.position.x - transform.position.x;
                         float ySp = player.transform.position.y - transform.position.y;
                         //exponential speed
                         /*if (distance.magnitude < 1.5) {
                                 xSp *= -temp;
                                 ySp *= -temp;
                                 temp *= 1.2;
												
                         } else if (distance.magnitude < 2) {
                                 xSp = 0;
                                 ySp = 0;
                                 //temp = 0;
                         }
                         else {
                                 temp = 0.5;
                         }
                         speed = new Vector2 ((float)xSp, (float)ySp);
                         //Debug.Log ("Que es x and y? : " + xSp + " and " + ySp);
                         speed = speed.normalized;*/

                         if (distance.magnitude < 1)
                         {
                              moveController.Move(-xSp / 10f, -ySp / 10f);
                         }
                         else if (distance.magnitude < 1.15)
                         {
                              moveController.Move(0, 0);
                         }
                         else
                         {
                              moveController.Move(xSp / 15f, ySp / 15f);
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
                    //GetComponent<Rigidbody2D> ().velocity = speed;


                    //Debug.Log (t);
                    //GetComponent<Rigidbody2D> ().velocity = speed;
                    //Debug.Log (rigidbody2D.velocity.magnitude);
               }
          }

          public Vector2 getIdle()
          {
               // facing = moveController.getFacing ();
               float thisX = transform.position.x;
               float thisY = transform.position.y;
               if (facing.x == 1)
               {
                    return new Vector2(thisX - 1, thisY - 1);
               }
               else if (facing.x == -1)
               {
                    return new Vector2(thisX + 1, thisY - 1);
               }
               else if (facing.y == 1)
               {
                    return new Vector2(thisX + 1, thisY - 1);
               }
               else if (facing.y == -1)
               {
                    return new Vector2(thisX - 1, thisY + 1);
               }
               return new Vector2(0, 0);
          }

          public bool getAgro()
          {
               return isAgro;
          }

          public float currentHp()
          {
               return health.currentHealth;
          }


     }
}

