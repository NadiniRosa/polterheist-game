using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class TopDownPlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 5f;

    [Header("Player Health")]
    public int maxHealth = 3;
    private int currentHealth;

    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Next Level Settings")]
    public Transform spawnPoint;
    public ProceduralRoomGenerator roomGenerator;

    private bool justTeleported = false;
    private float teleportCooldown = 1.5f;
    private float teleportTimer = 0f;

    private CharacterController controller;
    private Vector3 moveDirection;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(h, v, 0).normalized;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        if (justTeleported)
        {
            teleportTimer -= Time.deltaTime;

            if (teleportTimer <= 0f)
            {
                justTeleported = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TrapTile"))
        {
            TakeDamage(1);
        }

        if (other.CompareTag("DoorTile") && !justTeleported)
        {
            RegenerateRoomAndRespawn();
        }
    }

    void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("PLAYER DIED!");
        // TODO: Add game over or respawn logic
    }

    void UpdateHealthUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].sprite = i < currentHealth ? fullHeart : emptyHeart;
        }
    }

    void RegenerateRoomAndRespawn()
    {
        if (roomGenerator != null)
        {
            roomGenerator.GenerateRoom();
        }

        if (spawnPoint != null)
        {
            controller.enabled = false;
            transform.position = spawnPoint.position;
            controller.enabled = true;
        }

        justTeleported = true;
        teleportTimer = teleportCooldown;
    }
}
