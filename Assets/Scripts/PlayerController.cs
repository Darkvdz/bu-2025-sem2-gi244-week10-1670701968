using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float jumpForce;
    public float gravityModifier;
    public int jumpCount;
    public float health;
    public ParticleSystem explosionParticle;
    public ParticleSystem dirtParticle;

    //public Transform spawnFxPoint;
    //public GameObject crashFx;

    public AudioClip jumpSfx;
    public AudioClip crashSfx;

    private Rigidbody rb;
    private InputAction jumpAction;
    private bool isOnGround = true;

    private Animator playerAnim;
    private AudioSource playerAudio;

    public bool gameOver = false;

    private MoveLeft moveLeft;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Physics.gravity *= gravityModifier;

        jumpAction = InputSystem.actions.FindAction("Jump");

        gameOver = false;

        moveLeft = GameObject.Find("Background").GetComponent<MoveLeft>();

        health = 3.0f;
    }

    // Update is called once per frame
    void Update()
    {

        if (jumpAction.triggered && jumpCount<2 && !gameOver)
        {
            rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
            jumpCount++;
            playerAnim.SetTrigger("Jump_trig");
            dirtParticle.Stop();
            playerAudio.PlayOneShot(jumpSfx);
        }

        if (Keyboard.current.shiftKey.isPressed)
        {
            moveLeft.Dash();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
            dirtParticle.Play();
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            health -= 1.0f;
            Instantiate(explosionParticle, transform.position, Quaternion.identity);
            {
                if (!gameOver && health > 0) 
                {
                    
                    Destroy(collision.gameObject);
                    playerAudio.PlayOneShot(crashSfx);

                }

                else
                {
                    Debug.Log("Game Over!");
                    gameOver = true;
                    playerAnim.SetBool("Death_b", true);
                    playerAnim.SetInteger("DeathType_int", 1);
                    explosionParticle.Play();
                    dirtParticle.Stop();
                    playerAudio.PlayOneShot(crashSfx);
                }

            }

        }
    }

}