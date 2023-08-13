using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WonderfulInput : MonoBehaviour
{
    static WonderfulInput current;
    static Vector2 wInput;
    public static Vector2 WInput
    {
        get
        {
            return wInput;
        }
    }

    Vector2 currentInput;
    Vector2 inputTimer;

    [SerializeField] float waitTime;
    [SerializeField] float bufferTime;

    private void Awake()
    {
        if(current == null)
        {
            current = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        print(wInput.x);

        Vector2 _newInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        if(_newInput.x != currentInput.x)
        {
            wInput.x = _newInput.x;
            inputTimer.x = waitTime;
            currentInput.x = _newInput.x;
        }
        else if(inputTimer.x <= 0f)
        {
            wInput.x = currentInput.x;
            inputTimer.x = bufferTime;
        }
        else
        {
            wInput.x = 0f;
        }

        if (_newInput.y != currentInput.y)
        {
            wInput.y = _newInput.y;
            inputTimer.y = waitTime;
            currentInput.y = _newInput.y;
        }
        else if (inputTimer.y <= 0f)
        {
            wInput.y = currentInput.y;
            inputTimer.y = bufferTime;
        }
        else
        {
            wInput.y = 0f;
        }

        inputTimer.x -= Time.deltaTime;
        inputTimer.y -= Time.deltaTime;
    }
}
