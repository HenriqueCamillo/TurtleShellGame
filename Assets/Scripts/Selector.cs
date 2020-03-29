using UnityEngine;

public class Selector : MonoBehaviour
{
    [SerializeField] bool isSelectingObject = false;
    [SerializeField] Selectable selectedObject;
    [SerializeField] Snappable snappable;
    private Vector2 selectedObjectTile;

    public static Selector instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    void Update()
    {
        if (!isSelectingObject)
        {
            //Raycast
            if (Input.GetMouseButtonDown(0))
            {
                Snappable content = GridManager.instance.GetGridContentByScreenPosition(Input.mousePosition);
                if (content != null && content.TryGetComponent(out Selectable selectable))
                {
                    selectedObject = selectable;
                    snappable = content;
                    isSelectingObject = true;
                    selectedObjectTile = snappable.tile;

                    selectable.Select();
                }
            }
        }
        else
        {
            //Raycast
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 clickedTile = GridManager.instance.ScreenToTileIndex(Input.mousePosition);
                if (clickedTile == selectedObjectTile)
                {
                    selectedObject.SecondClick();
                }
                else
                {
                    Unselect();

                    Snappable content = GridManager.instance.GetGridContentByTile(clickedTile);
                    if (content != null && content.TryGetComponent(out Selectable selectable))
                    {
                        selectedObject = selectable;
                        snappable = content;
                        isSelectingObject = true;
                        selectedObjectTile = snappable.tile;

                        selectable.Select();
                    }
                }
                    
            }
            // heldObject.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // if (Input.GetMouseButtonDown(0))
            // {
            //     Debug.Log("Released");
            //     selectedObject.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            //     GridManager.instance.PlaceByScreenPosition(selectedObject, Input.mousePosition);
            //     selectedObject = null;
            //     isSelectingObject = false;
            // }
        }
    }

    public void Unselect()
    {
        selectedObject.Unselect();
        selectedObject = null;
        snappable = null;
        isSelectingObject = false;
    }
}