using System;
using UnityEngine;

public class GoalKey : MonoBehaviour
{
    [NonSerialized] public bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollected)
        {
            Debug.Log("Goal Key Collected!");
            isCollected = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
