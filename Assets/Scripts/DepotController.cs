using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class DepotController : MonoBehaviour
{
    public static bool MovementEnabled = true;

    public static Vector2 Position => instance.transform.position;

    public static Inventory PlayerInventory => instance.playerInventory;

    public static int PackagesDelivered;

    public int JumpCount { get; private set; }

    private const float fallVelocity = -15.5f;
    private const string JumpKey = "isJumping";
    private const string JumpInput = "Jump";

    private static DepotController instance;

    [Header("Horizontal Movement")]
    [Range(1, 50), SerializeField] private float baseVelocity = 10f;
    [Range(1, 10), SerializeField] private float acceleration = 5f;

    [Header("Vertical Movement")]
    [Range(1, 50), SerializeField] private float jumpVelocity = 16f;
    [Range(0.5f, 1), SerializeField] private float jumpCutHeight;
    [Range(0.01f, 0.5f), SerializeField] private float coyoteTime = 0.1f;
    [Range(0.01f, 0.5f), SerializeField] private float jumpBuffer = 0.1f;

    private Inventory playerInventory;
    private PlayerInputActions inputActions;

    private new Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private CapsuleCollider2D groundCheck;

    private readonly LayerMask groundLayer = 1 << 6;

    private float coyoteTimeCounter;
    private bool isCoyoteTimerOver;

    private float jumpBufferCounter;
    private bool isJumpBuffering;

    private float currentVelocityX;
    private Vector2 moveInput;

    private bool isGrounded;
    private bool isFalling;

    [Serializable]
    public struct Inventory {
        public List<string> Packages;
    }

    private void Awake()
    {
        playerInventory.Packages = new(3);

        if (instance == null)
            instance = this;

        inputActions = new PlayerInputActions();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        groundCheck = GetComponent<CapsuleCollider2D>();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        isGrounded = IsGrounded();
        isFalling = rigidbody.linearVelocityY < fallVelocity;

        bool jumpInput = MovementEnabled && Input.GetButtonDown(JumpInput);
        moveInput = MovementEnabled
            ? inputActions.Player.Move.ReadValue<Vector2>()
            : Vector2.zero;

        spriteRenderer.flipX = currentVelocityX < 0;

        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("velocityY", rigidbody.linearVelocityY);
        animator.SetFloat("velocityX", Mathf.Abs(rigidbody.linearVelocityX));

        if (isGrounded)
        {
            if (rigidbody.linearVelocityY <= 0)
            {
                animator.SetBool(JumpKey, false);
                coyoteTimeCounter = coyoteTime;
            }

            isCoyoteTimerOver = false;
            if (isJumpBuffering)
                Jump();

            isJumpBuffering = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            if (coyoteTimeCounter <= 0 && !isCoyoteTimerOver)
            {
                JumpCount = 1;
                isCoyoteTimerOver = true;
            }

            if (isJumpBuffering)
            {
                jumpBufferCounter -= Time.deltaTime;
                if (jumpBufferCounter <= 0)
                    isJumpBuffering = false;
            }
        }

        if (jumpInput && (JumpCount < 1 || isGrounded || coyoteTimeCounter > 0))
            Jump();
        else if (jumpInput && !isGrounded)
        {
            jumpBufferCounter = jumpBuffer;
            isJumpBuffering = true;
        }

        if (Input.GetButtonUp(JumpInput) && rigidbody.linearVelocityY > 0)
            rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocityX, rigidbody.linearVelocityY * jumpCutHeight);
    }

    private void FixedUpdate()
    {
        float targetVelocityX = MovementEnabled
            ? moveInput.x * baseVelocity
            : 0;

        currentVelocityX = Mathf.Lerp(currentVelocityX, targetVelocityX, acceleration * Time.deltaTime);
        rigidbody.linearVelocity = new Vector2(currentVelocityX, rigidbody.linearVelocityY);
    }

    private void Jump()
    {
        animator.SetBool(JumpKey, true);
        animator.SetTrigger("Jump");
        rigidbody.linearVelocity = Vector2.up * jumpVelocity;
        coyoteTimeCounter = 0;
        JumpCount++;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = IsGrounded();
        if (isGrounded)
            JumpCount = 0;
    }

    /* IsGrounded:
     * 
     * Creates a box raycast from the player's waist to a small amount below the feet.
     * If this raycast collides with the ground layer, the player is grounded. */
    protected bool IsGrounded()
    {
        const float extraHeight = 0.2f;
        RaycastHit2D hit2D = Physics2D.BoxCast(groundCheck.bounds.center, groundCheck.bounds.size - new Vector3(0.1f, 0f, 0f), 0f, Vector2.down, extraHeight, groundLayer);
        Color rayColor = hit2D.collider != null ? Color.green : Color.red;

        /* These Debug.DrawRay statements allow you to see the box raycast by going to the game view and
         * enabling "Gizmos". They appear green when the player is grounded, and red otherwise.
         * These lines can be removed during builds, as they do not interfere with raycast calculations. */
        Debug.DrawRay(groundCheck.bounds.center + new Vector3(groundCheck.bounds.extents.x, 0), Vector2.down * (groundCheck.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(groundCheck.bounds.center - new Vector3(groundCheck.bounds.extents.x, 0), Vector2.down * (groundCheck.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(groundCheck.bounds.center - new Vector3(groundCheck.bounds.extents.x, groundCheck.bounds.extents.y + extraHeight), groundCheck.bounds.extents.x * 2 * Vector2.right, rayColor);

        return hit2D.collider != null;
    }
}
