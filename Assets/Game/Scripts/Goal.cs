using System;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [NonSerialized] public bool IsReached = false;
    [SerializeField] private GoalKey key;
    private bool isOpen = false;

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
        if (collision.CompareTag("Player"))
        {
            if (CanOpen() == false) return;
            //Debug.Log("Goal reached!");
            //Debug.Log(collision.gameObject.name + " Has Reached the Goal!");
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
