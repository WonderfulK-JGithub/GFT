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
    [SerializeField] Transform[] allyPositions;
    [SerializeField] Transform enemyBasePos;

    public static List<int> alliesToSpawn = new();
    public static Dictionary<Vector3,int> enemiesToSpawn = new();//enemies har inte bestämda positioner, därför används ett dictionary här (men inte för allies)

    List<IAllyable> alliesData = new();

    List<IBattleable> allies = new();
    List<IBattleable> enemies = new();
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

    BattleState state;

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
    [SerializeField] GameObject markerPrefab;

    int currentAlly;
    int mainOptionTarget;
    [HideInInspector] public int enemyTarget;
    List<SpriteRenderer> currentMarkers = new();

    [SerializeField] GameObject battleNumberPrefab;
    [SerializeField] GameObject battleTextPrefab;

    private void Awake()
    {
        current = this;

        if(alliesToSpawn.Count == 0)
        {
            alliesToSpawn.Add(0);
            enemiesToSpawn.Add(Vector3.zero,0);
        }

        //skapar allies och enemies och lägger till dem i listorna
        for (int i = 0; i < alliesToSpawn.Count; i++)
        {
            Component _ally = Instantiate(allyPrefabs[alliesToSpawn[i]], allyPositions[i].position, Quaternion.identity).GetComponent(typeof(IBattleable));
            allies.Add(_ally as IBattleable);
            alliesData.Add(_ally as IAllyable);
            (_ally as IAllyable).SetStats(i);
            allyStatPanels[i].SetActive(true);
            UpdateAllyStatPanel(i);
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
        switch (state)
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
                        case 0:
                        default:
                            state = BattleState.selectSingleTarget;
                            currentMarkers.Add(Instantiate(markerPrefab).GetComponent<SpriteRenderer>());
                            NewEnemyTarget(0);
                            OnEndSelection += alliesData[currentAlly].RegularAttack;
                            break;
                    }
                    mainOptions[mainOptionTarget].color = Color.gray;
                }
                
                break;
                #endregion
            case BattleState.selectSingleTarget:
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
                    state = BattleState.action;
                }
                else if (Input.GetButtonDown("Return"))
                {
                    state = BattleState.mainSelect;
                    Destroy(currentMarkers[0]);
                    currentMarkers.Clear();
                    OnEndSelection = null;
                    mainOptions[mainOptionTarget].color = selectColor;
                }
                break;
                #endregion
        }
    }

    public void NewMainOptionTarget(int _newTarget)
    {
        foreach (var item in mainOptions)
        {
            item.color = Color.white;
        }
        mainOptions[_newTarget].color = selectColor;
        mainOptionTarget = _newTarget;
    }
    void NewEnemyTarget(int _newTarget)
    {
        enemyTarget = _newTarget;
        currentMarkers[0].transform.position = enemies[enemyTarget].GetGameObject().transform.position + enemies[enemyTarget].TargetBounds.center;
        currentMarkers[0].size = (Vector2)enemies[enemyTarget].TargetBounds.size + Vector2.one * 0.2f;
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
        alliesToSpawn.Clear();
        enemiesToSpawn.Clear();
    }

    public void TurnEnded()
    {
        battleQue.RemoveAt(0);

        if(enemies.Count == 0)
        {
            EndBattle();
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
        state = BattleState.mainSelect;
        currentAlly = _ally;
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
        if (battleQue.Contains(_enemy)) battleQue.Remove(_enemy);
        enemies.Remove(_enemy);
    }
}
public enum BattleState
{
    action,
    mainSelect,
    selectSingleTarget,
}

public interface IBattleable
{
    void YourTurn();
    void TakeDamage(int _damage);

    int Speed { get; }
    int Health { get; }

    Bounds TargetBounds { get; }

    GameObject GetGameObject();

}

public interface IAllyable
{
    void SetStats(int _index);
    void RegularAttack();

    int Index { get; set; }
    int Health { get; }
    int MaxHealth { get; }
    int Energy { get; }
    int MaxEnergy { get; }
    int Tilt { get; }
}