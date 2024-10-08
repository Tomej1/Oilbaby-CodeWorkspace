using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPerma : Interactable
{
    [SerializeField] private int maxUses = 2;
    private int uses;
    private PlayerTorch playerTorchScript;
    private GameObject fireParent;

    private List<float> startSize = new List<float>();
    private List<float> startLifeTime = new List<float>();
    private List<float> startRateOverTime = new List<float>();
    private float startIntensity;
    private void Start()
    {
        playerTorchScript = FindObjectOfType<PlayerTorch>();
        fireParent = transform.Find("FireParent").gameObject;
        uses = maxUses;

        startIntensity = fireParent.GetComponent<Light>().intensity;

        for (int i = 0; i < fireParent.transform.childCount; i++)
        {
            var ps = fireParent.transform.GetChild(i).GetComponent<ParticleSystem>().main;
            startLifeTime.Add(ps.startLifetimeMultiplier);
            startSize.Add(ps.startSizeMultiplier);

            var em = fireParent.transform.GetChild(i).GetComponent<ParticleSystem>().emission;
            startRateOverTime.Add(em.rateOverTimeMultiplier);
        }

    }

    public override void OffHover()
    {
        
    }

    public override void OnHover()
    {
        
    }

    public override void OnInteract()
    {
        if (uses > 0)
        {
            playerTorchScript.Reignite();
            uses--;
            fireParent.GetComponent<Light>().intensity -= startIntensity / maxUses;

            for (int i = 0; i < fireParent.transform.childCount; i++)
            {
                var ps = fireParent.transform.GetChild(i).GetComponent<ParticleSystem>().main;
                ps.startLifetimeMultiplier -= startLifeTime[i] / maxUses;
                ps.startSizeMultiplier -= startSize[i] / maxUses;

                var em = fireParent.transform.GetChild(i).GetComponent<ParticleSystem>().emission;
                em.rateOverTimeMultiplier -= startRateOverTime[i] / maxUses;
            }

        }
    }
}
