using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public const int fightScene = 1;
    public static SceneTransition current;
    public static int lastScene;

    Animator anim;
    

    private void Awake()
    {
        if(current == null)
        {
            anim = GetComponent<Animator>();
            current = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    int targetScene;
    void LoadScene()
    {
        SaveSystem.current.CaptureState();
        SceneManager.LoadScene(targetScene);
        anim.Play("Transition_Enter");
    }

    public void EnterBattleScene()
    {
        targetScene = fightScene;
        anim.Play("Transition_Exit");
        lastScene = SceneManager.GetActiveScene().buildIndex;
        PlayerController.current.SaveSpawnPos();
    }

    public void BackFromBattle()
    {
        targetScene = lastScene;
        anim.Play("Transition_Exit");
    }
}
