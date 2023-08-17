using UnityEngine;

[CreateAssetMenu(fileName = "BaseItem", menuName = "Custom SO/BaseItem")]
public class Item : ScriptableObject
{
    
    [SerializeField] string itemName = null;
    [SerializeField] string description = null;
    [SerializeField] int goldValue = 0;
    [SerializeField] bool useInBattle = false;
    [SerializeField] bool useInInventory = false;
    [SerializeField] int sortingOrder = 0;
    [SerializeField] ItemType typeOfItem = 0;
    [SerializeField] Sprite itemSprite;


    
    public string ItemName => itemName;
    public string Description => description;
    public int GoldValue => goldValue;
    public bool UseInBattle => useInBattle;
    public bool UseInInventory => useInInventory;
    public int SortingOrder => sortingOrder;
    public ItemType TypeOfItem => typeOfItem;
    public Sprite ItemSprite => itemSprite;
}

public enum ItemType
{
    healing,
}
