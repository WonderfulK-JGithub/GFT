using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour,ICollideable,ISaveable
{

    int state;

    public void OnCollide()
    {
        BattleManager.enemiesToSpawn.Add(Vector3.up,0);
        BattleManager.enemiesToSpawn.Add(Vector3.down, 0);

        SceneTransition.current.EnterBattleScene();
        state = 1;
    }

    private void Start()
    {
        if(state == 1)
        {
            Destroy(gameObject);
        }
    }

    public object CaptureState()
    {
        return new SaveData
        {
            state = state,
        };
    }
    public void RestoreState(object _state)
    {
        state = ((SaveData)_state).state;
    }

    [System.Serializable]
    struct SaveData
    {
        public int state;
    }

}
