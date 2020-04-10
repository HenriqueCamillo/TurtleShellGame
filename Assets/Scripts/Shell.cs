using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float collisionDetectionDistance;
    private Rigidbody2D rBody;
    private Vector2 direction;
    private Vector2[] possibleDirections = {Vector2.up, Vector2.right, Vector2.down, Vector2.left,
                                (Vector2.up + Vector2.right).normalized, (Vector2.right + Vector2.down).normalized,
                                (Vector2.down + Vector2.left).normalized, (Vector2.left + Vector2.up).normalized};

    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        direction = (this.transform.rotation * Vector3.right).normalized;
        //TODO remove this gambiarra
        this.transform.rotation = Quaternion.identity;
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

        // Ignores raycast if it hits the turtle that threw it (its parent)
        if (hit && hit.transform != this.transform.parent)
        {
            if (hit.collider.CompareTag("SoftWall"))
            {
                Vector2 newDirection = Reflect(direction, hit.normal);
                
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

    private Vector2 Reflect(Vector2 direction, Vector2 normal)
    {
        Vector2 nearest = new Vector2();
        float distance;
        float minDistance = 2f;

        // Searches for the direction with the minimun angle distance
        foreach (var dir in possibleDirections)
        {
            distance = Vector2.Distance(normal, dir);
            if (Vector2.Distance(normal, dir) < minDistance)
            {
                minDistance = distance;
                nearest = dir;       
            }
        }

        return Vector2.Reflect(direction, nearest);
    }
}