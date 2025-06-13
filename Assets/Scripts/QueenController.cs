using UnityEngine;
using UnityEngine.InputSystem;

public class QueenController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump King Settings")]
    [SerializeField] private float maxChargeTime = 1.2f;
    [SerializeField] private float minJumpForce = 6f;
    [SerializeField] private float maxJumpForce = 26f;
    [SerializeField] private float diagonalJumpAngle = 38f;     // Góc thấp → bay xa
    [SerializeField] private float straightUpExtraForce = 1.2f; // Nhảy thẳng vẫn cao
    [SerializeField] private float jumpSpeedMultiplier = 1.4f;  // Tăng lực tổng thể

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isCharging;
    private float chargeTime;
    private int jumpDirection = 0;

    private InputAction moveAction;
    private InputAction jumpAction;

    private void Awake()
    {
        moveAction = new InputAction(type: InputActionType.Value);
        moveAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/a")
            .With("Negative", "<Keyboard>/leftArrow")
            .With("Positive", "<Keyboard>/d")
            .With("Positive", "<Keyboard>/rightArrow");

        jumpAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");
        jumpAction.started += OnJump;
        jumpAction.canceled += OnJump;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (context.started && isGrounded)
        {
            isCharging = true;
            chargeTime = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;

            float moveInput = moveAction.ReadValue<float>();
            jumpDirection = Mathf.RoundToInt(moveInput); // -1 / 0 / 1
        }
        else if (context.canceled && isCharging)
        {
            isCharging = false;
            rb.bodyType = RigidbodyType2D.Dynamic;

            float chargeRatio = Mathf.Clamp01(chargeTime / maxChargeTime);
            float baseForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargeRatio);

            Vector2 force;

            if (jumpDirection == 0)
            {
                // Nhảy thẳng: tăng chiều cao
                float yForce = baseForce * straightUpExtraForce * jumpSpeedMultiplier;
                force = new Vector2(0f, yForce);
            }
            else
            {
                // Nhảy nghiêng: ưu tiên khoảng cách
                float angleRad = diagonalJumpAngle * Mathf.Deg2Rad;
                float xForce = Mathf.Cos(angleRad) * baseForce * jumpSpeedMultiplier * 1.2f * jumpDirection; // nhân thêm 1.2f
                float yForce = Mathf.Sin(angleRad) * baseForce * jumpSpeedMultiplier;

                force = new Vector2(xForce, yForce);
            }

            rb.linearVelocity = force;
        }
    }


    private void HandleMovement()
    {
        if (isCharging || !isGrounded) return;

        float moveInput = moveAction.ReadValue<float>();
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (isCharging)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Min(chargeTime, maxChargeTime);
        }

        HandleMovement();
    }
}
