using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleText : MonoBehaviour
{
    [SerializeField] Color[] textColors;
    [SerializeField] float hoverSpeed;
    [SerializeField] float lifeTime;
    [SerializeField] CanvasGroup group;

    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        Invoke(nameof(Fade), lifeTime / 2f);
    }

    void Fade()
    {
        LeanTween.alphaCanvas(group, 0f, lifeTime / 2f);
        Destroy(gameObject, lifeTime / 2f);
    }

    public void SetText(BattleTextType _type)
    {
        switch (_type)
        {
            case BattleTextType.Nice:
            default:
                text.text = "Nice";
                break;
            case BattleTextType.Blocked:
                text.text = "Blocked";
                break;
            case BattleTextType.Miss:
                text.text = "Missed";
                break;
            case BattleTextType.KO:
                text.text = "KO";
                break;
        }
        text.color = textColors[(int)_type];
    }

    private void FixedUpdate()
    {
        transform.position += hoverSpeed * Time.deltaTime * Vector3.up;
    }
}

public enum BattleTextType
{
    Nice,
    Blocked,
    Miss,
    KO,
}
