using UnityEngine;

public class Snappable : MonoBehaviour
{
    [HideInInspector]
    public bool isPlaced = false;
    public Vector2Int tile;
    public bool positionedInInspector;

    private void Start()
    {
        // Snaps to grid if it is an object that was placed in inspector during level design
        if (positionedInInspector)
            GridManager.instance.PlaceByTile(this, GridManager.instance.WorldToTileIndex(this.transform.position));
    }

    public void Place(Vector2Int tile)
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