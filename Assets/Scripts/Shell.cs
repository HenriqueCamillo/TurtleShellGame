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
    private bool wasTrownWithRightHand;
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
        Vector2 movement = (Vector2)this.transform.position + direction * speed * Time.deltaTime;
        Vector2 correction = CalculatePathCorrection();

        rBody.MovePosition(movement + correction);
    }

    private Vector2 CalculatePathCorrection()
    {

        Vector3 position = this.transform.position - gridPosition;
        Vector2 positionInTile = new Vector2(position.x % cellSize, position.y % cellSize);
        Vector2 correction = Vector2.zero;

        float angle = (Vector2.SignedAngle(Vector2.right, direction) + 360f) % 180f;

        switch (angle)
        {
            // Horizontal
            case 0f:
                if (Mathf.Abs(positionInTile.y - cellSize/2f) > correctionTolerance)
                {
                    // Fixes vertical position   
                    if (positionInTile.y > cellSize/2f)
                        correction = Vector2Int.down;
                    else
                        correction = Vector2Int.up;
                }
                break;

            // Vertical
            case 90f:
                if (Mathf.Abs(positionInTile.x - cellSize/2f) > correctionTolerance)
                {
                    // Fixes horizontal position   
                    if (positionInTile.x > cellSize/2f)
                        correction = Vector2Int.left;
                    else
                        correction = Vector2Int.right;
                }
                break;

            // Ascending diagonal
            case 45f:
                Vector2 upperPathStart = new Vector2(0f, cellSize/2f);
                Vector2 lowerPathStart = new Vector2(cellSize/2f, 0f);

                Vector2 upperNearestPoint = FindNearestPointOnLine(upperPathStart, direction, positionInTile);
                Vector2 lowerNearestPoint = FindNearestPointOnLine(lowerPathStart, direction, positionInTile);

                float upperPathDistance = Vector2.Distance(positionInTile, upperNearestPoint);
                float lowerPathDistance = Vector2.Distance(positionInTile, lowerNearestPoint);

                // Checks if path is above or under the center of the tile
                if (upperPathDistance < lowerPathDistance)
                {
                    // Checks if it needs correction
                    if (upperPathDistance > correctionTolerance)
                    {
                        // Finds out the correction direction
                        float angularCoefficient = AngularCoefficient(upperPathStart, positionInTile);

                        if (angularCoefficient > 1f)
                            correction = Vector2.down;
                        else
                            correction = Vector2.up;
                    }
                }
                else
                {
                    // Checks if it needs correction
                    if (lowerPathDistance > correctionTolerance)
                    {
                        // Finds out the correction direction
                        float angularCoefficient = AngularCoefficient(lowerPathStart, positionInTile);

                        if (angularCoefficient > 1f)
                            correction = Vector2.down;
                        else
                            correction = Vector2.up;
                    }
                }
                break;

            // Descending diagonal
            case 135f:
                upperPathStart = new Vector2(cellSize/2f, 1f);
                lowerPathStart = new Vector2(0f, cellSize/2f);

                upperNearestPoint = FindNearestPointOnLine(upperPathStart, direction, positionInTile);
                lowerNearestPoint = FindNearestPointOnLine(lowerPathStart, direction, positionInTile);

                upperPathDistance = Vector2.Distance(positionInTile, upperNearestPoint);
                lowerPathDistance = Vector2.Distance(positionInTile, lowerNearestPoint);

                // Checks if path is above or under the center of the tile
                if (upperPathDistance < lowerPathDistance)
                {
                    // Checks if it needs correction
                    if (upperPathDistance > correctionTolerance)
                    {
                        // Finds out the correction direction
                        float angularCoefficient = AngularCoefficient(upperPathStart, positionInTile);

                        if (angularCoefficient > 1f)
                            correction = Vector2.down;
                        else
                            correction = Vector2.up;
                    }
                }
                else
                {
                    // Checks if it needs correction
                    if (lowerPathDistance > correctionTolerance)
                    {
                        // Finds out the correction direction
                        float angularCoefficient = AngularCoefficient(lowerPathStart, positionInTile);

                        if (angularCoefficient > 1f)
                            correction = Vector2.down;
                        else
                            correction = Vector2.up;
                    }
                }
                break;
        }

        return correction * correctionFactor * Time.deltaTime;
    }

    private Vector2 FindNearestPointOnLine(Vector2 origin, Vector2 direction, Vector2 point)
    {
        direction.Normalize();
        Vector2 lhs = point - origin;

        float dotP = Vector2.Dot(lhs, direction);
        return origin + direction * dotP;
    }

    private float AngularCoefficient(Vector2 start, Vector2 end)
    {
        float deltaX = end.x - start.x;
        float deltaY = end.y - start.y;
        
        return deltaY / deltaX;
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