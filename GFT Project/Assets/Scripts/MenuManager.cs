using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] Color selectColor;
    [SerializeField] Color selectedColor;
    [SerializeField] Color unavailableColor;
    [SerializeField] Color selectUnavailableColor;

    [Header("MainMenu")]
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] TMP_Text[] mainOptionText;
    int mainOptionTarget;

    [Header("Inventory")]
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] Transform inventoryContents;
    [SerializeField] GameObject itemTextPrefab;
    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text itemDescription;
    [SerializeField] float itemTextOffset;

    //List<TMP_Text> loadedItemTexts = new();
    List<ItemText> loadedItemTexts = new();
    List<Item> loadedItems = new();
    int itemTarget;

    [Header("Equipment")]
    [SerializeField] GameObject equipPanel;
    [SerializeField] TMP_Text[] alliesToEquipTexts;
    [SerializeField] GameObject equipTypeBackdrop;
    [SerializeField] TMP_Text[] equipTypeTexts;
    [SerializeField] Image[] equipImages;
    [SerializeField] GameObject[] equipBackdrops;
    [SerializeField] Transform equipContents;

    int equipAllyTarget;
    int equipTypeTarget;
    int equipItemTarget;

    MenuState state;

    private void Update()
    {
        if (Input.GetButtonDown("Menu"))
        {
            if(state == MenuState.off)
            {
                state = MenuState.main;
                mainMenuPanel.SetActive(true);
                NewMainOptionTarget(mainOptionTarget);
                PlayerController.current.LockPlayer();
            }
            else
            {
                state = MenuState.off;
                DisableMenus();
                PlayerController.current.UnLockPlayer();
                return;
            }
        }
        int _input;
        switch (state)
        {
            case MenuState.main:
                #region
                _input = (int)WonderfulInput.WInput.x;
                if (_input != 0)
                {
                    int _newTarget = mainOptionTarget - _input;
                    if (_newTarget < 0) _newTarget = mainOptionText.Length - 1;
                    else if (_newTarget > mainOptionText.Length - 1) _newTarget = 0;

                    NewMainOptionTarget(_newTarget);
                }
                if (Input.GetButtonDown("Select"))
                {
                    mainMenuPanel.SetActive(false);
                    switch (mainOptionTarget)
                    {
                        default:
                        case 0:
                            inventoryPanel.SetActive(true);
                            LoadInventory();
                            NewItemTarget(itemTarget);
                            state = MenuState.inventory;
                            break;
                        case 1:
                            OpenEquipPanel();
                            NewEquipAllyTarget(0);
                            state = MenuState.equipAlly;
                            break;
                    }
                }
                break;
            #endregion
            case MenuState.inventory:
                #region
                _input = (int)WonderfulInput.WInput.y;
                if (_input != 0)
                {
                    int _newTarget = itemTarget - _input;
                    if (_newTarget < 0) _newTarget = loadedItemTexts.Count - 1;
                    else if (_newTarget > loadedItemTexts.Count - 1) _newTarget = 0;

                    NewItemTarget(_newTarget);
                }

                if (Input.GetButtonDown("Return"))
                {
                    mainMenuPanel.SetActive(true);
                    inventoryPanel.SetActive(false);
                    state = MenuState.main;
                }
                break;
            #endregion
            case MenuState.equipAlly:
                #region
                _input = (int)WonderfulInput.WInput.y;
                if (_input != 0)
                {
                    int _newTarget = equipAllyTarget - _input;
                    if (_newTarget < 0) _newTarget = AllyStatsManager.current.currentParty.Count - 1;
                    else if (_newTarget > AllyStatsManager.current.currentParty.Count - 1) _newTarget = 0;

                    NewEquipAllyTarget(_newTarget);
                }

                if (Input.GetButtonDown("Return"))
                {
                    mainMenuPanel.SetActive(true);
                    equipPanel.SetActive(false);
                    state = MenuState.main;
                }
                else if (Input.GetButtonDown("Select"))
                {
                    ShowAllyEquipment();
                    NewEquipTypeTarget(equipTypeTarget);
                    state = MenuState.equipType;
                    alliesToEquipTexts[equipAllyTarget].color = selectedColor;
                }
                break;
            #endregion
            case MenuState.equipType:
                #region
                _input = (int)WonderfulInput.WInput.y;
                if (_input != 0)
                {
                    int _newTarget = equipTypeTarget - _input;
                    if (_newTarget < 0) _newTarget = equipTypeTexts.Length - 1;
                    else if (_newTarget > equipTypeTexts.Length - 1) _newTarget = 0;

                    NewEquipTypeTarget(_newTarget);
                }
                if (Input.GetButtonDown("Return"))
                {
                    equipTypeBackdrop.SetActive(false);
                    NewEquipAllyTarget(equipAllyTarget);
                    state = MenuState.equipAlly;
                }
                else if (Input.GetButtonDown("Select"))
                {
                    LoadEquipmentItems();
                    equipTypeTexts[equipTypeTarget].color = selectedColor;
                    NewEquipItemTarget(0);
                    state = MenuState.equipItem;
                }
                break;
            #endregion
            case MenuState.equipItem:
                #region
                _input = (int)WonderfulInput.WInput.y;
                if (_input != 0)
                {
                    int _newTarget = equipItemTarget - _input;
                    if (_newTarget < 0) _newTarget = loadedItemTexts.Count - 1;
                    else if (_newTarget > loadedItemTexts.Count - 1) _newTarget = 0;

                    NewEquipItemTarget(_newTarget);
                }

                if (Input.GetButtonDown("Return"))
                {
                    equipContents.gameObject.SetActive(false);
                    NewEquipTypeTarget(equipTypeTarget);
                    state = MenuState.equipType;
                }
                else if (Input.GetButtonDown("Select"))
                {
                    EquipOnAlly();
                    equipContents.gameObject.SetActive(false);
                    ShowAllyEquipment();
                    NewEquipTypeTarget(equipTypeTarget);
                    state = MenuState.equipType;
                }
                break;
                #endregion
        }
    }
    void NewMainOptionTarget(int _target)
    {
        mainOptionText[mainOptionTarget].color = Color.white;
        mainOptionText[_target].color = selectColor;
        mainOptionTarget = _target;
    }
    void NewItemTarget(int _target)
    {
        loadedItemTexts[itemTarget].color = Color.white;
        loadedItemTexts[_target].color = selectColor;
        itemTarget = _target;

        itemImage.sprite = loadedItems[itemTarget].ItemSprite;
        itemName.text = loadedItems[itemTarget].ItemName;
        itemDescription.text = loadedItems[itemTarget].Description;
    }
    void NewEquipAllyTarget(int _target)
    {
        alliesToEquipTexts[equipAllyTarget].color = Color.white;
        alliesToEquipTexts[_target].color = selectColor;
        equipAllyTarget = _target;
    }
    void NewEquipTypeTarget(int _target)
    {
        equipTypeTexts[equipTypeTarget].color = Color.white;
        equipTypeTexts[_target].color = selectColor;
        equipTypeTarget = _target;
    }
    void NewEquipItemTarget(int _target)
    {
        loadedItemTexts[equipItemTarget].color = Color.white;
        loadedItemTexts[_target].color = selectColor;
        equipItemTarget = _target;
    }

    void DisableMenus()
    {
        inventoryPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        equipPanel.SetActive(false);
    }

    void LoadInventory()
    {
        foreach (var item in loadedItemTexts)
        {
            Destroy(item.gameObject);
        }
        loadedItemTexts.Clear();
        loadedItems.Clear();
        foreach (var item in AllyStatsManager.current.inventory)
        {
            loadedItems.Add(item);
        }
        for (int i = 0; i < loadedItems.Count; i++)
        {
            Item _item = loadedItems[i];

            Transform _itemText = Instantiate(itemTextPrefab).transform;
            _itemText.SetParent(inventoryContents);
            _itemText.transform.localScale = Vector3.one;
            _itemText.transform.localPosition = new Vector3(0f, i * -itemTextOffset);

            var _text = _itemText.GetComponent<ItemText>();
            _text.SetText(_item, AllyStatsManager.current.inventoryAmount[_item]);
            loadedItemTexts.Add(_text);
        }
    }

    void OpenEquipPanel()
    {
        equipPanel.SetActive(true);
        equipTypeBackdrop.SetActive(false);
        equipContents.gameObject.SetActive(false);

        foreach (var item in alliesToEquipTexts)
        {
            item.enabled = false;
        }

        for (int i = 0; i < AllyStatsManager.current.currentParty.Count; i++)
        {
            int _index = AllyStatsManager.current.currentParty[i];
            AllyStats _ally = AllyStatsManager.current.alliesStats[_index];
            alliesToEquipTexts[i].text = _ally.heroName;
            alliesToEquipTexts[i].enabled = true;
        }
    }
    void ShowAllyEquipment()
    {
        equipTypeBackdrop.SetActive(true);
        AllyStats _ally = AllyStatsManager.current.alliesStats[equipAllyTarget];

        if (_ally.equipedWeapon == null)
        {
            equipTypeTexts[0].text = "none";
            equipImages[0].enabled = false;
            equipBackdrops[0].SetActive(false);
        }
        else
        {
            equipTypeTexts[0].text = _ally.equipedWeapon.ItemName;
            equipImages[0].enabled = true;
            equipImages[0].sprite = _ally.equipedWeapon.ItemSprite;
            equipBackdrops[0].SetActive(true);
        }

        if (_ally.equipedArmor == null)
        {
            equipTypeTexts[1].text = "none";
            equipImages[1].enabled = false;
            equipBackdrops[1].SetActive(false);
        }
        else
        {
            equipTypeTexts[1].text = _ally.equipedArmor.ItemName;
            equipImages[1].enabled = true;
            equipImages[1].sprite = _ally.equipedArmor.ItemSprite;
            equipBackdrops[1].SetActive(true);
        }
    }
    void LoadEquipmentItems()
    {
        equipContents.gameObject.SetActive(true);
        foreach (var item in loadedItemTexts)
        {
            Destroy(item.gameObject);
        }
        loadedItemTexts.Clear();
        loadedItems.Clear();

        loadedItems.Add(null);

        foreach (var item in AllyStatsManager.current.inventory)
        {
            switch (equipTypeTarget)
            {
                case 0:
                    if (item as Weapon == null || 
                    !AllyStatsManager.current.alliesStats[equipAllyTarget].allowedWeaponTypes.Contains((item as Weapon).WeaponType)) continue;
                    break;
                case 1:
                    if (item as Armor == null) continue;
                    break;
            }

            loadedItems.Add(item);
        }

        for (int i = 0; i < loadedItems.Count; i++)
        {
            Item _item = loadedItems[i];

            Transform _itemText = Instantiate(itemTextPrefab).transform;
            _itemText.SetParent(equipContents);
            _itemText.transform.localScale = Vector3.one;
            _itemText.transform.localPosition = new Vector3(0f, i * -itemTextOffset);

            var _text = _itemText.GetComponent<ItemText>();
            if (_item == null) _text.SetNull();
            else _text.SetText(_item, AllyStatsManager.current.inventoryAmount[_item]);
            loadedItemTexts.Add(_text);
        }
    }
    void EquipOnAlly()
    {
        Item _oldEquip;
        Item _newEquip = loadedItems[equipItemTarget];

       
        switch (equipTypeTarget)
        {
            case 0://Weapons
                _oldEquip = AllyStatsManager.current.alliesStats[equipAllyTarget].equipedWeapon;
                if (_oldEquip != null) AllyStatsManager.current.AddItem(_oldEquip);
                if (_newEquip != null) AllyStatsManager.current.RemoveItem(_newEquip);
                AllyStatsManager.current.alliesStats[equipAllyTarget].equipedWeapon = _newEquip as Weapon;
                break;
            case 1://Armor
                _oldEquip = AllyStatsManager.current.alliesStats[equipAllyTarget].equipedArmor;
                if (_oldEquip != null) AllyStatsManager.current.AddItem(_oldEquip);
                if (_newEquip != null) AllyStatsManager.current.RemoveItem(_newEquip);
                AllyStatsManager.current.alliesStats[equipAllyTarget].equipedArmor = _newEquip as Armor;
                break;
        }
    }
    
}

public enum MenuState
{
    off,
    main,
    inventory,
    equipAlly,
    equipType,
    equipItem,
}
