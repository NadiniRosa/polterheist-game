using UnityEngine;

public class GhostOne : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;

    public bool IsDead { get; private set; } = false;

    public AudioClip deathClip;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    public void Die()
    {
        if (IsDead)
            return;

        audioSource.PlayOneShot(deathClip);

        IsDead = true;
        animator.SetBool("IsDead", true);

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
        IsDead = true;
    }
}
