using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instace;

    [SerializeField] GameObject shellTurtleUI;
    [SerializeField] GameObject blockPlacementUI;

    public delegate void EventHandler();
    public event EventHandler OnUIDisabled;

    private void Awake()
    {
        if (instace == null)
            instace = this;
        else if (instace != this)
            Destroy(this.gameObject);

        DisableUI();
    }

    private void Start()
    {
        Selector.instance.OnUnselected += DisableUI;
    }
    
    public void DisplayShellTurtleUI()
    {
        shellTurtleUI.SetActive(true);
    }

    public void DisplayBlockPlacementUI()
    {
        blockPlacementUI.SetActive(true);
    }

    public void DisableUI()
    {
        shellTurtleUI.SetActive(false);
        blockPlacementUI.SetActive(false);
        OnUIDisabled?.Invoke();
    }
}