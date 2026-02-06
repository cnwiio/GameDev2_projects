using System;
using UnityEngine;

public class GoalKey : MonoBehaviour
{
    [NonSerialized] public bool isCollected = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollected)
        {
            Debug.Log("Goal Key Collected!");
            isCollected = true;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
