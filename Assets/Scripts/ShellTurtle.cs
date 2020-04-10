﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellTurtle : Selectable
{
    [SerializeField] GameObject shellPrefab;
    [SerializeField] GameObject arrow;
    private bool isAiming = false;
    private bool inAction = false;
    [SerializeField] float minDragDistance = 0.3f;
    public bool isUsingRightHand;

    // 8 direction vectors
    private Vector2[] possibleDirections = {Vector2.up, Vector2.right, Vector2.down, Vector2.left,
                                (Vector2.up + Vector2.right).normalized, (Vector2.right + Vector2.down).normalized,
                                (Vector2.down + Vector2.left).normalized, (Vector2.left + Vector2.up).normalized};

    private bool IsAiming 
    {
        get => isAiming;
        set
        {
            isAiming = value;
            arrow.SetActive(value);
        }
    }

    private void Update()
    {
        //TODO change to UI
        if (Input.GetMouseButtonDown(1))
            ChangeHand();

        if (inAction)
        {
            Vector2 direction = GetThrowDirection();
            RotateArrow(direction);

            // Enables or disables the arrow depending on the distance form the center of the tile
            IsAiming = Vector2.Distance(this.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) > minDragDistance;

            if (Input.GetMouseButtonUp(0))
            {
                if (IsAiming)
                {
                    ThrowShell(direction);
                    Unselect();
                }
                else
                {
                    Selector.instance.AcitonEnded();
                }

                inAction = false;
            }
        }        
    }

    private void ThrowShell(Vector2 direction)
    {
        GameObject shell = Instantiate(shellPrefab, arrow.transform.position, Quaternion.identity, this.transform);
        shell.transform.rotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, direction));
    }

    private Vector2 GetThrowDirection()
    {
        // Works like a slingshot, the shell will be thrown to the opposite direction of the mouse
        Vector2 direction = (this.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)).normalized;

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

    public override void Select()
    {
        isSelected = true;
        UIManager.instace.DisplayShellTurtleUI();
        GetComponent<SpriteRenderer>().color = Color.green;
    }

    public override void Unselect()
    {
        isSelected = false;
        inAction = false;
        IsAiming = false;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public override void StartAction()
    {
        inAction = true;
        UIManager.instace.DisableUI();
    }

    private void RotateArrow(Vector2 direction)
    {
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, direction));
        arrow.transform.position = this.transform.position;

        // If diagonal direction
        if (direction != Vector2.up && direction != Vector2.right && direction != Vector2.down && direction != Vector2.left)
        {
            float cellDiagonal = GridManager.instance.CellSize * Mathf.Sqrt(2f);
            if (isUsingRightHand)
                arrow.transform.position -=  (Vector3)Vector2.Perpendicular(direction).normalized * (cellDiagonal / 4f);
            else
                arrow.transform.position +=  (Vector3)Vector2.Perpendicular(direction).normalized * (cellDiagonal / 4f);
        }

        Debug.DrawRay(arrow.transform.position, direction * 100, Color.green, 0.1f);
    }

    public void ChangeHand()
    {
        isUsingRightHand = !isUsingRightHand;
    }
}