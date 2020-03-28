using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    [SerializeField] float cellSize;
    [SerializeField] int gridSize = 40;
    private Snappable[,] grid;
    private Vector2 lastTile = Vector2.zero;

    [SerializeField] Snappable obj;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)        
            Destroy(this.gameObject);

        grid = new Snappable[gridSize, gridSize];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 tile = WorldToTileIndex(clickPosition);
            Vector2 position = TileToWorldPosition(tile);

            // GameObject gmObj = Instantiate(obj, position, Quaternion.identity);
            // grid[(int)tile.x, (int)tile.y] = gmObj;
        }

        // Highlight(); 
    }

    void Highlight()
    {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 tile = WorldToTileIndex(mousePosition);

            if (tile == lastTile)           return;
            if (tile.x < 0 || tile.y < 0)   return;
            
            if (grid[(int)lastTile.x, (int)lastTile.y] != null)
                grid[(int)lastTile.x, (int)lastTile.y].gameObject.GetComponent<SpriteRenderer>().color = Color.white; 
        
            if (grid[(int)tile.x, (int)tile.y] != null)
                grid[(int)tile.x, (int)tile.y].gameObject.GetComponent<SpriteRenderer>().color = Color.blue; 

            lastTile = tile;
    }

    public bool PlaceByTile(Snappable snap, Vector2 tile)
    {
        // Checks if the tile is occupied
        if (!FreeTile(tile))
        {
            snap.Reset();
            return false; //TODO Change objects
        }

        if (snap.isPlaced)
        {
            SetGridContentByTile(null, tile);
        }

        snap.Place(tile);
        SetGridContentByTile(snap, tile);

        return true;
    }

    public bool PlaceByScreenPosition(Snappable snap, Vector2 screenPosition)
    {
        Vector2 tile = WorldToTileIndex(Camera.main.ScreenToWorldPoint(screenPosition));
        return PlaceByTile(snap, tile);
    }

    // public Snappable GetObject(Vector2 tile)
    // {
    //     Snappable content = GetGridContent(tile);

    //     return content;
    // }

    public bool FreeTile(Vector2 tile)
    {
        if (GetGridContentByTile(tile) != null)
        {
            Debug.Log("Occupied tile");
            return false;
        }
        else
            return true;
    }

    public void SetGridContentByTile(Snappable content, Vector2 tile)
    {
        if (!ValidTile(tile))
            return;
        else
            grid[(int)tile.x, (int)tile.y] = content;
    }
    public Snappable GetGridContentByTile(Vector2 tile)
    {
        if (!ValidTile(tile))
            return null;
        else
            return grid[(int)tile.x, (int)tile.y];
    }

    public Snappable GetGridContentByScreenPosition(Vector2 screenPosition)
    {
        return GetGridContentByTile(ScreenToTileIndex(screenPosition));
    }

    public bool ValidTile(Vector2 tile)
    {
        if (tile.x % 1 != 0 || tile.y % 1 != 0 || tile.x < 0 || tile.y < 0)
        {
            Debug.LogWarning("Invalid tile");
            return false;
        }
        else
            return true;
    }

    public Vector2 ScreenToTileIndex(Vector2 screenPosition)
    {
        Vector2 world = Camera.main.ScreenToWorldPoint(screenPosition);
        return WorldToTileIndex(world);
    }

    public Vector2 WorldToGridPosition(Vector2 position)
    {
        return TileToWorldPosition(WorldToTileIndex(position));
    }

    public Vector2 WorldToTileIndex(Vector2 position)
    {
        // The tile index must be relative to the position of the grid
        position -= (Vector2)this.transform.position;

        Vector2 tile = new Vector2(Mathf.Floor(position.x/cellSize), Mathf.Floor(position.y/cellSize));
        return tile;
    }
    public Vector2 TileToWorldPosition(Vector2 tile)
    {
        return (Vector2)this.transform.position + tile + new Vector2(cellSize/2, cellSize/2);
    }


    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);
    }
}