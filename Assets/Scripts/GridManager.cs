using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    [SerializeField] float cellSize;
    [SerializeField] int gridSize;
    private Snappable[,] grid;
    private Vector2Int lastTile = Vector2Int.zero;
    [SerializeField] Tilemap softWall;
    [SerializeField] Tilemap hardWall;
    [SerializeField] bool showGrid;
    [SerializeField] bool showGridLimits;

    public float CellSize => cellSize;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)        
            Destroy(this.gameObject);

        grid = new Snappable[gridSize, gridSize];
    }

    private void Update()
    {
        // Highlight(); 
        // if (Input.GetMouseButtonDown(0))
        //     TileIsFree(ScreenToTileIndex(Input.mousePosition));
    }


    private void Highlight()
    {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int tile = WorldToTileIndex(mousePosition);

            if (tile == lastTile)           return;
            if (tile.x < 0 || tile.y < 0)   return;
            
            if (grid[(int)lastTile.x, (int)lastTile.y] != null)
                grid[(int)lastTile.x, (int)lastTile.y].gameObject.GetComponent<SpriteRenderer>().color = Color.white; 
        
            if (grid[(int)tile.x, (int)tile.y] != null)
                grid[(int)tile.x, (int)tile.y].gameObject.GetComponent<SpriteRenderer>().color = Color.blue; 

            lastTile = tile;
    }

    public bool PlaceByTile(Snappable snap, Vector2Int tile)
    {
        if (!TileIsFree(tile))
            return false; 

        // If moving a tile (not placing a new), frees the last tile
        if (snap.isPlaced)
            SetGridContentByTile(null, tile);

        snap.Place(tile);
        SetGridContentByTile(snap, tile);

        return true;
    }

    public bool PlaceByScreenPosition(Snappable snap, Vector2 screenPosition)
    {
        Vector2Int tile = WorldToTileIndex(Camera.main.ScreenToWorldPoint(screenPosition));
        return PlaceByTile(snap, tile);
    }

    public bool TileIsFree(Vector2Int tile)
    {
        if (!ValidTile(tile))
            return false;
        else if (GetGridContentByTile(tile) != null)
        {
            // Debug.Log("Occupied tile (object)");
            return false;
        }
        else
        {
            TileBase hardWallTile = softWall.GetTile((Vector3Int)tile);
            TileBase softWallTile = hardWall.GetTile((Vector3Int)tile);

            if (hardWallTile != null || softWallTile != null)
            {
                // Debug.Log("Occupied tile (tilemap)");
                return false;
            }
            else
                return true;
        }
    }

    public void SetGridContentByTile(Snappable content, Vector2Int tile)
    {
        if (!ValidTile(tile))
            return;
        else
            grid[tile.x, tile.y] = content;
    }
    public Snappable GetGridContentByTile(Vector2Int tile)
    {
        if (!ValidTile(tile))
            return null;
        else
            return grid[tile.x, tile.y];
    }

    public Snappable GetGridContentByScreenPosition(Vector2 screenPosition)
    {
        return GetGridContentByTile(ScreenToTileIndex(screenPosition));
    }

    public bool ValidTile(Vector2Int tile)
    {
        if (tile.x % 1 != 0 || tile.y % 1 != 0 || tile.x < 0 || tile.y < 0)
        {
            Debug.LogWarning("Invalid tile");
            return false;
        }
        else
            return true;
    }

    public Vector2Int ScreenToTileIndex(Vector2 screenPosition)
    {
        Vector2 world = Camera.main.ScreenToWorldPoint(screenPosition);
        return WorldToTileIndex(world);
    }

    public Vector2 WorldToGridPosition(Vector2 position)
    {
        return TileToWorldPosition(WorldToTileIndex(position));
    }

    public Vector2Int WorldToTileIndex(Vector2 position)
    {
        // The tile index must be relative to the position of the grid
        position -= (Vector2)this.transform.position;

        Vector2Int tile = new Vector2Int((int)Mathf.Floor(position.x/cellSize), (int)Mathf.Floor(position.y/cellSize));
        return tile;
    }
    public Vector2 TileToWorldPosition(Vector2Int tile)
    {
        return (Vector2)this.transform.position + tile + new Vector2(cellSize/2, cellSize/2);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, cellSize/2f);

        if (showGrid)
        {
            for (int i = 0; i < gridSize; i++)
            {
                Gizmos.DrawLine(this.transform.position + Vector3.up * cellSize * gridSize + Vector3.right * i, this.transform.position + Vector3.right * i);
                Gizmos.DrawLine(this.transform.position + Vector3.right * cellSize * gridSize + Vector3.up * i, this.transform.position + Vector3.up * i);
            }
        }

        if (showGridLimits)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, this.transform.position + Vector3.right * gridSize * cellSize);
            Gizmos.DrawLine(this.transform.position, this.transform.position + Vector3.up * gridSize * cellSize);

            Gizmos.DrawLine(this.transform.position + Vector3.up * gridSize * cellSize, this.transform.position + Vector3.up * gridSize * cellSize + Vector3.right * gridSize * cellSize);
            Gizmos.DrawLine(this.transform.position + Vector3.right * gridSize * cellSize, this.transform.position + Vector3.up * gridSize * cellSize + Vector3.right * gridSize * cellSize);
        }
    }
}