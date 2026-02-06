using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float moveSpeed = 5f; // Horizontal speed
    public float jumpForce = 7f; // Upward impulse for jump
    public PlayerGroundCheck groundCheck; // Reference to your GroundCheck script
    private Rigidbody2D rb;
    private float moveInput; // -1, 0, or +1 from keyboard
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        // 1) Read horizontal input (A/D or Left/Right arrows)
        moveInput = Input.GetAxisRaw("Horizontal");
        // 2) Jump only when grounded
        if (groundCheck != null && groundCheck.IsGrounded)
        {
            // "Jump" is mapped to Space by default in the old Input Manager
            if (Input.GetButtonDown("Jump"))
            {
                // Reset vertical velocity to make jump consistent
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }
    void FixedUpdate()
    {
        // Apply horizontal movement in physics step
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }
}