using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    [SerializeField]
    private Enemy enemy;
    private GameObject kunai;
    void Start()
    {

    }
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.tag == "Enemy")
        //{
        //    return; // do nothing
        //}
        if (other.tag == "Player")
        {
            enemy.Target = other.gameObject;

        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            enemy.Target = null;
        }
    }
}
