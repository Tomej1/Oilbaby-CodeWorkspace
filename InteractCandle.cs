using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractCandle : Interactable
{
    [SerializeField] CampRoom campScript;
    float startLT;

    private void Start()
    {
        transform.GetChild(3).GetComponent<Light>().enabled = false;
        var ps = transform.GetChild(2).GetComponent<ParticleSystem>().main;
        startLT = ps.startLifetimeMultiplier;
        ps.startLifetimeMultiplier = 0f;
    }

    public override void OffHover()
    {
        
    }

    public override void OnHover()
    {
        
    }

    public override void OnInteract()
    {
        var ps = transform.GetChild(2).GetComponent<ParticleSystem>().main;
        ps.startLifetime = startLT;
        transform.GetChild(3).GetComponent<Light>().enabled = true;
        campScript.LightCandle((name == "Left_Candle") ? "left" : "right");
    }
}
