using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    public int damage = 4;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyTracker.Unregister(other.gameObject);

            EnemyDrop drop = other.GetComponent<EnemyDrop>();
            
            if (drop != null)
            {
                drop.DropItems();
            }

            Destroy(other.gameObject);
        }

        Destroy(gameObject);
    }
}
