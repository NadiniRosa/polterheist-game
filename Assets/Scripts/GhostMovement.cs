using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GhostMovement : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    private CharacterController controller;
    private Transform player;
    private GhostOne ghostEnemy;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        ghostEnemy = GetComponent<GhostOne>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (ghostEnemy != null && ghostEnemy.IsDead)
            return;

        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            controller.Move(direction * moveSpeed * Time.deltaTime);

            if (direction.x != 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Sign(direction.x) * Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
        }
    }

}
