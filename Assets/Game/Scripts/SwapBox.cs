using System;
using TarodevController;
using UnityEngine;

public class SwapBox : MonoBehaviour
{
    [NonSerialized] public bool isReady = false;
    [NonSerialized] private bool isEntered = false;
    [NonSerialized] public GameObject player;
    [NonSerialized] public GameObject target;
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
        //Debug.Log(collision.gameObject == target);
        if (collision.gameObject == target) return;
        if (collision.CompareTag("Player") && !isEntered)
        {
            //Debug.Log(gameObject.name + "Player Entered Swap Box!"); 
            player = collision.gameObject;
            collision.GetComponent<PlayerMovement>().ToggleEnble(false);
            player.transform.localPosition = transform.position;
            isEntered = true;
            isReady = true;
            //Debug.Log(Equals(collision.gameObject.transform.localPosition, transform.position));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == target) target = null;
    }

    public void SwapToPos(Vector3 TargetPos)
    {
        player.transform.localPosition = TargetPos;
        player.transform.GetComponent<PlayerMovement>().ToggleEnble(true);
        isReady = false;
        isEntered = false;
        player = null;
    }

    public void SetTarget(GameObject gameObject)
    {
        if(target != null) return;
        target = gameObject;
    }
}
