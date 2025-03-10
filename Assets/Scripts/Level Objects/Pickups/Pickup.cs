﻿using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{
     public int ValueOfPickup;
     public int MinimumValueRange;
     public int MaximumValueRange;


     public virtual void Start()
     {
          if (ValueOfPickup == 0)
          {
               ValueOfPickup = Random.Range(1, 4);
          }
     }

     void OnTriggerEnter2D(Collider2D other)
     {
          if (other.gameObject.tag == "Player")
          {

               sendPickupMessage();
               AddItemToInventory(other, ValueOfPickup);
               Destroy(gameObject);
          }
     }

     public virtual void AddItemToInventory(Collider2D player, int value)
     {

     }
     public virtual void sendPickupMessage()
     {

     }
}