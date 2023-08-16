using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BattleNumber : MonoBehaviour
{
    [SerializeField] Color[] textColors;
    [SerializeField] float force;
    [SerializeField] float lifeTime;
    [SerializeField] CanvasGroup group;

    TextMeshProUGUI text;
    Rigidbody2D rb;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        rb = GetComponent<Rigidbody2D>();
        Invoke(nameof(Fade), lifeTime / 2f);
    }

    void Fade()
    {
        LeanTween.alphaCanvas(group, 0f, lifeTime / 2f);
        Destroy(gameObject, lifeTime / 2f);
    }

    public void SetText(string _text, BattleNumberType _type)
    {
        text.text = _text;
        text.color = textColors[(int)_type];

        float _dir = Mathf.Sign(-transform.position.x);
        rb.velocity = new Vector2(Random.Range(0.3f, 1.5f)* _dir, 1f).normalized * Random.Range(force * 0.7f,force * 1.2f);
    }
}

public enum BattleNumberType
{
    AllyDamage,
    EnemyDamage,
}