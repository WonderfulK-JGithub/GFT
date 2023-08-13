using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public const int fightScene = 1;
    public static SceneTransition current;

    Animator anim;
    

    private void Awake()
    {
        anim = GetComponent<Animator>();
        current = this;
    }

    int targetScene;
    void LoadScene()
    {
        SceneManager.LoadScene(targetScene);
    }

    public void EnterBattleScene()
    {
        targetScene = fightScene;
        anim.Play("Transition_Exit");
    }
}
