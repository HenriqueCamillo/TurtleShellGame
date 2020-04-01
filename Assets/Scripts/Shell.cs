using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float collisionDetectionDistance;
    private Rigidbody2D rBody;
    private Vector2 direction;

    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        direction = (this.transform.rotation * Vector3.right).normalized;
    }

    private void FixedUpdate()
    {
        rBody.MovePosition((Vector2)this.transform.position + direction * speed * Time.deltaTime);
        CheckCollision();
    }

    private void CheckCollision()
    {
        Debug.DrawLine(this.transform.position, this.transform.position + (Vector3)direction * collisionDetectionDistance, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction, collisionDetectionDistance, LayerMask.GetMask("Walls") | LayerMask.GetMask("Turtles")); 

        if (hit)
        {
            if (hit.collider.CompareTag("SoftWall"))
            {
                Vector2 newDirection = Vector2.Reflect(direction, hit.normal);
                
                // Doesn't reflect in the opposite direction to avoid loops
                if (newDirection == -direction)
                {
                    Selector.instance.AcitonEnded();
                    Destroy(this.gameObject);
                }
                else
                {
                    direction = newDirection;   
                }
            }
            else if (hit.collider.CompareTag("HardWall") || hit.collider.CompareTag("ActionUnity"))
            {
                Selector.instance.AcitonEnded();
                Destroy(this.gameObject);
            }
            else if (hit.collider.CompareTag("TurtleInDanger"))
            {
                Selector.instance.AcitonEnded();
                hit.collider.GetComponent<TurtleInDanger>().Rescue();
                Destroy(this.gameObject);
            }
        }
    }
}