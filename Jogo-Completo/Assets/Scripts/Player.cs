using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float normalSpeed = 5f;
    public float sprintSpeed = 10f;
    public float sprintDuration = 0.3f;
    public float doubleTapTime = 0.25f;

    private float lastPressD;
    private float lastPressA;
    private bool isSprinting;
    private float sprintTimer;

    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");

        // Movimenta��o normal
        if (!isSprinting)
        {
            rb.linearVelocity = new Vector2(move * normalSpeed, rb.linearVelocity.y);
        }

        // Verifica duplo toque no D
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (Time.time - lastPressD <= doubleTapTime)
                StartSprint(1);
            lastPressD = Time.time;
        }

        // Verifica duplo toque no A
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Time.time - lastPressA <= doubleTapTime)
                StartSprint(-1);
            lastPressA = Time.time;
        }

        // Controle do sprint
        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;
            if (sprintTimer <= 0)
                StopSprint();
        }

        // Anima��o
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        if (isSprinting)
            anim.SetBool("Speed", true);
        else
            anim.SetBool("Speed", false);
    }

    void StartSprint(int direction)
    {
        isSprinting = true;
        sprintTimer = sprintDuration;
        rb.linearVelocity = new Vector2(direction * sprintSpeed, rb.linearVelocity.y);
    }

    void StopSprint()
    {
        isSprinting = false;
    }
}