using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.Universal.Internal;

public class PlayerTorch : MonoBehaviour
{
    [SerializeField] private bool testing = false;
    [SerializeField] private Camera cam;

    GameManager GM;

    AudioSource audioSource;
    private Light wallTorchLight;

    private Light light;
    private float duration = 15f;

    // Information for the torch
    // Values for the torch diffrent properties
    private float startIntensity;
    private float startRange;
    private float startSize;
    private float startLifeTime;
    private float startRateOverTime;
    private float startVolume;

    private bool safe = true; // Default: true

    [SerializeField] GameObject playerTorch;
    ParticleSystem torchParticle;

    // Start is called before the first frame update
    void Start()
    {
        GM = FindObjectOfType<GameManager>();
        audioSource = playerTorch.transform.GetChild(1).GetComponent<AudioSource>();
        light = playerTorch.transform.GetChild(0).GetComponent<Light>();
        torchParticle = playerTorch.GetComponent<ParticleSystem>();

        // Attaching start values to variables
        startIntensity = light.intensity;
        startVolume = audioSource.volume;
        startRange = light.range;

        // Getting the information from the particle system
        var tp = torchParticle.main;
        startLifeTime = tp.startLifetimeMultiplier;
        startSize = tp.startSizeMultiplier;

        // Getting information about particle system emission
        var em = torchParticle.emission;
        startRateOverTime = em.rateOverTimeMultiplier;

        if (!testing)
        {
            // The torch is unlit at the start of the game
            light.intensity = 0;
            audioSource.volume = 0;
            light.range = 0;
            em.rateOverTimeMultiplier = 0;
            tp.startLifetimeMultiplier = 0;
            tp.startSizeMultiplier = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!testing)
        {
            if (light.intensity > 0)
            {
                // Make the light and the range of the light lower
                // Also lower the sound of the Torch over time
                audioSource.volume -= (startVolume / duration) * Time.deltaTime;
                light.intensity -= (startIntensity / duration) * Time.deltaTime;
                light.range -= (startRange / (duration) ) * Time.deltaTime;

                // Variable to handle Torchparticle.main
                var tp = torchParticle.main;
                // storlek Start size is reduced
                tp.startSizeMultiplier -= (startSize / duration) * Time.deltaTime;

                // Start life time is reduced
                tp.startLifetimeMultiplier -= (startLifeTime / duration) * Time.deltaTime;

                // Variable to handle Torchparticle.emission
                // Rate over time is reduced
                var em = torchParticle.emission;
                em.rateOverTimeMultiplier -= (startRateOverTime / duration) * Time.deltaTime;
            }
            if (light.intensity <= 0 && !safe)
            {
                // Check conditions for walltorch light, if the player should die or not
                if ((wallTorchLight != null && wallTorchLight.intensity <= 0) || wallTorchLight == null)
                {
                    audioSource.volume = 0;
                    GM.DeathScreen();
                }
            }
        }
    }

    public void Reignite()
    {
        // When the torch is reignited the torch info
        // is reset to the starting values

        audioSource.volume = startVolume;
        light.intensity = startIntensity;
        light.range = startRange;

        var tp = torchParticle.main;
        tp.startSizeMultiplier = startSize;
        tp.startLifetimeMultiplier = startLifeTime;

        var em = torchParticle.emission;
        em.rateOverTimeMultiplier = startRateOverTime;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("ToriExit"))
        {
            // The player is no longer safe
            safe = false;

            // Torch lights up
            Reignite();

            // Tori room spotlight turns red
            col.transform.GetChild(0).GetComponent<Light>().color = Color.red;

            // Stones falling to block the door
            col.transform.GetChild(1).gameObject.SetActive(true);

            // The light in the tori room dissapear
            
        }
        if (col.CompareTag("CampTorch"))
        {
            wallTorchLight = col.transform.Find("FireParent").GetComponent<Light>();
        }

        if (col.CompareTag("WallTorch"))
        {
            wallTorchLight = col.transform.GetChild(0).GetComponent<Light>();
        }

        if (col.CompareTag("BabyGrove"))
            safe = true;
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("WallTorch"))
        {
            wallTorchLight = null;
        }
        if (col.CompareTag("CampTorch"))
        {
            // When the player exits the survival camp
            wallTorchLight = null;
        }

        if (col.CompareTag("BabyGrove"))
            safe = false;
    }

    public float GetLightRange()
    {
        // Enemy calls for the light range to decide
        // how close it can come to the player
        return light.range;
    }
}
