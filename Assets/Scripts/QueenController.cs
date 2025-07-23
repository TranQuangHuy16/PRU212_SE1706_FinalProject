using UnityEngine;
using UnityEngine.InputSystem;

public class QueenController : MonoBehaviour
{
    [Header("Map Boundary")]
    [SerializeField] private float leftLimit = -10f;
    [SerializeField] private float rightLimit = 10f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump King Settings")]
    [SerializeField] private float maxChargeTime = 1.2f;
    [SerializeField] private float minJumpForce = 6f;
    [SerializeField] private float maxJumpForce = 26f;
    [SerializeField] private float diagonalJumpAngle = 38f;     // Góc thấp → bay xa
    [SerializeField] private float straightUpExtraForce = 1.2f; // Nhảy thẳng vẫn cao
    [SerializeField] private float jumpSpeedMultiplier = 1.4f;  // Tăng lực tổng thể

    [Header("Bounce Settings")]
    [SerializeField] private float bounceForceMultiplier = 0.5f; // hệ số phản lực khi va chạm
    [SerializeField] private float bounceThresholdSpeed = 4f;    // tốc độ đủ mạnh mới bật lại


    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isCharging;
    private float chargeTime;
    private int jumpDirection = 0;

    private InputAction moveAction;
    private InputAction jumpAction;
    private Animator animator;
    private GameManager gameManager;
    private AudioManager audioManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioManager = FindAnyObjectByType<AudioManager>();
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindAnyObjectByType<GameManager>();
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


        rb.gravityScale = 2.5f;  // tăng tốc độ rơi
        rb.linearDamping = 0f;          // đảm bảo không bị "bay chậm"

        if (PlayerPrefs.GetInt("HasSaved", 0) == 1)
        {
            float x = PlayerPrefs.GetFloat("PlayerX");
            float y = PlayerPrefs.GetFloat("PlayerY");
            transform.position = new Vector2(x, y);
        }
    }

    private void updateAnimation()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isJumping = !isGrounded;
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
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
            float power = Mathf.Lerp(minJumpForce, maxJumpForce, chargeRatio); // Lực tổng thể

            Vector2 force;

            //if (jumpDirection == 0)
            //{
            //    // Nhảy thẳng đứng (Space + không bấm A/D)
            //    float yForce = power * straightUpExtraForce * jumpSpeedMultiplier;
            //    force = new Vector2(0f, yForce);
            //}
            if (jumpDirection == 0)
            {
                // Nhảy thẳng đứng (Space + không bấm A/D)
                float yForce = power * jumpSpeedMultiplier; // giảm lực để không nhảy quá cao
                force = new Vector2(0f, yForce);
            }

            else
            {
                float maxAngle = diagonalJumpAngle; // ví dụ: 55 độ cho cung cong
                float minAngle = 25f;               // cho lực mạnh vẫn còn góc cong

                float angleDeg = Mathf.Lerp(maxAngle, minAngle, chargeRatio);

                float angleRad = angleDeg * Mathf.Deg2Rad;

                float speed = power * jumpSpeedMultiplier;

                float xForce = Mathf.Cos(angleRad) * speed * jumpDirection;
                float yForce = Mathf.Sin(angleRad) * speed;

                force = new Vector2(xForce, yForce);
            }

            rb.linearVelocity = force;

            audioManager.PlaySoundJump();
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
        if (gameManager.IsGameWin()) return;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (isCharging)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Min(chargeTime, maxChargeTime);
        }

        HandleMovement();
        updateAnimation();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;
            bool isFalling = rb.linearVelocity.y < -bounceThresholdSpeed; // chỉ bật nếu rơi đủ nhanh
            bool hitWall = Mathf.Abs(normal.x) > 0.7f;    // đụng tường ngang
            bool hitCeiling = normal.y < -0.7f;           // đập đầu vào trần

            // Bật lại nếu va vào tường hoặc trần khi đang rơi
            if (isFalling && (hitWall || hitCeiling))
            {
                Vector2 reflected = Vector2.Reflect(rb.linearVelocity, normal);
                rb.linearVelocity = reflected * bounceForceMultiplier;
                break;
            }
        }
    }


    private void LateUpdate()
    {
        // Giới hạn vị trí X không vượt ra khỏi vùng cho phép
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, leftLimit, rightLimit);
        transform.position = pos;
    }


}
