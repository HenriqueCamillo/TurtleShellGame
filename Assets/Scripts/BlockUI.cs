using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BlockUI : MonoBehaviour, IPointerClickHandler
{
    private bool disabled = false;
    private GameObject prefab;
    private int index;
    private Image image;
    public TextMeshProUGUI quantityText;
    private Inventory inventory;



    private void Awake()
    {
        image = GetComponent<Image>();        
        Selector.instance.OnUnselected += Unselect;
    }


    public void Initialize(GameObject prefab, int index, Inventory inventory)
    {
        this.prefab = prefab;
        this.index = index;
        this.inventory = inventory;
        image.sprite = prefab.GetComponent<SpriteRenderer>().sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!disabled)
        {
            Selector.instance.SelectedBlockUnity(prefab);
            image.color = Color.green;
            inventory.selectedBlockIndex = index;
        }
    }

    private void Unselect()
    {
        if (!disabled)
            image.color = Color.white;
    }

    public void Disable()
    {
        image.color = Color.gray;
        disabled = true;
    }
}