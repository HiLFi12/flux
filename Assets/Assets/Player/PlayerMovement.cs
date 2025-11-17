using System;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class PlayerMovement : MonoBehaviour
{
    public enum PlayerState { Solid, Liquid, Gas }

    [Header("Current State")]
    public PlayerState currentState = PlayerState.Solid;

    [Header("Solid Settings")]
    public float solidMoveSpeed = 4f;
    public float solidJumpForce = 8f;
    public float solidGravity = 3f;
    public Vector2 solidScale = new Vector2(4f, 6f);

    [Header("Solid Strength")]
    public float solidMaxStrength = 100f;
    [SerializeField] private float _solidStrength = 100f;

    private bool isTouchingWall;
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.6f;

    [Header("Solid Interaction")]
    public float breakBlockCost = 20f;
    public float breakDistance = 1f;
    public LayerMask breakableLayer;

    public float fallDamageMultiplier = 5f;
    public float solidJumpStrengthLoss = 5f;
    public float solidMinScaleY = 3f;

    private float fallStartY;
    private bool wasGrounded;

    public float SolidStrength
    {
        get => _solidStrength;
        set
        {
            float prev = _solidStrength;
            _solidStrength = Mathf.Clamp(value, 0f, solidMaxStrength);
            if (!Mathf.Approximately(prev, _solidStrength))
                OnSolidStrengthChanged?.Invoke(_solidStrength, solidMaxStrength);

            if(_solidStrength <= 0f)
                OnPlayerDied();
        }
    }

    [Header("Liquid Settings")]
    public float liquidMoveSpeed = 6f;
    public float liquidGravity = 0.5f;
    public float liquidDrag = 2f;
    public Vector2 liquidScale = new Vector2(4f, 4f);
    public float liquidMaxHealth = 100f;
    public float liquidCurrentHealth = 100f;
    public float liquidDrainPerSecond = 10f;
    public float liquidMinScaleY = 1.0f;

    [Header("Gas Settings")]
    public float gasMoveSpeed = 3f;
    public float gasFlapForce = 2.5f;
    public float gasGravity = 0.3f;
    public float gasMaxVerticalSpeed = 5f;
    public Vector2 gasScale = new Vector2(3f, 3f);
    public float gasMaxHealth = 100f;
    public float gasCurrentHealth = 100f;
    public float gasDrainPerFlap = 10f;

    public float gasMinScaleX = 1.0f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveSpeed;
    private float jumpForce;
    private Collider2D[] playerColliders;
    [SerializeField] private SpriteRenderer visualSprite;
    [SerializeField] private Transform visualRoot;
    private float lastAppliedScaleY = -1f;
    [SerializeField] private float scaleEpsilon = 0.002f;

    public event Action<PlayerState> OnStateChanged;
    public event Action<float, float> OnLiquidHealthChanged;
    public event Action<float, float> OnGasHealthChanged;
    public event Action<float, float> OnSolidStrengthChanged;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SolidStrength = Mathf.Clamp(_solidStrength, 0f, solidMaxStrength);
        wasGrounded = true;
        fallStartY = transform.position.y;

        // material sin fricción
        var mats = GetComponents<Collider2D>();
        PhysicsMaterial2D zeroFric = new PhysicsMaterial2D("pm_zero") { friction = 0f, bounciness = 0f };
        foreach (var c in mats)
            c.sharedMaterial = zeroFric;
    }

    void Start()
    {
        if (visualSprite == null)
        {
            var srs = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sr in srs)
            {
                if (sr.gameObject != this.gameObject)
                {
                    visualSprite = sr;
                    break;
                }
            }
        }

        playerColliders = GetComponents<Collider2D>();
        if (visualRoot == null && visualSprite != null)
            visualRoot = visualSprite.transform;

        SetState(PlayerState.Solid);
        UpdateSolidScale();
    }

    void Update()
    {
        CheckGround();
        HandleFallDamage();
        Move();
        JumpOrFlap();
        UpdateSolidScale();
        BreakPlatform();
        CheckWall();
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    void HandleFallDamage()
    {
        if (currentState != PlayerState.Solid) return;

        if (!wasGrounded && isGrounded)
        {
            float fallDistance = Mathf.Abs(fallStartY - transform.position.y);
            if (fallDistance > 1f)
            {
                float damage = fallDistance * fallDamageMultiplier;
                SolidStrength -= damage;
            }
        }

        if (!isGrounded && wasGrounded)
            fallStartY = transform.position.y;

        wasGrounded = isGrounded;
    }

    // --------------------
    //   MOVE (ARREGLADO)
    // --------------------
    void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        // anti-pared
        if (isTouchingWall && moveInput != 0f)
        {
            if (Mathf.Sign(moveInput) == Mathf.Sign(transform.localScale.x))
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        // arrastre líquido
        rb.linearDamping = (currentState == PlayerState.Liquid) ? liquidDrag : 0f;

        // drenaje del líquido
        if (currentState == PlayerState.Liquid)
        {
            bool isMovingHoriz = Mathf.Abs(moveInput) > 0.01f;
            if (isMovingHoriz && liquidCurrentHealth > 0f)
            {
                liquidCurrentHealth -= liquidDrainPerSecond * Time.deltaTime;
                liquidCurrentHealth = Mathf.Max(0f, liquidCurrentHealth);
                OnLiquidHealthChanged?.Invoke(liquidCurrentHealth, liquidMaxHealth);
                if (liquidCurrentHealth <= 0f) OnPlayerDied();
            }

            float t = Mathf.Clamp01(liquidCurrentHealth / liquidMaxHealth);
            float targetY = Mathf.Lerp(liquidMinScaleY, liquidScale.y, t);
            ApplyVisualScale(new Vector2(liquidScale.x, targetY));
        }

    
        if (currentState == PlayerState.Solid && Mathf.Abs(moveInput) > 0.01f)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(moveInput), 0.6f);
            if (hit.collider != null && hit.collider.CompareTag("Movable"))
            {
                MovableBlock block = hit.collider.GetComponent<MovableBlock>();
                if (block != null)
                {
                    if (SolidStrength >= block.requiredStrength)
                        block.TryPush(moveInput, SolidStrength, solidMaxStrength, solidMoveSpeed);
                    else
                        block.Stop();
                }
            }
        }


        if (currentState == PlayerState.Gas)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                Mathf.Clamp(rb.linearVelocity.y, -gasMaxVerticalSpeed, gasMaxVerticalSpeed)
            );
        }
    }

    // --------------------
    //  SALTO / FLAP
    // --------------------
    void JumpOrFlap()
    {
        if (currentState == PlayerState.Solid && Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, solidJumpForce);
            SolidStrength -= solidJumpStrengthLoss;
        }

        if (currentState == PlayerState.Gas && Input.GetKeyDown(KeyCode.Space))
        {
            float vy = Mathf.Min(rb.linearVelocity.y + gasFlapForce, gasMaxVerticalSpeed);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, vy);

            gasCurrentHealth -= gasDrainPerFlap;
            gasCurrentHealth = Mathf.Max(0, gasCurrentHealth);
            if (gasCurrentHealth <= 0f) OnPlayerDied();

            float t = Mathf.Clamp01(gasCurrentHealth / gasMaxHealth);
            float x = Mathf.Lerp(gasMinScaleX, gasScale.x, t);
            ApplyVisualScale(new Vector2(x, gasScale.y));
            OnGasHealthChanged?.Invoke(gasCurrentHealth, gasMaxHealth);
        }
    }

    void UpdateSolidScale()
    {
        if (currentState != PlayerState.Solid) return;
        float t = Mathf.Clamp01(SolidStrength / solidMaxStrength);
        float newY = Mathf.Lerp(solidMinScaleY, solidScale.y, t);
        ApplyVisualScale(new Vector2(solidScale.x, newY));
    }

    private void ApplyVisualScale(Vector2 targetXY)
    {
        if (visualRoot == null) return;
        if (Mathf.Abs(targetXY.y - lastAppliedScaleY) < scaleEpsilon) return;

        var s = visualRoot.localScale;
        s.x = targetXY.x;
        s.y = targetXY.y;
        s.z = 1f;
        visualRoot.localScale = s;
        lastAppliedScaleY = targetXY.y;
    }

        
    private void BreakPlatform()
    {
        if (currentState != PlayerState.Solid || SolidStrength <= 0f) 
            return;

        Vector2 dir = Vector2.zero;

        // DIRECCIONES PARA ROMPER
        if (Input.GetKey(KeyCode.D)) dir = Vector2.right;
        else if (Input.GetKey(KeyCode.A)) dir = Vector2.left;
        else dir = Vector2.down;  
        

        if (!Input.GetKey(KeyCode.E)) return;

        
        Vector2 origin = (Vector2)transform.position + dir * 0.5f;

       
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, 1f, breakableLayer);
        Debug.DrawRay(origin, dir * 1f, Color.red, 0.2f);

        if (hit.collider == null) return;

        
        BreakablePlatform bp = hit.collider.GetComponent<BreakablePlatform>();
        if (bp != null)
        {
            bp.TriggerBreak();
            SolidStrength -= breakBlockCost;
            return;
        }

        
        BreakableBlock bb = hit.collider.GetComponent<BreakableBlock>();
        if (bb != null)
        {
            bb.TriggerBreak();
            SolidStrength -= breakBlockCost;
            return;
        }
    }

    public void SetState(PlayerState newState)
    {
        currentState = newState;

       

        switch (currentState)
        {
            case PlayerState.Solid:
                moveSpeed = solidMoveSpeed;
                rb.gravityScale = solidGravity;
                rb.linearDamping = 0f;
                SetVisualColor(Color.gray);
                ApplyVisualScale(solidScale);
                UpdateMovableCollision(true);
                break;

            case PlayerState.Liquid:
                moveSpeed = liquidMoveSpeed;
                rb.gravityScale = liquidGravity;
                rb.linearDamping = liquidDrag;
                SetVisualColor(Color.cyan);
                UpdateMovableCollision(false);
                break;

            case PlayerState.Gas:
                moveSpeed = gasMoveSpeed;
                rb.gravityScale = gasGravity;
                rb.linearDamping = 0f;
                SetVisualColor(Color.white);
                UpdateMovableCollision(true);
                break;
        }
        OnStateChanged?.Invoke(currentState);
    }

    public void RefillLiquid()
    {
        liquidCurrentHealth = liquidMaxHealth;
        OnLiquidHealthChanged?.Invoke(liquidCurrentHealth, liquidMaxHealth);
    }

    public void RefillGas()
    {
        gasCurrentHealth = gasMaxHealth;
        OnGasHealthChanged?.Invoke(gasCurrentHealth, gasMaxHealth);
    }

    private void OnPlayerDied() 
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

    }

    private void UpdateMovableCollision(bool enable)
    {
        if (playerColliders == null || playerColliders.Length == 0) return;
        var movables = GameObject.FindGameObjectsWithTag("Movable");
        foreach (var movable in movables)
        {
            var movableColliders = movable.GetComponents<Collider2D>();
            foreach (var playerCol in playerColliders)
            {
                foreach (var movableCol in movableColliders)
                    Physics2D.IgnoreCollision(playerCol, movableCol, !enable);
            }
        }
    }

    void CheckWall()
    {
        float dir = Input.GetAxisRaw("Horizontal");
        isTouchingWall = false;

        if (Mathf.Abs(dir) < 0.1f) return;

        Vector2 origin = (Vector2)transform.position + Vector2.right * dir * 0.5f;
        float distance = 0.15f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * Mathf.Sign(dir), distance, wallLayer);
        
        isTouchingWall = hit.collider != null;
    }

    private void SetVisualColor(Color c)
    {
        if (visualSprite == null)
        {
            var srs = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sr in srs)
            {
                if (sr.gameObject != this.gameObject)
                {
                    visualSprite = sr;
                    break;
                }
            }
        }
        if (visualSprite != null)
            visualSprite.color = c;
    }

    public void RestoreSolidStrength()
    {
        SolidStrength = solidMaxStrength;
    }
}
