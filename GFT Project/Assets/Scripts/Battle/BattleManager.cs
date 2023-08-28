using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager current;

    public static Action OnEndSelection;

    [SerializeField] List<GameObject> allyPrefabs;
    [SerializeField] List<GameObject> enemyPrefabs;
    [SerializeField] Transform allyBasePos;
    [SerializeField] Transform enemyBasePos;

    //public static List<int> alliesToSpawn = new();
    public static Dictionary<Vector3,int> enemiesToSpawn = new();//enemies har inte bestämda positioner, därför används ett dictionary här (men inte för allies)

    List<IAllyable> alliesData = new();//om en ally dör finns den fortfarande kvar i denna lista

    List<IBattleable> allies = new();
    List<IBattleable> enemies = new();

    public List<IAllyable> AlliesData
    {
        get
        {
            return alliesData;
        }
    }
    public List<IBattleable> Allies
    {
        get
        {
            return allies;
        }
    }
    public List<IBattleable> Enemies
    {
        get
        {
            return enemies;
        }
    }

    List<IBattleable> battleQue = new();
    List<Image> queImages = new();

    public List<IBattleable> BattleQue
    {
        get
        {
            List<IBattleable> _newList = new();
            foreach (var item in battleQue)
            {
                _newList.Add(item);
            }
            return _newList;
        }
    }

    BattleState lastState;
    BattleState state;
    BattleState State
    {
        get
        {
            return state;
        }
        set
        {
            lastState = state;
            state = value;
        }
    }

    [Header("Menu & UI")]
    [SerializeField] GameObject[] allyStatPanels;
    [SerializeField] TMP_Text[] allyHealthTexts;
    [SerializeField] Slider[] allyHealthBars;
    [SerializeField] TMP_Text[] allyEnergyTexts;
    [SerializeField] Slider[] allyEnergyBars;
    [SerializeField] TMP_Text[] allyTiltTexts;
    [SerializeField] Slider[] allyTiltBars;

    [SerializeField] GameObject playerPanel;
    [SerializeField] TMP_Text[] mainOptions;
    [SerializeField] Color selectColor;
    [SerializeField] Color selectedColor;
    [SerializeField] Color unavailableColor;
    [SerializeField] Color selectUnavailableColor;
    [SerializeField] GameObject markerPrefab;

    [SerializeField] Transform turnOrderPanel;
    [SerializeField] GameObject emptyImagePrefab;
    [SerializeField] Transform startTurnPoint;
    [SerializeField] Transform endTurnPoint;
    [SerializeField] float queImageOffset;


    [Header("   Main Options")]
    int currentAlly;
    int mainOptionTarget;
    [HideInInspector] public int enemyTarget;
    List<SpriteRenderer> currentMarkers = new();
    [HideInInspector] public int allyTarget;


    [Header("   Item/Inventory")]
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject itemTextPrefab;
    [SerializeField] Transform inventoryContents;
    [SerializeField] float itemTextOffset;
    public GameObject battleItemIconPrefab;
    List<ItemText> itemTexts = new();
    List<Item> battleInventory = new();
    int itemTarget;
    [HideInInspector] public Item selectedItem;

    [Header("   Abilities")]
    [SerializeField] GameObject abilityPanel;
    [SerializeField] GameObject abilityTextPrefab;
    [SerializeField] GameObject energyTextPrefab;
    [SerializeField] Transform abilityContents;
    [SerializeField] float abilityTextOffset;
    List<TMP_Text> abilityTexts = new();
    List<TMP_Text> energyTexts = new();
    List<bool> canUseAbilities = new();
    List<List<Ability>> alliesBattleAbilities = new();
    int abilityTarget;
    [HideInInspector] public Ability selectedAbility;

    [SerializeField] GameObject battleNumberPrefab;
    [SerializeField] GameObject battleTextPrefab;

    private void Awake()
    {
        current = this;

        List<int> _alliesToSpawn = AllyStatsManager.current.currentParty;

        if(_alliesToSpawn.Count == 0)
        {
            _alliesToSpawn.Add(0);
            enemiesToSpawn.Add(Vector3.zero,0);
        }

        List<Vector3> _positions = new();

        switch (_alliesToSpawn.Count)
        {
            case 1:
            default:
                _positions.Add(allyBasePos.position);
                break;
            case 2:
                _positions.Add(allyBasePos.position + Vector3.up * 1.25f);
                _positions.Add(allyBasePos.position + Vector3.down * 1.25f);
                break;
            case 3:
                _positions.Add(allyBasePos.position + Vector3.up * 1.5f);
                _positions.Add(allyBasePos.position + Vector3.down * 0.5f);
                _positions.Add(allyBasePos.position + Vector3.down * 2.5f);
                break;
        }

        //skapar allies och enemies och lägger till dem i listorna
        for (int i = 0; i < _alliesToSpawn.Count; i++)
        {
            Component _ally = Instantiate(allyPrefabs[_alliesToSpawn[i]], _positions[i], Quaternion.identity).GetComponent(typeof(IBattleable));
            allies.Add(_ally as IBattleable);
            alliesData.Add(_ally as IAllyable);
            (_ally as IAllyable).SetStats(i, _alliesToSpawn[i]);
            allyStatPanels[i].SetActive(true);
            UpdateAllyStatPanel(i);

            List<Ability> _battleAbilities = new();
            foreach (var _ability in AllyStatsManager.current.alliesStats[_alliesToSpawn[i]].abilities)
            {
                if(_ability.NeededMembers == null)
                {
                    _battleAbilities.Add(_ability);
                    continue;
                }
                List<int> _neededMembers = _ability.NeededMembers;
                foreach (var _a in _alliesToSpawn)
                {
                    if (_neededMembers.Contains(_a)) _neededMembers.Remove(_a);
                }
                if(_neededMembers.Count == 0)
                {
                    _battleAbilities.Add(_ability);
                }
            }
            alliesBattleAbilities.Add(_battleAbilities);
        }
        foreach (var item in enemiesToSpawn)
        {
            IBattleable _enemy = Instantiate(enemyPrefabs[item.Value], enemyBasePos.position + item.Key, Quaternion.identity).GetComponent(typeof(IBattleable)) as IBattleable;
            enemies.Add(_enemy);
        }

        LoadBattleQue();
        battleQue[0].YourTurn();
    }

    private void Update()
    {
        int _input;
        switch (State)
        {
            case BattleState.mainSelect:
                #region
                _input = (int)WonderfulInput.WInput.x;
                if(_input != 0)
                {
                    int _newTarget = mainOptionTarget + _input;
                    if (_newTarget < 0) _newTarget = mainOptions.Length - 1;
                    else if (_newTarget > mainOptions.Length - 1) _newTarget = 0;

                    NewMainOptionTarget(_newTarget);
                }

                if (Input.GetButtonDown("Select"))
                {
                    switch (mainOptionTarget)
                    {
                        case 0://normal attack
                        default:
                            State = BattleState.selectSingleEnemy;
                            currentMarkers.Add(Instantiate(markerPrefab).GetComponent<SpriteRenderer>());
                            NewEnemyTarget(0);
                            OnEndSelection += alliesData[currentAlly].RegularAttack;
                            break;
                        case 1://abilities
                            State = BattleState.abilitySelect;
                            abilityPanel.SetActive(true);
                            LoadAbilityTexts();
                            abilityTarget = 0;
                            NewAbilityTarget(0);
                            break;
                        case 2://items
                            if(AllyStatsManager.current.inventory.Count == 0)
                            {
                                return;
                            }
                            State = BattleState.itemSelect;
                            inventoryPanel.SetActive(true);
                            LoadItemTexts();
                            itemTarget = 0;
                            NewItemTarget(0);
                            break;
                    }
                    mainOptions[mainOptionTarget].color = selectColor;
                }
                
                break;
                #endregion
            case BattleState.selectSingleEnemy:
                #region
                _input = (int)WonderfulInput.WInput.x;
                if (_input != 0)
                {
                    int _newTarget = enemyTarget + _input;
                    if (_newTarget < 0) _newTarget = enemies.Count - 1;
                    else if (_newTarget > enemies.Count - 1) _newTarget = 0;

                    NewEnemyTarget(_newTarget);
                }
                _input = (int)WonderfulInput.WInput.y;
                if (_input != 0)
                {
                    int _newTarget = enemyTarget + _input;
                    if (_newTarget < 0) _newTarget = enemies.Count - 1;
                    else if (_newTarget > enemies.Count - 1) _newTarget = 0;

                    NewEnemyTarget(_newTarget);
                }
                if (Input.GetButtonDown("Select"))
                {
                    Destroy(currentMarkers[0]);
                    currentMarkers.Clear();
                    OnEndSelection?.Invoke();
                    OnEndSelection = null;
                    playerPanel.SetActive(false);
                    State = BattleState.action;
                }
                else if (Input.GetButtonDown("Return"))
                {
                    BackFromTargeting();
                }
                break;
            #endregion
            case BattleState.abilitySelect:
                #region
                _input = (int)WonderfulInput.WInput.y;
                if (_input != 0)
                {
                    int _newTarget = abilityTarget + _input;
                    if (_newTarget < 0) _newTarget = abilityTexts.Count - 1;
                    else if (_newTarget > abilityTexts.Count - 1) _newTarget = 0;
                    NewAbilityTarget(_newTarget);
                }
                if (Input.GetButtonDown("Select"))
                {
                    if (canUseAbilities[abilityTarget])
                    {
                        AbilitySelected();
                        RemoveAbilityTexts();
                        abilityPanel.SetActive(false);
                    }
                }
                else if (Input.GetButtonDown("Return"))
                {
                    State = BattleState.mainSelect;
                    RemoveAbilityTexts();
                    abilityPanel.SetActive(false);
                    mainOptions[mainOptionTarget].color = selectColor;
                }
                break;
            #endregion
            case BattleState.itemSelect:
                #region
                _input = (int)WonderfulInput.WInput.y;
                if (_input != 0)
                {
                    int _newTarget = itemTarget + _input;
                    if (_newTarget < 0) _newTarget = itemTexts.Count - 1;
                    else if (_newTarget > itemTexts.Count - 1) _newTarget = 0;

                    NewItemTarget(_newTarget);
                }
                if (Input.GetButtonDown("Select"))
                {
                    ItemSelected();
                    RemoveItemTexts();
                    inventoryPanel.SetActive(false);
                }
                else if(Input.GetButtonDown("Return"))
                {
                    State = BattleState.mainSelect;
                    RemoveItemTexts();
                    inventoryPanel.SetActive(false);
                    mainOptions[mainOptionTarget].color = selectColor;
                }
                break;
            #endregion
            case BattleState.selectSingleAlly:
                #region
                _input = (int)WonderfulInput.WInput.y;
                if (_input != 0)
                {
                    int _newTarget = allyTarget + _input;
                    if (_newTarget < 0) _newTarget = allies.Count - 1;
                    else if (_newTarget > allies.Count - 1) _newTarget = 0;

                    NewAllyTarget(_newTarget);
                }
                if (Input.GetButtonDown("Select"))
                {
                    Destroy(currentMarkers[0]);
                    currentMarkers.Clear();
                    OnEndSelection?.Invoke();
                    OnEndSelection = null;
                    playerPanel.SetActive(false);
                    State = BattleState.action;
                }
                else if (Input.GetButtonDown("Return"))
                {
                    BackFromTargeting();
                }
                break;
            #endregion
            case BattleState.selectAnyAlly:
                #region
                _input = (int)WonderfulInput.WInput.y;
                if (_input != 0)
                {
                    int _newTarget = allyTarget + _input;
                    if (_newTarget < 0) _newTarget = alliesData.Count - 1;
                    else if (_newTarget > alliesData.Count - 1) _newTarget = 0;

                    NewAllyTarget(_newTarget,true);
                }
                if (Input.GetButtonDown("Select"))
                {
                    Destroy(currentMarkers[0]);
                    currentMarkers.Clear();
                    OnEndSelection?.Invoke();
                    OnEndSelection = null;
                    playerPanel.SetActive(false);
                    State = BattleState.action;
                }
                else if (Input.GetButtonDown("Return"))
                {
                    BackFromTargeting();
                }
                break;
                #endregion
        }

        for (int i = 0; i < queImages.Count; i++)
        {
            Vector3 _targetPos = endTurnPoint.position + i * queImageOffset * Vector3.right;
            queImages[i].transform.position = Vector3.Lerp(queImages[i].transform.position, _targetPos, 5f * Time.deltaTime);
        }
    }

    public void NewMainOptionTarget(int _newTarget)
    {
        foreach (var item in mainOptions)
        {
            item.color = Color.white;
        }
        if (AllyStatsManager.current.inventory.Count == 0)
        {
            if(_newTarget == 2)
            {
                mainOptions[2].color = selectUnavailableColor;
            }
            else
            {
                mainOptions[2].color = unavailableColor;
                mainOptions[_newTarget].color = selectColor;
            }
            
        }
        else
        {
            mainOptions[_newTarget].color = selectColor;
        }
        
        mainOptionTarget = _newTarget;
    }
    void NewEnemyTarget(int _newTarget)
    {
        enemyTarget = _newTarget;
        currentMarkers[0].transform.position = enemies[enemyTarget].GetGameObject().transform.position + enemies[enemyTarget].TargetBounds.center;
        currentMarkers[0].size = (Vector2)enemies[enemyTarget].TargetBounds.size + Vector2.one * 0.2f;
    }
    public void NewItemTarget(int _newTarget)
    {
        itemTexts[itemTarget].color = Color.white;
        itemTexts[_newTarget].color = selectColor;
        itemTarget = _newTarget;
    }
    void NewAllyTarget(int _newTarget,bool _targetAnyAlly = false)
    {
        allyTarget = _newTarget;
        if (_targetAnyAlly)
        {
            IBattleable _ally = alliesData[allyTarget].BattleData;
            currentMarkers[0].transform.position = _ally.GetGameObject().transform.position + _ally.TargetBounds.center;
            currentMarkers[0].size = (Vector2)_ally.TargetBounds.size + Vector2.one * 0.2f;
        }
        else
        {
            IBattleable _ally = allies[allyTarget];
            currentMarkers[0].transform.position = _ally.GetGameObject().transform.position + _ally.TargetBounds.center;
            currentMarkers[0].size = (Vector2)_ally.TargetBounds.size + Vector2.one * 0.2f;
        }
        
    }
    public void NewAbilityTarget(int _newTarget)
    {
        if (canUseAbilities[abilityTarget])
        {
            abilityTexts[abilityTarget].color = Color.white;
        }
        else
        {
            abilityTexts[abilityTarget].color = unavailableColor;
        }
        if (canUseAbilities[_newTarget])
        {
            abilityTexts[_newTarget].color = selectColor;
        }
        else
        {
            abilityTexts[_newTarget].color = selectUnavailableColor;
        }
        
        abilityTarget = _newTarget;
    }

    List<IBattleable> GetAllEnteties()
    {
        List<IBattleable> _list = new List<IBattleable>();
        foreach (var item in allies)
        {
            _list.Add(item);
        }
        foreach (var item in enemies)
        {
            _list.Add(item);
        }
        return _list;
    }

    void LoadBattleQue()
    {
        var _battleEnteties = GetAllEnteties();
        int _totalSpeed = 0;
        foreach (var _battleEntety in _battleEnteties)
        {
            _totalSpeed += _battleEntety.Speed;
        }

        while(_battleEnteties.Count > 0)
        {
            int _random = UnityEngine.Random.Range(0, _totalSpeed + 1);
            int _speedOffset = 0;
            for (int i = 0; i < _battleEnteties.Count; i++)
            {
                var _battleEntety = _battleEnteties[i];
                if(_battleEntety.Speed + _speedOffset >= _random)
                {
                    battleQue.Add(_battleEntety);
                    _battleEnteties.RemoveAt(i);
                    _totalSpeed -= _battleEntety.Speed;
                    Color _col;
                    if (enemies.Contains(_battleEntety))
                    {
                        _col = Color.red;
                    }
                    else
                    {
                        _col = Color.green;
                    }
                    Image _image = Instantiate(emptyImagePrefab,turnOrderPanel).GetComponent<Image>();
                    _image.transform.localScale = Vector3.one;
                    _image.color = _col;
                    _image.transform.position = startTurnPoint.position;
                    queImages.Add(_image);
                }
                else
                {
                    _speedOffset += _battleEntety.Speed;
                }
            }
        }
    }

    void EndBattle()
    {
        SceneTransition.current.BackFromBattle();
        enemiesToSpawn.Clear();
    }
    void GameOver()
    {
        print("rip");
    }

    public void TurnEnded()
    {
        battleQue.RemoveAt(0);
        Destroy(queImages[0].gameObject);
        queImages.RemoveAt(0);

        if(enemies.Count == 0)
        {
            EndBattle();
        }
        else if(allies.Count == 0)
        {
            GameOver();
        }
        else
        {
            if (battleQue.Count == 0)
            {
                LoadBattleQue();
            }
            battleQue[0].YourTurn();
        }
    }
    public void TurnsEnded(List<int> _queIndexes)
    {
        foreach (var item in _queIndexes)
        {
            print(item);
            battleQue.RemoveAt(item);
            Destroy(queImages[item].gameObject);
            queImages.RemoveAt(item);
        }

        if (enemies.Count == 0)
        {
            EndBattle();
        }
        else if (allies.Count == 0)
        {
            GameOver();
        }
        else
        {
            if (battleQue.Count == 0)
            {
                LoadBattleQue();
            }
            battleQue[0].YourTurn();
        }
    }
    public void StartSelection(int _ally)
    {
        NewMainOptionTarget(0);
        playerPanel.SetActive(true);
        State = BattleState.mainSelect;
        currentAlly = _ally;

        if(AllyStatsManager.current.inventory.Count == 0)
        {
            mainOptions[2].color = unavailableColor;
        }
    }
    public void UpdateAllyStatPanel(int _ally)
    {
        IAllyable _allyData = alliesData[_ally];
        allyHealthTexts[_ally].text = _allyData.Health.ToString() + " / " + _allyData.MaxHealth.ToString();
        allyHealthBars[_ally].value = _allyData.Health / (float)_allyData.MaxHealth;

        allyEnergyTexts[_ally].text = _allyData.Energy.ToString() + " / " + _allyData.MaxEnergy.ToString();
        allyEnergyBars[_ally].value = _allyData.Energy / (float)_allyData.MaxEnergy;

        allyTiltTexts[_ally].text = _allyData.Tilt.ToString() + "%";
        allyTiltBars[_ally].value = _allyData.Tilt / 100f;
    }
    public bool IsCurrentAlly(int _battleIndex)
    {
        return _battleIndex == currentAlly;
    }

    public void CreateBattleNumberText(Vector3 _position, string _text, BattleNumberType _type)
    {
        BattleNumber _number = Instantiate(battleNumberPrefab, _position, Quaternion.identity).GetComponent<BattleNumber>();
        _number.SetText(_text, _type);
    }
    public void CreateBattleText(Vector3 _position,BattleTextType _type)
    {
        BattleText _number = Instantiate(battleTextPrefab, _position, Quaternion.identity).GetComponent<BattleText>();
        _number.SetText(_type);
    }
    public void EnemyDefeated(IBattleable _enemy)
    {
        if (battleQue.Contains(_enemy))
        {
            int _index = battleQue.IndexOf(_enemy);
            Destroy(queImages[_index].gameObject);
            queImages.RemoveAt(_index);
            battleQue.Remove(_enemy);
        }
        
        enemies.Remove(_enemy);
    }
    public void AllyDefeated(IBattleable _ally)
    {
        if (battleQue.Contains(_ally))
        {
            int _index = battleQue.IndexOf(_ally);
            Destroy(queImages[_index].gameObject);
            queImages.RemoveAt(_index);
            battleQue.Remove(_ally);
        }

        allies.Remove(_ally);
    }
    public void AllyRevived(IBattleable _ally)
    {
        if (allies.Contains(_ally))
        {
            Debug.LogError("huh");
            return;
        }
        allies.Add(_ally);
    }

    void LoadItemTexts()
    {
        for (int i = 0; i < AllyStatsManager.current.inventory.Count; i++)
        {
            Item _item = AllyStatsManager.current.inventory[i];
            if (!_item.UseInBattle) continue;

            Transform _itemText = Instantiate(itemTextPrefab).transform;
            _itemText.SetParent(inventoryContents);
            _itemText.transform.localScale = Vector3.one;
            _itemText.transform.localPosition = new Vector3(0f, i * -itemTextOffset);

            var _text = _itemText.GetComponent<ItemText>();
            _text.SetText(_item, AllyStatsManager.current.inventoryAmount[_item]);
            itemTexts.Add(_text);
            battleInventory.Add(_item);
        }
    }
    void RemoveItemTexts()
    {
        foreach (var item in itemTexts)
        {
            Destroy(item.gameObject);
        }
        itemTexts.Clear();
        battleInventory.Clear();
    }
    void ItemSelected()
    {
        selectedItem = battleInventory[itemTarget];
        switch (selectedItem.TypeOfItem)
        {
            default:
                Debug.Log("This item ain't do shit");
                break;
            case ItemType.healing:
                HealingItem _heal = selectedItem as HealingItem;
                if (_heal.TargetAll)
                {
                    Debug.Log("Fixar det här senare");
                }
                else if (_heal.Revive)
                {
                    State = BattleState.selectAnyAlly;
                    currentMarkers.Add(Instantiate(markerPrefab).GetComponent<SpriteRenderer>());
                    NewAllyTarget(0,true);
                    OnEndSelection += alliesData[currentAlly].UseItem;
                }
                else
                {
                    State = BattleState.selectSingleAlly;
                    currentMarkers.Add(Instantiate(markerPrefab).GetComponent<SpriteRenderer>());
                    NewAllyTarget(0);
                    OnEndSelection += alliesData[currentAlly].UseItem;
                }
                break;
        }
    }

    void LoadAbilityTexts()
    {
        for (int i = 0; i < alliesBattleAbilities[currentAlly].Count; i++)
        {
            Ability _ability = alliesBattleAbilities[currentAlly][i];

            Transform _abilityText = Instantiate(abilityTextPrefab).transform;
            _abilityText.SetParent(abilityContents);
            _abilityText.transform.localScale = Vector3.one;
            _abilityText.transform.localPosition = new Vector3(0f, i * -abilityTextOffset);

            Transform _engergyText = Instantiate(energyTextPrefab).transform;
            _engergyText.SetParent(abilityContents);
            _engergyText.transform.localScale = Vector3.one;
            _engergyText.transform.localPosition = new Vector3(25f, i * -abilityTextOffset);

            var _text = _abilityText.GetComponent<TMP_Text>();
            var _text2 = _engergyText.GetComponent<TMP_Text>();

            _text.text = _ability.DisplayName;
            _text2.text = _ability.Energy.ToString();

            bool _hasMembers = false;
            if (_ability.NeededMembers == null)
            {
                _hasMembers = true;
            }
            else
            {
                List<int> _neededMembers = _ability.NeededMembers;
                foreach (var _a in battleQue)
                {
                    IAllyable _ally = (_a as IAllyable);
                    if (_ally == null) continue;
                    if (_ally.Tilt > 50) continue;
                    int _index = _ally.AllyIndex;
                    if (_neededMembers.Contains(_index)) _neededMembers.Remove(_index);
                }
                if (_neededMembers.Count == 0)
                {
                    _hasMembers = true;
                }
            }
           

            if (_ability.Energy <= alliesData[currentAlly].Energy && _hasMembers)
            {
                _text2.color = Color.blue;
                canUseAbilities.Add(true);
            }
            else
            {
                _text.color = selectUnavailableColor;
                _text2.color = selectUnavailableColor;
                canUseAbilities.Add(false);
            }
            
            abilityTexts.Add(_text);
            energyTexts.Add(_text2);
        }
    }
    void RemoveAbilityTexts()
    {
        foreach (var item in abilityTexts)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in energyTexts)
        {
            Destroy(item.gameObject);
        }
        abilityTexts.Clear();
        energyTexts.Clear();
        canUseAbilities.Clear();
    }
    void AbilitySelected()
    {
        selectedAbility = alliesBattleAbilities[currentAlly][abilityTarget];
        switch (selectedAbility.TargetType)
        {
            case TargetType.singleEnemy:
                State = BattleState.selectSingleEnemy;
                currentMarkers.Add(Instantiate(markerPrefab).GetComponent<SpriteRenderer>());
                NewEnemyTarget(0);
                OnEndSelection += alliesData[currentAlly].UseAbility;
                break;
            case TargetType.noTarget:
            default:
                alliesData[currentAlly].UseAbility();
                State = BattleState.action;
                playerPanel.SetActive(false);
                break;
        }
    }

    void BackFromTargeting()
    {
        Destroy(currentMarkers[0]);
        currentMarkers.Clear();
        OnEndSelection = null;
        switch (lastState)
        {
            case BattleState.mainSelect:
                State = BattleState.mainSelect;
                mainOptions[mainOptionTarget].color = selectColor;
                break;
            case BattleState.abilitySelect:
                State = BattleState.abilitySelect;
                abilityPanel.SetActive(true);
                LoadAbilityTexts();
                NewAbilityTarget(abilityTarget);
                break;
            case BattleState.itemSelect:
                State = BattleState.itemSelect;
                inventoryPanel.SetActive(true);
                LoadItemTexts();
                NewItemTarget(0);
                break;
        }
    }
}
public enum BattleState
{
    action,
    mainSelect,
    abilitySelect,
    itemSelect,
    selectSingleEnemy,
    selectSingleAlly,
    selectAnyAlly,
}

public interface IBattleable
{
    void YourTurn();
    void TakeDamage(int _damage);
    void Heal(int _amount);

    int Speed { get; }
    int Health { get; }

    Bounds TargetBounds { get; }

    GameObject GetGameObject();

}

public interface IAllyable
{
    void SetStats(int _battleIndex,int _allyIndex);
    void RegularAttack();
    void UseAbility();
    void UseItem();
    void Revive();

    public IBattleable BattleData { get; }
    int BattleIndex { get; set; }
    int AllyIndex { get; }
    int Health { get; }
    int MaxHealth { get; }
    int Energy { get; }
    int MaxEnergy { get; }
    int Tilt { get; }
}