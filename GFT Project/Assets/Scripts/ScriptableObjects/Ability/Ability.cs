using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Custom SO/Ability")]
public class Ability : ScriptableObject
{
    [SerializeField] AbilityName abilityName;
    [SerializeField] string displayName;
    [SerializeField] int energy;
    [SerializeField] List<int> neededMembers;
    [SerializeField] TargetType targetType;

    public AbilityName AbilityName => abilityName;
    public string DisplayName => displayName;
    public int Energy => energy;
    public List<int> NeededMembers
    {
        get
        {
            List<int> _newList = new();
            foreach (var item in neededMembers)
            {
                _newList.Add(item);
            }
            return _newList;
        }
    }
    public TargetType TargetType => targetType;
}

public enum TargetType
{
    noTarget,
    singleEnemy,
    allEnemies,
    singleAlly,
    allAllies,
}

public enum AbilityName
{
    //adam
    HockeyTricks,
    Filosofi,

    //gustav
    GustavKladdkaka,

    //Herman
    CrossCheck,
}