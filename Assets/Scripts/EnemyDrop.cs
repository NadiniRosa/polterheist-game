using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [Header("Drop Prefabs")]
    public GameObject[] commonDrops;
    public GameObject[] rareDrops;

    [Header("Drop Chances")]
    [Range(0f, 1f)] public float rareDropChance = 0.2f;

    public void DropItems()
    {
        float roll = Random.value;

        if (roll < rareDropChance && rareDrops != null && rareDrops.Length > 0)
        {
            int index = Random.Range(0, rareDrops.Length);
            Instantiate(rareDrops[index], transform.position, Quaternion.identity);
        }
        else if (commonDrops != null && commonDrops.Length > 0)
        {
            int index = Random.Range(0, commonDrops.Length);
            Instantiate(commonDrops[index], transform.position, Quaternion.identity);
        }
    }
}
