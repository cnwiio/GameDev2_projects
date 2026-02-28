using System;
using TarodevController;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [NonSerialized] public bool IsReached = false;
    [SerializeField] private GoalKey key;
    [SerializeField] private GameObject Target;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (CanOpen() == false) return;
            if (Target != null && collision.gameObject != Target) return;
            //Debug.Log("Goal reached!");
            //Debug.Log(collision.gameObject.name + " Has Reached the Goal!");
            collision.GetComponent<PlayerMovement>().ToggleEnble(false);
            IsReached = true;
        }
    }

    private bool CanOpen()
    {
        if(key == null)
            return true;
        if (key.isCollected)
            return true;
        return false;
    }
}
