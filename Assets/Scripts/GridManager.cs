using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    [SerializeField] float cellSize;
    [SerializeField] int gridSize = 40;
    private GameObject[,] grid;
    private Vector2 lastTile = Vector2.zero;
    public GameObject obj;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)        
            Destroy(this.gameObject);

        grid = new GameObject[gridSize, gridSize];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 tile = WorldToTileIndex(clickPosition);
            Vector2 position = TileToWorldPosition(tile);

            GameObject gmObj = Instantiate(obj, position, Quaternion.identity);
            grid[(int)tile.x, (int)tile.y] = gmObj;
        }

        Highlight(); 
    }

    void Highlight()
    {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 tile = WorldToTileIndex(mousePosition);

            if (tile == lastTile)           return;
            if (tile.x < 0 || tile.y < 0)   return;
            
            if (grid[(int)lastTile.x, (int)lastTile.y] != null)
                grid[(int)lastTile.x, (int)lastTile.y].GetComponent<SpriteRenderer>().color = Color.white; 
        
            if (grid[(int)tile.x, (int)tile.y] != null)
                grid[(int)tile.x, (int)tile.y].GetComponent<SpriteRenderer>().color = Color.blue; 

            lastTile = tile;
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
}