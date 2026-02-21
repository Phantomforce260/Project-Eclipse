using UnityEngine;
using UnityEngine.InputSystem;

public class DepotController : MonoBehaviour
{
    public static bool MovementEnabled = true;

    [Range(1, 50), SerializeField] private float baseVelocity = 10f;
    [Range(1, 10), SerializeField] private float acceleration = 5f;

    private PlayerInputActions inputActions;

    private new Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;

    private float currentVelocityX;
    private Vector2 moveInput;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        spriteRenderer.flipX = currentVelocityX < 0;
    }

    private void FixedUpdate()
    {
        float targetVelocityX = MovementEnabled
            ? moveInput.x * baseVelocity
            : 0;

        currentVelocityX = Mathf.Lerp(currentVelocityX, targetVelocityX, acceleration * Time.deltaTime);
        rigidbody.linearVelocity = new Vector2(currentVelocityX, rigidbody.linearVelocityY);

        //Debug.Log("Input: " + moveInput.x + ", target: " + targetVelocityX + ", current: " + currentVelocityX + ", rigidbody: " + rigidbody.linearVelocity.x);
    }
}
