using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
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

    [Header("Audio")]
    public AudioClip damageClip;
    public AudioClip dieClip;
    public AudioClip gunReloadingClip;
    public AudioClip gunShotClip;
    public AudioClip dropCollectedClip;

    private AudioSource audioSource;

    [Header("Level Info")]
    public int currentLevel = 1;
    public TextMeshProUGUI levelText;

    public ScreenTransitions screenTransition;

    [Header("GameOver")]
    public GameObject gameOverCanvas;
    public TextMeshProUGUI finalScore;
    private bool isDead = false;

    private Animator animator;

    [Header("Score")]
    public int currentScore = 0;
    public TextMeshProUGUI scoreText;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        currentAmmo = maxAmmo;
        
        UpdateAmmoUI();
        UpdateHealthUI();
        UpdateLevelUI();
        UpdateScoreUI();
    }

    void Update()
    {
        if (isDead)
            return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(h, v, 0).normalized;
        animator.SetBool("IsWalking", moveDirection.magnitude > 0);

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
                    Color color = playerSprite.color;
                    color.a = color.a == 1f ? 0f : 1f;
                    playerSprite.color = color;
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
        if (isDead)
            return;

        if (other.CompareTag("TrapTile") || other.CompareTag("Enemy") && !isInvincible)
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

        if (other.CompareTag("Drop"))
        {
            DropItem drop = other.GetComponent<DropItem>();

            if (drop != null)
            {
                int gainedScore = drop.dropType == DropItem.DropType.Rare ? drop.rareScore : drop.commonScore;
                currentScore += gainedScore;
                UpdateScoreUI();
                audioSource.PlayOneShot(dropCollectedClip);
            }

            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        UpdateHealthUI();
        audioSource.PlayOneShot(damageClip);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        audioSource.PlayOneShot(dieClip);
        animator.SetBool("IsDead", true);
        
        finalScore.text = "Score: " + currentScore;
        gameOverCanvas.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    void RegenerateRoomAndRespawn()
    {
        void DoRegeneration()
        {
            roomGenerator?.GenerateRoom();

            if (spawnPoint != null)
            {
                controller.enabled = false;
                transform.position = spawnPoint.position;
                controller.enabled = true;
            }

            currentLevel++;
            UpdateLevelUI();

            screenTransition?.StartShrink();
        }

        justTeleported = true;
        teleportTimer = teleportCooldown;

        if (screenTransition != null)
        {
            screenTransition.StartExpand(DoRegeneration);
        }
        else
        {
            DoRegeneration();
        }
    }

    #region Shooting
    void Shoot()
    {
        currentAmmo--;
        UpdateAmmoUI();
        audioSource.PlayOneShot(gunShotClip);

        if (bulletPrefab != null && gunMuzzle != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, gunMuzzle.position, gunMuzzle.rotation, bulletContainer);

            Collider bulletCol = bullet.GetComponent<Collider>();
            Collider playerCol = GetComponent<Collider>();

            if (bulletCol != null && playerCol != null)
            {
                Physics.IgnoreCollision(bulletCol, playerCol);
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        audioSource.PlayOneShot(gunReloadingClip);

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

    void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = "Level " + currentLevel;
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }
    #endregion
}
