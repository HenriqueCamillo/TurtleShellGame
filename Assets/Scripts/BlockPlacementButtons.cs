using UnityEngine;

public class BlockPlacementButtons : MonoBehaviour
{
    public void RotateClockwise()
    {
       Rotate(-90f); 
    }

    public void RotateAnticlockwise()
    {
        Rotate(90f);
    }

    private void Rotate(float angle)
    {
        Vector3 currentRotation = Selector.instance.placingObject.transform.rotation.eulerAngles;
        Vector3 newRotation = new Vector3(currentRotation.x, currentRotation.y, currentRotation.z + angle);
        Selector.instance.placingObject.transform.rotation = Quaternion.Euler(newRotation);
    }

    public void Confirm()
    {
        Selector.instance.ConfirmPlacement();
    }

    public void Cancel()
    {
        Selector.instance.CancelPlacement();
    }
}