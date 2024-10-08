using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent enemy;
    [SerializeField] Transform playerTarget;
    PlayerTorch playerTorch;
    AudioSource monsterSound;
    AudioSource cryingSound;

    float volume;
    float distanceToPlayer;

    private void Start()
    {
        enemy = GetComponent<NavMeshAgent>();
        monsterSound = GetComponent<AudioSource>();
        volume = monsterSound.volume;
        playerTorch = playerTarget.GetComponent<PlayerTorch>();
        monsterSound.volume = 0;

        cryingSound = transform.Find("CryingSound").GetComponent<AudioSource>();
        cryingSound.Stop();
    }
    private void Update()
    {
        // The AI will not move before the player has exited the tutorial
        if (playerTarget.GetComponent<PlayerController>().GetTutorialState())
        {
            // Make the AI only move up to the player depending on the Torch light range
            // The monster sound also starts
            monsterSound.volume = volume;

            distanceToPlayer = playerTorch.GetLightRange();
            var offsetDirection = (playerTarget.transform.position - enemy.transform.position).normalized;
            enemy.destination = (playerTarget.position - offsetDirection * distanceToPlayer);
            
            if(distanceToPlayer > 3)
            {
                cryingSound.Play();
            }
        }
    }
}
