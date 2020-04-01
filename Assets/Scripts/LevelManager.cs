using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    private int turtlesWithoutShell;

    public int TurtlesWithoutShell
    {
        get {return turtlesWithoutShell;}
        set
        {
            turtlesWithoutShell = value;
            if (turtlesWithoutShell <= 0)
                EndLevel();
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    private void EndLevel()
    {
        Debug.Log("You win");
    }

}