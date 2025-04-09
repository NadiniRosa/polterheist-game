using UnityEngine;

public class DoorLock : MonoBehaviour
{
    private BoxCollider boxCollider;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite _doorClosed;
    [SerializeField] private Sprite _doorOpen;

    private bool _wasOpen = false;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateLockState();
    }

    void Update()
    {
        UpdateLockState();
    }

    void UpdateLockState()
    {
        if (boxCollider == null || spriteRenderer == null) return;

        bool isOpen = EnemyTracker.AllEnemiesDefeated();

        boxCollider.isTrigger = isOpen;

        if (_wasOpen != isOpen)
        {
            spriteRenderer.sprite = isOpen ? _doorOpen : _doorClosed;
            _wasOpen = isOpen;
        }
    }
}
