using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Inventory : MonoBehaviour
{
    [SerializeField] InventoryItem[] items;
    private BlockUI[] blocks;
    [SerializeField] GameObject blockUI;
    public int selectedBlockIndex;
    [SerializeField] int widthSpaceUnits;

    private void Start()
    {
        blocks = new BlockUI[items.Length];

        for (int i = 0; i < items.Length; i++)
        {
            GameObject block = Instantiate(blockUI, this.transform);
            blocks[i] = block.GetComponent<BlockUI>();
            blocks[i].Initialize(items[i].block, i, this);

            if (items[i].quantity <= 0)
                blocks[i].Disable();
        }

        GridLayoutGroup gridLayout = GetComponent<GridLayoutGroup>();
        RectTransform rectTransform = GetComponent<RectTransform>();

        float xSize = gridLayout.cellSize.x * transform.childCount + gridLayout.spacing.x * (widthSpaceUnits + transform.childCount);
        rectTransform.sizeDelta = new Vector2(xSize, rectTransform.sizeDelta.y);

        Selector.instance.OnBlockPlaced += DecreaseBlockCount;
        Selector.instance.OnUnselected += UnselectBlock;
    }

    private void DecreaseBlockCount()
    {
        items[selectedBlockIndex].quantity--;
        //TODO blocks[selectedBlockIndex].quantityText.text = items[selectedBlockIndex].quantity.ToString();

        if (items[selectedBlockIndex].quantity <= 0)
            blocks[selectedBlockIndex].Disable();

    }

    private void UnselectBlock()
    {
        selectedBlockIndex = -1;
    }
}