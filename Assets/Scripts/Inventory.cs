using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject[] blocks;
    [SerializeField] GameObject blockUI;

    void Start()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            GameObject block = Instantiate(blockUI, this.transform);
            block.GetComponent<BlockUI>().Initialize(blocks[i]);
        }

        GridLayoutGroup gridLayout = GetComponent<GridLayoutGroup>();
        RectTransform rectTransform = GetComponent<RectTransform>();

        float xSize = gridLayout.cellSize.x * transform.childCount + gridLayout.spacing.x * (2 + transform.childCount);
        rectTransform.sizeDelta = new Vector2(xSize, rectTransform.sizeDelta.y);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
