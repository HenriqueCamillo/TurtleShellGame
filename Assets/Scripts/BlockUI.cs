using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BlockUI : MonoBehaviour, IPointerClickHandler
{
    private bool disabled = false;
    public GameObject prefab;
    private int index;
    private Image image;
    public TextMeshProUGUI quantityText;
    private Inventory inventory;



    private void Awake()
    {
        image = GetComponent<Image>();        
        Selector.instance.OnUnselected += Unselect;
    }


    public void Initialize(InventoryItem item, int index, Inventory inventory)
    {
        this.prefab = item.block;
        quantityText.text = item.quantity.ToString();

        this.index = index;
        this.inventory = inventory;
        image.sprite = prefab.GetComponent<SpriteRenderer>().sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!disabled)
        {
            if (index == inventory.selectedBlockIndex)
            {
                Selector.instance.Unselect();
            }
            else
            {
                Selector.instance.SelectBlockUnity(this);
                image.color = Color.green;
                inventory.selectedBlockIndex = index;
            }
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