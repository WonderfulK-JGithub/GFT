using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemText : MonoBehaviour
{
    [SerializeField] TMP_Text itemNameText;
    [SerializeField] TMP_Text itemAmountText;
    [SerializeField] Image itemImage;
    [SerializeField] GameObject imageBackdrop;

    public Color color
    {
        get
        {
            return itemNameText.color;
        }
        set
        {
            itemNameText.color = value;
            itemAmountText.color = value;
        }
    }

    public void SetText(Item _item,int _amount)
    {
        itemAmountText.text = _amount.ToString();
        itemNameText.text = _item.ItemName;
        itemImage.sprite = _item.ItemSprite;
    }

    public void SetNull()
    {
        itemNameText.text = "none";
        itemAmountText.enabled = false;
        itemImage.enabled = false;
        imageBackdrop.SetActive(false);
    }
}
