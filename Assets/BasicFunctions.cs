using System;
using UnityEngine;
using System.Collections;

public class BasicFunctions : MonoBehaviour
{
     private Player player;
     private bool follow;

     // Use this for initialization
     void Start()
     {

     }

     // Update is called once per frame
     void Update()
     {
          if(follow)
          {
               transform.position = player.transform.position;
          }
     }

     public void Death()
     {
          Destroy(gameObject);
     }

     public void FollowPlayer()
     {
          follow = true;
          player = FindObjectOfType<Player>();
     }

     public void Death(float time)
     {
          Destroy(gameObject, time);
     }

}
