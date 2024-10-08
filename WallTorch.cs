using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTorch : MonoBehaviour
{
    [SerializeField] private int uses = 1;
    [SerializeField] ParticleSystem wallTorch;
    AudioSource audioSource;

    private Light light;
    float startIntensity;
    float startVolume;
    float startROT;
    private float duration = 20f;


    // Start is called before the first frame update
    private void Awake()
    {
        audioSource = transform.GetChild(1).GetComponent<AudioSource>();
        wallTorch = GetComponent<ParticleSystem>();
        var em = wallTorch.emission;
        startROT = em.rateOverTimeMultiplier;
        em.rateOverTime = 0f;
        wallTorch.Stop();
    }
    void Start()
    {
        startVolume = audioSource.volume;
        audioSource.Stop();

        light = transform.GetChild(0).GetComponent<Light>();
        startIntensity = light.intensity;
        light.intensity = 0f;

        light.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // The torch will lose light over time
        if (light.gameObject.activeInHierarchy && light.intensity > 0)
        {
            light.intensity -= (startIntensity / duration) * Time.deltaTime;
            audioSource.volume -= (startVolume / duration) * Time.deltaTime;

            var em = wallTorch.emission;
            em.rateOverTimeMultiplier -= (startROT / duration) * Time.deltaTime;
        }
        else if (light.gameObject.activeInHierarchy)
        {
            light.enabled = false;
            wallTorch.Stop();
        }
    }

    public void ActivateLight()
    {
        // If the torch still has uses left the player can reignite the walltorch
        if (uses > 0)
        {
            light.enabled = true;
            wallTorch.Play();
            audioSource.Play();
            light.intensity = startIntensity;
            var em = wallTorch.emission;
            em.rateOverTime = startROT;
            uses--;
        }
    }

    public Light GetLight()
    {
        return light;
    }
}
