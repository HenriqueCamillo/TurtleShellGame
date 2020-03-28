using UnityEngine;

public abstract class Selectable : MonoBehaviour
{
    public bool isSelected;
    public abstract void Select();
    public abstract void Unselect();
    public virtual void SecondClick() {}
}