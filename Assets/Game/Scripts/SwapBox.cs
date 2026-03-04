using System;
using TarodevController;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class SwapBox : MonoBehaviour
{
    [NonSerialized] public bool isReady = false;
    [NonSerialized] private bool isEntered = false;
    [NonSerialized] public GameObject player;
    [NonSerialized] public GameObject target;

    [SerializeField] private TextMeshPro textMesh;

    private void Update()
    {
        if (isEntered)
        {
            if (Input.GetKeyDown(KeyCode.E) && player != null)
            {
                player.GetComponent<PlayerMovement>().ToggleEnble(false);
                player.transform.localPosition = transform.position;
                isReady = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject == target);
        if (collision.gameObject == target) return;
        if (collision.CompareTag("Player") && !isEntered)
        {
            //Debug.Log(gameObject.name + "Player Entered Swap Box!"); 
            player = collision.gameObject;
            isEntered = true;
            ToggleText(true);
            //Debug.Log(Equals(collision.gameObject.transform.localPosition, transform.position));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isEntered = false;
        ToggleText(false);
        if (collision.gameObject == target) target = null;
    }

    public void SwapToPos(Vector3 TargetPos)
    {
        player.transform.localPosition = TargetPos;
        player.transform.GetComponent<PlayerMovement>().ToggleEnble(true);
        isReady = false;
        player = null;
    }

    public void SetTarget(GameObject gameObject)
    {
        if(target != null) return;
        target = gameObject;
    }

    private void ToggleText(bool isEnabled)
    {
        if (textMesh == null)
        {
            Debug.LogWarning(gameObject.name + " TextMeshPro component is not assigned in the inspector! Please assign it to display swap instructions.");
            return;
        }
        textMesh.enabled = isEnabled;
    }
}
