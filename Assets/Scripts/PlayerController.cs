using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class TopDownPlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;

    [Header("Health")]
    public int maxHealth = 3;
    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private int currentHealth;

    [Header("Damage Cooldown")]
    public float damageCooldown = 1.5f;

    private bool isInvincible = false;
    private float damageCooldownTimer = 0f;

    [Header("Blinking")]
    public SpriteRenderer playerSprite;
    public float blinkInterval = 0.1f;

    private float blinkTimer = 0f;

    [Header("Next Level Settings")]
    public Transform spawnPoint;
    public ProceduralRoomGenerator roomGenerator;

    [Header("Teleport")]
    private bool justTeleported = false;
    private float teleportCooldown = 1.5f;
    private float teleportTimer = 0f;

    [Header("Movement")]
    private CharacterController controller;
    private Vector3 moveDirection;

    [Header("Gun Settings")]
    public Transform gunMuzzle;
    public GameObject bulletPrefab;

    public int maxAmmo = 6;
    private int currentAmmo;

    public Image[] ammoIcons;
    public Sprite fullBullet;
    public Sprite emptyBullet;

    public float reloadTime = 0.6f;
    private bool isReloading = false;

    public Transform bulletContainer;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        currentHealth = maxHealth;
        currentAmmo = maxAmmo;
        
        UpdateAmmoUI();
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

        if (isInvincible)
        {
            damageCooldownTimer -= Time.deltaTime;
            blinkTimer -= Time.deltaTime;

            if (blinkTimer <= 0f)
            {
                if (playerSprite != null)
                {
                    playerSprite.enabled = !playerSprite.enabled;
                }
                blinkTimer = blinkInterval;
            }

            if (damageCooldownTimer <= 0f)
            {
                isInvincible = false;
                if (playerSprite != null)
                    playerSprite.enabled = true;
            }
        }

        if (!isReloading)
        {
            if (Input.GetButtonDown("Fire1") && currentAmmo > 0)
            {
                Shoot();
            }
            else if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(Reload());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TrapTile") && !isInvincible)
        {
            TakeDamage(1);

            isInvincible = true;
            damageCooldownTimer = damageCooldown;
            blinkTimer = blinkInterval;
        }

        if (other.CompareTag("DoorTile") && !justTeleported)
        {
            if (EnemyTracker.AllEnemiesDefeated())
            {
                RegenerateRoomAndRespawn();
            }
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

    #region Shooting
    void Shoot()
    {
        currentAmmo--;
        UpdateAmmoUI();

        if (bulletPrefab != null && gunMuzzle != null)
        {
            Instantiate(bulletPrefab, gunMuzzle.position, gunMuzzle.rotation, bulletContainer);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        isReloading = false;
    }

    #endregion

    #region UI
    void UpdateHealthUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].sprite = i < currentHealth ? fullHeart : emptyHeart;
        }
    }

    void UpdateAmmoUI()
    {
        for (int i = 0; i < ammoIcons.Length; i++)
        {
            ammoIcons[i].sprite = i < currentAmmo ? fullBullet : emptyBullet;
        }
    }
    #endregion
}
