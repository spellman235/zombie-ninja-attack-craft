﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormSpawn : MonoBehaviour {

   public GameObject norm;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

   private void OnTriggerEnter2D(Collider2D collision)
   {
      norm.SetActive(true);
      this.gameObject.SetActive(false);
   }
}
