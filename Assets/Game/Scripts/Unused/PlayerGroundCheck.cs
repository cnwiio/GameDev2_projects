using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    public Transform groundCheck;
    public float checkRadius = 0.15f;
    public LayerMask groundLayer;
    public bool IsGrounded;
    void Update()
    {
        IsGrounded = Physics2D.OverlapCircle(
        groundCheck.position,
        checkRadius,
        groundLayer
        );
    }
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}