using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellTurtle : Selectable
{
    [SerializeField] GameObject shellPrefab;
    [SerializeField] GameObject arrow;
    private bool isAiming = false;
    [SerializeField] float minDragDistance = 0.3f;


    private bool IsAiming 
    {
        get => isAiming;
        set
        {
            isAiming = value;
            arrow.SetActive(value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isAiming)
        {
            Vector2 direction = GetNearestDirection();
            RotateArrow(direction);

            if (Input.GetMouseButtonUp(0))
            {
                if (Vector2.Distance(this.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) > minDragDistance)
                {
                    ThrowShell(direction);
                    Selector.instance.Unselect();
                }
                else
                {
                    Debug.Log("Too close");
                }
            }
        }        
    }

    private void ThrowShell(Vector2 direction)
    {
        GameObject shell = Instantiate(shellPrefab, this.transform.position, Quaternion.identity);
        shell.transform.rotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, direction));
    }

    private Vector2 GetNearestDirection()
    {
        Vector2 direction = this.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);

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

    public override void Select()
    {
        isSelected = true;
        GetComponent<SpriteRenderer>().color = Color.green;
    }

    public override void Unselect()
    {
        isSelected = false;
        IsAiming = false;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public override void SecondClick()
    {
        IsAiming = true;
    }


    void RotateArrow(Vector2 direction)
    {
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, direction));
    }
}
