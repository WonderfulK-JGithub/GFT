using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealingItem", menuName = "Custom SO/HealingItem")]
public class HealingItem : Item
{
    [SerializeField] int healthGain;
    [SerializeField] int energyGain;
    [SerializeField] bool revive;
    [SerializeField] bool targetAll;

    public int HealthGain => healthGain;
    public int EnergyGain => energyGain;
    public bool Revive => revive;
    public bool TargetAll => targetAll;
}
