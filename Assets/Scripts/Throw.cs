using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    [SerializeField] GameObject shellPrefab;
    [SerializeField] GameObject arrow;
    [SerializeField] bool isSelected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected)
        {
            Vector2 direction = GetNearestDirection();
            DrawArrow(direction);

            if (Input.GetMouseButtonDown(0))
            {
                ThrowShell(direction);
            }
        }        
    }

    private void ThrowShell(Vector2 direction)
    {
        Unselect();
        GameObject shell = Instantiate(shellPrefab, this.transform.position, Quaternion.identity);
        shell.transform.rotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, direction));
    }

    private Vector2 GetNearestDirection()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position   ;

        Vector2[] possibleDirections = {Vector2.up, Vector2.right, Vector2.down, Vector2.left,
                                        Vector2.up + Vector2.right, Vector2.right + Vector2.down,
                                        Vector2.down + Vector2.left, Vector2.left + Vector2.up};

        float minDistance = 2f, distance;
        Vector2 nearest = new Vector2();
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

    public void Select()
    {
        isSelected = true;
        arrow.SetActive(true);
    }

    void Unselect()
    {
        isSelected = false;
        arrow.SetActive(false);
    }

    void DrawArrow(Vector2 direction)
    {
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, direction));
    }
}
