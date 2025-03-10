﻿using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
     public Player player;
     public float AgroRange;
     private GameObject deathPile;
     public Sprite deathSprite;
     public EnemyMoveController moveController;
     public bool isInvincible, blink, canBlink;
     public float timeSpentInvincible, stunTimer;

     [HideInInspector]
     public float currentX, currentY, playerX, playerY, angle;

     [HideInInspector]
     public Vector2 direction;

     public double t;
     public System.Random rnd;

     void Awake()
     {
          blink = false;
          isInvincible = false;
          canBlink = true;
          timeSpentInvincible = 0;
          GetComponent<Rigidbody2D>().gravityScale = 0;
          deathPile = GetComponent<Health>().deathPilePrefab;
         
     }

     public void checkInvincibility()
     {
          if (isInvincible)
          {
               timeSpentInvincible += Time.deltaTime;

               if (timeSpentInvincible > 0.2f)
               {
                    setStun(0.1f);
               }

               if (timeSpentInvincible <= 0.3f)
               {
                    if (canBlink)
                    {
                         blink = !blink;
                         GetComponent<Renderer>().enabled = blink;
                         GetComponent<SpriteRenderer>().color = Color.red;
                    }
               }

               else
               {
                    isInvincible = false;
                    timeSpentInvincible = 0;
                    GetComponent<Renderer>().enabled = true;
                    GetComponent<SpriteRenderer>().color = new Color(255,255,255,255);
               }
          }
     }

     public Vector3 idle(double someDub, System.Random ran)
     {
          ran = new System.Random(Guid.NewGuid().GetHashCode());
          Vector3 returnVec = new Vector3(0,0,(float)someDub);
          //Debug.Log("I am a " + ToString() + "and my t is " + someDub);
          if (someDub < 1 && someDub > 0)
          {
             returnVec = new Vector3(0, 0, 3);

          }
          else if (someDub < 2 && someDub > 1.3)
          {
               int rand = ran.Next(1, 5);
               if (rand == 1)
               {
                    //speed = new Vector2 (2, 0);
                    returnVec = new Vector3(1/5f, 0, 1.3f);

                    someDub = 1.3;
               }
               else if (rand == 2)
               {
                    //speed = new Vector2 (-2, 0);
                    returnVec = new Vector3(-1/5f, 0, 1.3f);
                    someDub = 1.3;
               }
               else if (rand == 3)
               {
                    //speed = new Vector2 (0, 2);

                    returnVec = new Vector3(0, 1/5f, 1.3f);
                    someDub = 1.3;
               }
               else
               {
                    //speed = new Vector2 (0, -2);
                    returnVec = new Vector3(0, -1/5f, 1.3f);
                    someDub = 1.3;
               }
          }
          return returnVec;
     }

     public void findPos()
     {
          currentX = transform.position.x;
          currentY = transform.position.y;

          if (player != null)
          {
               playerX = player.transform.position.x;
               playerY = player.transform.position.y;

               angle = Vector2.Angle(player.transform.position, transform.position);
               direction = new Vector2(playerX - currentX, playerY - currentY);
               direction = direction.normalized;
          }
     }

     public bool checkStun()
     {
          return (stunTimer > 0);
     }

     public void setStun(float st)
     {
          stunTimer = st;
     }


     public virtual void onDeath()
     {
          if (deathPile)
          {
               GameObject deathObject = (GameObject)Instantiate(deathPile, this.transform.position, Quaternion.identity);
               if(deathSprite != null)
               {
                    deathObject.GetComponent<SpriteRenderer>().sprite = deathSprite;
                    deathObject.transform.localScale = transform.localScale;
                    deathObject.GetComponent<EnemyMoveController>().Knockback(GetComponent<Health>().getKnockback() * 5);
               }
          }

          GameManager.incrementKills();
          //death stuff for sub classes
     }
}

