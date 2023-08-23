using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleIndicator : MonoBehaviour
{
    public const float circleTime = 0.5f;
    [SerializeField] float radius;

    [SerializeField] Transform circle;
    [SerializeField] Transform mask;

    private void Awake()
    {
        startScale = circle.localScale.x;
    }

    float startScale;
    float timer;
    private void Update()
    {
        timer += Time.deltaTime;
        timer = Mathf.Clamp(timer, 0f, circleTime);

        float _scale = Mathf.Lerp(startScale, 0f, timer / circleTime);
        circle.localScale = Vector3.one * _scale;
        mask.localScale = Vector3.one * Mathf.Clamp(_scale - radius, 0f, startScale);

        if (timer == circleTime) Destroy(gameObject);
    }
}
