using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float collisionDetectionDistance;
    private Rigidbody2D rBody;
    public Vector2 direction = Vector2.zero;
    private Vector3 gridPosition;
    private float cellSize;
    [SerializeField] float correctionTolerance;
    [SerializeField] float correctionFactor;
    private Vector2[] possibleDirections = {Vector2.up, Vector2.right, Vector2.down, Vector2.left,
                                (Vector2.up + Vector2.right).normalized, (Vector2.right + Vector2.down).normalized,
                                (Vector2.down + Vector2.left).normalized, (Vector2.left + Vector2.up).normalized};

    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>();

        cellSize = GridManager.instance.CellSize;
        gridPosition = GridManager.instance.transform.position;
    }

    private void FixedUpdate()
    {
        Move();
        CheckCollision();
    }

    private void Move()
    {
        // Standar movement
        Vector2 movement = (Vector2)this.transform.position + direction * speed * Time.deltaTime;

        // Path correction
        Vector3 position = this.transform.position - gridPosition;
        Vector2 positionInTile = new Vector2(position.x % cellSize, position.y % cellSize);
        // Vector2 positionVector = new Vector2(positionInTile.x - cellSize/2f, positionInTile.y - cellSize/2f);
        Vector2 correction = Vector2.zero;

        // If not diagonal
        if (direction.x == 0 || direction.y == 0)
        {
            // Vetical movement
            if (direction.x == 0 && Mathf.Abs(positionInTile.x - cellSize/2f) > correctionTolerance)
            {
                // Fixes horizontal position
                if (positionInTile.x > cellSize/2f)
                    correction = Vector2Int.left;
                else
                    correction = Vector2Int.right;
            }
            // Horizontal movement
            else if (direction.y == 0 && Mathf.Abs(positionInTile.y - cellSize/2f) > correctionTolerance)
            {
                // Fixes vertical position   
                if (positionInTile.y > cellSize/2f)
                    correction = Vector2Int.down;
                else
                    correction = Vector2Int.up;
            }
        }

        rBody.MovePosition(movement + correction * correctionFactor * Time.deltaTime);
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
        normal = EightDirectionVector(normal);
        direction = Vector2.Reflect(direction, normal);

        return EightDirectionVector(direction); 
    }

    private Vector2 EightDirectionVector(Vector2 direction)
    {
        direction = direction.normalized;
        Vector2 nearest = new Vector2();
        float distance;
        float minDistance = 2f;

        // Searches for the direction with the minimun angle distance
        foreach (var dir in possibleDirections)
        {
            distance = Vector2.Distance(direction, dir);
            if (Vector2.Distance(direction, dir) < minDistance)
            {
                minDistance = distance;
                nearest = dir;       
            }
        }

        return nearest;
    }
}