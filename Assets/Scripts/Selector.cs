using UnityEngine;

public class Selector : MonoBehaviour
{
    private CameraMovement cameraMovement;
    private Vector3 clickPosition;
    [SerializeField] float minMoveDist;

    [SerializeField] Selectable selectedObject;
    [SerializeField] Snappable snappable;
    [SerializeField] Selectable newContent;

    private Vector2 currentTile;
    private Vector2 selectedObjectTile;

    [SerializeField] State lastState = State.Unselected;
    [SerializeField] State currentState = State.Unselected;
    [SerializeField] State nextState = State.Unselected;

    public static Selector instance;

    private State CurrentState
    {
        get => currentState;
        set
        {
            currentState = value;
            if (currentState == State.MovingCamera)
                cameraMovement.Moving = true;
            else
                cameraMovement.Moving = false;
        }
    }

    private enum State
    {
        Unselected,
        Selected,
        ActionUnitySelected,
        InAction,
        BlockUnitySelected,
        MovingCamera,
        WaitingConfirmation
    };

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);

        cameraMovement = Camera.main.GetComponent<CameraMovement>();
    }

    Vector3 lastPos;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickPosition = Input.mousePosition;

            switch (CurrentState)
            {
                case State.Unselected:
                    lastState = CurrentState;
                    if (TryToSelect())
                    {
                        CurrentState = State.WaitingConfirmation;
                        nextState = State.Selected;
                    }
                    else
                        CurrentState = State.MovingCamera;
                    break;

                case State.ActionUnitySelected:
                    currentTile = GridManager.instance.ScreenToTileIndex(Input.mousePosition);
                    lastState = CurrentState;
            
                    if (currentTile == selectedObjectTile)
                    {
                        CurrentState = State.InAction;
                        selectedObject.StartAction();
                    }
                    else if (TryToSelect())
                    {
                        CurrentState = State.WaitingConfirmation;
                        nextState = State.Selected;
                    }
                    else
                    {
                        CurrentState = State.WaitingConfirmation;
                        nextState = State.Unselected;
                    }
                    
                    break;
                
                case State.BlockUnitySelected:
                    //TODO
                    break;
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            switch (CurrentState)
            {
                case State.MovingCamera:
                    cameraMovement.Moving = false;
                    CurrentState = lastState;
                    lastState = State.MovingCamera;
                    break;

                case State.WaitingConfirmation:
                    Vector2 tile = GridManager.instance.ScreenToTileIndex(Input.mousePosition);
                    if (tile == currentTile)
                    {
                        switch (nextState)
                        {
                            case State.Unselected:
                                Unselect();
                                break;

                            case State.Selected:
                                ConfirmSelection();
                                break;
                        }
                    }
                    else
                        CurrentState = lastState;
                    break;
            }
        }

        if (CurrentState != State.MovingCamera && Input.GetMouseButton(0) && Moved())
        {
            switch (CurrentState)
            {
                case State.Unselected:
                case State.ActionUnitySelected:
                case State.BlockUnitySelected:
                    lastState = CurrentState;
                    CurrentState = State.MovingCamera;
                    break;

                case State.WaitingConfirmation:
                    CurrentState = State.MovingCamera;
                    break;
            }
        }
    }

    private bool TryToSelect()
    {
        Snappable content = GridManager.instance.GetGridContentByScreenPosition(Input.mousePosition);
        if (content != null && content.TryGetComponent(out Selectable selectable))
        {
            newContent = selectable;
            currentTile = content.tile;

            return true;
        }
        else
            return false;
    }

    private void ConfirmSelection()
    {
        if (lastState == State.ActionUnitySelected || lastState == State.BlockUnitySelected)
            selectedObject.Unselect();

        selectedObject = newContent;
        selectedObjectTile = currentTile;
        snappable = GridManager.instance.GetGridContentByTile(selectedObjectTile);
        selectedObject.Select();

        if (selectedObject.CompareTag("ActionUnity"))
            CurrentState = State.ActionUnitySelected;
        else if (selectedObject.CompareTag("BlockUnity"))
            CurrentState = State.BlockUnitySelected;


    }

    public void Unselect()
    {
        selectedObject.Unselect();
        CurrentState = State.Unselected;
    }

    private bool Moved()
    {
        Vector2 click = Camera.main.ScreenToWorldPoint(clickPosition);
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        return Vector2.Distance(click, mouse) > minMoveDist;
    }

    public void AcitonEnded()
    {
        lastState = CurrentState;
        CurrentState = State.ActionUnitySelected;
        selectedObject.Select();
        
    }
}