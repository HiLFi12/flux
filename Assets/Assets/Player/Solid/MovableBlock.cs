using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class MovableBlock : MonoBehaviour
{
    public float requiredStrength = 50f;
    public float pushForce = 8f;
    public float maxPushSpeed = 3f;

    private Rigidbody2D rb;
    private bool isBeingPushed;
    private bool isGrabbed = false;
    private Transform grabber;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 2f;
    }

    void FixedUpdate()
    {
        if (isGrabbed && grabber != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.position = grabber.position + new Vector3(0.5f * Mathf.Sign(grabber.localScale.x), 0, 0);
        }
        else if (!isBeingPushed)
        {
            rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, 5f * Time.fixedDeltaTime), rb.linearVelocity.y);
        }

        isBeingPushed = false;
    }

    public void TryPush(float moveInput, float playerStrength, float playerMaxStrength, float playerMoveSpeed)
    {
        if (playerStrength < requiredStrength) return;

        isBeingPushed = true;
        float strengthFactor = Mathf.Clamp01(playerStrength / playerMaxStrength);
        float push = moveInput * pushForce * strengthFactor;
        rb.AddForce(new Vector2(push, 0), ForceMode2D.Force);

        if (Mathf.Abs(rb.linearVelocity.x) > maxPushSpeed)
            rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxPushSpeed, rb.linearVelocity.y);
    }

    // 🔹 Métodos para agarre
    public void Grab(Transform player)
    {
        isGrabbed = true;
        grabber = player;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void Release()
    {
        isGrabbed = false;
        grabber = null;
        rb.gravityScale = 2f;
    }

        public void Stop()
    {
        // Esto asegura que el bloque no siga moviéndose por fuerzas residuales
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

}
