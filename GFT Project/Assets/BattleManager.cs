using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] List<GameObject> allyPrefabs;
    [SerializeField] List<GameObject> enemyPrefabs;
    [SerializeField] Transform[] allyPositions;
    [SerializeField] Transform[] enemyPositions;

    public static List<int> alliesToSpawn = new();
    public static List<int> enemiesToSpawn = new();

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

    private void Awake()
    {
        //skapar allies och enemies och lägger till dem i listorna
        for (int i = 0; i < alliesToSpawn.Count; i++)
        {
            IBattleable _ally = Instantiate(allyPrefabs[alliesToSpawn[i]], allyPositions[i].position, Quaternion.identity).GetComponent(typeof(IBattleable)) as IBattleable;
            allies.Add(_ally);
        }
        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            IBattleable _enemy = Instantiate(enemyPrefabs[enemiesToSpawn[i]], enemyPositions[i].position, Quaternion.identity).GetComponent(typeof(IBattleable)) as IBattleable;
            enemies.Add(_enemy);
        }

        LoadBattleQue();
        battleQue[0].YourTurn();
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
            int _random = Random.Range(0, _totalSpeed + 1);
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

    public void TurnEnded()
    {
        battleQue.RemoveAt(0);
        if(battleQue.Count == 0)
        {
            LoadBattleQue();
        }
        battleQue[0].YourTurn();
    }
}

public interface IBattleable
{
    void YourTurn();
    void TakeDamage(int _damage);

    int Speed { get; }
}
