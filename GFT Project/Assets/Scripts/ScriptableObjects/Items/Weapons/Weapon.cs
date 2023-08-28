using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseItem", menuName = "Custom SO/Weapon")]
public class Weapon : Item
{
    [Header("Weapon")]
    [SerializeField] WeaponType weaponType;
    [SerializeField] int bonusAttackPower;
    [SerializeField] int bonusEnergy;
    [SerializeField] int bonusSpeed;

    public WeaponType WeaponType => weaponType;
    public int BonusAttackPower => bonusAttackPower;
    public int BonusEnergy => bonusEnergy;
    public int BonusSpeed => bonusSpeed;

    public Weapon()
    {
        typeOfItem = ItemType.weapon;
    }
}

public enum WeaponType
{
    club,//adam, herman och pontus vapen
    stick,//gustavs vapen
}
