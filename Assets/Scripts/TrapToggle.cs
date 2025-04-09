using UnityEngine;

public class TrapToggle : MonoBehaviour
{
    [Header("Timing")]
    public float toggleInterval = 2f;
    private float timer;

    [Header("Sprites")]
    [SerializeField] private Sprite trapInactiveSprite;
    [SerializeField] private Sprite trapActiveSprite;

    private SpriteRenderer spriteRenderer;
    private Collider trapCollider;

    private bool isActive = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        trapCollider = GetComponent<Collider>();

        if (trapCollider == null)
        {
            enabled = false;
            return;
        }

        if (spriteRenderer == null)
        {
            enabled = false;
            return;
        }

        SetTrapState(false);
        timer = toggleInterval;
    }


    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            isActive = !isActive;

            SetTrapState(isActive);

            timer = toggleInterval;
        }
    }

    void SetTrapState(bool active)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = active ? trapActiveSprite : trapInactiveSprite;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.detectCollisions = active;
    }
}
