using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float collisionDetectionDistance;
    private Rigidbody2D rBody;
    private Vector2 direction;

    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        direction = (this.transform.rotation * Vector3.right).normalized;
    }

    void FixedUpdate()
    {
        rBody.MovePosition((Vector2)this.transform.position + direction * speed * Time.deltaTime);
        CheckCollision();
    }

    void CheckCollision()
    {
        Debug.DrawLine(this.transform.position, this.transform.position + (Vector3)direction * collisionDetectionDistance, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction, collisionDetectionDistance, LayerMask.GetMask("Walls")); 

        if (hit)
        {
            if (hit.collider.CompareTag("SoftWall"))
                direction = Vector2.Reflect(direction, hit.normal);
            else if (hit.collider.CompareTag("HardWall"))
                Destroy(this.gameObject);
        }
    }
}
