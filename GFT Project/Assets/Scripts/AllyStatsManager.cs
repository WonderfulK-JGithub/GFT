using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyStatsManager : MonoBehaviour
{
    public static AllyStatsManager current;

    [SerializeField] Item golonka;
    [SerializeField] Item gustavgeosskladadakkaa;
    [SerializeField] Ability a;
    [SerializeField] Ability b;
    [SerializeField] Ability k;

    public List<AllyStats> alliesStats;
    public List<Item> inventory = new();
    public Dictionary<Item,int> inventoryAmount = new();

    private void Awake()
    {
        if(current == null)
        {
            current = this;
            DontDestroyOnLoad(gameObject);
            GenerateAlliesStats();
            AddItem(golonka);
            AddItem(gustavgeosskladadakkaa);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void GenerateAlliesStats()
    {
        alliesStats = new();

        AllyStats _adam = new AllyStats
        {
            type = AllyType.adam,
            heroName = "Adam",
            maxHealth = 40,
            maxEnergy = 20,
            attackPower = 5,
            defense = 3,
            accuracy = 100,
            speed = 5,
            currentHealth = 40,
            currentEnergy = 20,
            lvl = 1,
            experienceToNextLevel = 100,
        };
        _adam.abilities.Add(a);
        _adam.abilities.Add(b);
        alliesStats.Add(_adam);

        AllyStats _gustav = new AllyStats
        {
            type = AllyType.gustav,
            heroName = "Gustav",
            maxHealth = 30,
            maxEnergy = 25,
            attackPower = 4,
            defense = 4,
            accuracy = 90,
            speed = 3,
            currentHealth = 30,
            currentEnergy = 25,
            lvl = 1,
            experienceToNextLevel = 100,
        };
        _gustav.abilities.Add(b);
        _gustav.abilities.Add(k);
        alliesStats.Add(_gustav);

        AllyStats _herman = new AllyStats
        {
            type = AllyType.herman,
            heroName = "Herman",
            maxHealth = 25,
            maxEnergy = 40,
            attackPower = 6,
            defense = 3,
            accuracy = 90,
            speed = 8,
            currentHealth = 25,
            currentEnergy = 40,
            lvl = 1,
            experienceToNextLevel = 100,
        };
        alliesStats.Add(_herman);
    }

    public void RemoveItem(Item _itemToRemove)
    {
        if (!inventory.Contains(_itemToRemove))
        {
            Debug.LogError("Removed a none existing item");
            return;
        }

        inventoryAmount[_itemToRemove]--;
        if (inventoryAmount[_itemToRemove] == 0)
        {
            inventory.Remove(_itemToRemove);
            inventoryAmount.Remove(_itemToRemove);
        }
    }

    public void AddItem(Item _itemToAdd)
    {
        if (inventory.Contains(_itemToAdd))
        {
            inventoryAmount[_itemToAdd]++;
        }
        else
        {
            inventory.Add(_itemToAdd);
            inventoryAmount.Add(_itemToAdd, 1);
        }
    }
}

public class AllyStats
{
    public AllyType type;
    public string heroName;

    public int maxHealth;
    public int maxEnergy;
    public int attackPower;
    public int defense;
    public int accuracy;
    public int speed;

    public int currentHealth;
    public int currentEnergy;

    public int lvl;
    public int experience;
    public int experienceToNextLevel;

    public List<Ability> abilities = new();
}

public enum AllyType
{
    adam,
    gustav,
    herman,
}