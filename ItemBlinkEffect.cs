using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBlinkEffect : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] Color startColor;
    [SerializeField] Color endColor;

    [Range(0.1f, 1)]
    [SerializeField] float speed = 0.2f;
    private float length = 1f;

    Renderer ren;

    private void Awake()
    {
        ren = GetComponent<Renderer>();
    }
    void Update()
    {
        ren.material.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, length));
    }
}
