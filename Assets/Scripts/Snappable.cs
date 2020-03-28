using UnityEngine;

public class Snappable : MonoBehaviour
{
    public bool isPlaced = false;
    public Vector2 tile;
    [SerializeField] bool positionedInInspector;

    void Start()
    {
        if (positionedInInspector)
            GridManager.instance.PlaceByTile(this, GridManager.instance.WorldToTileIndex(this.transform.position));
    }

    public void Place(Vector2 tile)
    {
        this.tile = tile;
        this.transform.position = GridManager.instance.TileToWorldPosition(tile);
        isPlaced = true;
    }

    public void Reset()
    {
        this.transform.position = GridManager.instance.TileToWorldPosition(tile);
    }

}