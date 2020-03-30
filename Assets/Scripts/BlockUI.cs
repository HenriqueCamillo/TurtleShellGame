using UnityEngine;
using UnityEngine.UI;

public class BlockUI : MonoBehaviour
{
    private GameObject prefab;
    [SerializeField] Image image;


    void Start()
    {
        image = GetComponent<Image>();        
    }

    public void Initialize(GameObject prefab)
    {
        this.prefab = prefab;
        image.sprite = prefab.GetComponent<SpriteRenderer>().sprite;
    }


}