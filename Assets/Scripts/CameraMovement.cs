using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 mouseLastPosition;
    private Vector3 movement;

    [SerializeField] bool moving = false;
    public bool Moving 
    {
        get => moving;
        set
        {
            mouseLastPosition = Input.mousePosition;
            moving = value;
        }
    }

    private void Update()
    {
        if (moving)
        {
            movement = Camera.main.ScreenToWorldPoint(mouseLastPosition) -  Camera.main.ScreenToWorldPoint(Input.mousePosition) ;
            transform.position += movement;
            mouseLastPosition = Input.mousePosition;
        }
    }

}