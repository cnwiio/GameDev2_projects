using System;
using UnityEngine;

public class GoalKey : MonoBehaviour
{
    [NonSerialized] private bool isCollected = false;
    public event Action OnCollected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollected)
        {
            //Debug.Log("Goal Key Collected!");
            SoundManager.Instance.PlaySFX("Key");

            isCollected = true;
            OnCollected?.Invoke();
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
