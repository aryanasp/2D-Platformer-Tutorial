﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{

    [SerializeField]
    private BoxCollider2D platformCollider;
    [SerializeField]
    private BoxCollider2D platformTrigger;

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreCollision(platformCollider, platformTrigger, true);

    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
        {
            Physics2D.IgnoreCollision(platformCollider, other, true);
            
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
        {
            Physics2D.IgnoreCollision(platformCollider, other, false);
           
        }
    }
}
