using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "Custom SO/Armor")]
public class Armor : Item
{
    [Header("Armor")]
    [SerializeField] int bonusHealth;
    [SerializeField] int bonusDefense;
    [SerializeField] int bonusSpeed;

    public int BonusHealth => bonusHealth;
    public int BonusDefense => bonusDefense;
    public int BonusSpeed => bonusSpeed;

    public Armor()
    {
        typeOfItem = ItemType.armor;
    }
}
