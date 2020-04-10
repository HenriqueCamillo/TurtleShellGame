using UnityEngine;

public class ChangeHandButton : MonoBehaviour
{
    public ShellTurtle turtle;
    
    private void Start()
    {
        UIManager.instace.OnUIDisabled += RemoveTurtle;
    }

    public void ChangeHand()
    {
        if (turtle == null)
            turtle = Selector.instance.SelectedObject.GetComponent<ShellTurtle>();
        
        turtle.ChangeHand();
    }

    private void RemoveTurtle()
    {
        turtle = null;
    }
}