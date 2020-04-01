using UnityEngine;

public class TurtleInDanger : MonoBehaviour
{
    [SerializeField] GameObject shellTurtle;
    private Snappable snappable;

    private void Start()
    {
        snappable = GetComponent<Snappable>();
        LevelManager.instance.TurtlesWithoutShell++;
    }

    public void Rescue()
    {
        GridManager.instance.SetGridContentByTile(null, snappable.tile);
        //TODO play animation
        Instantiate(shellTurtle, this.transform.position, Quaternion.identity);
        LevelManager.instance.TurtlesWithoutShell--;
        Destroy(this.gameObject);
    }
}