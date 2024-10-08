using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTorch : Interactable
{
    private WallTorch wallTorchScript;
    private PlayerTorch playerTorchScript;

    private void Start()
    {
        wallTorchScript = GetComponent<WallTorch>();
        playerTorchScript = FindObjectOfType<PlayerTorch>();
    }
    public override void OffHover()
    {
        //Debug.Log("Stopped looking at Torch");
    }

    public override void OnHover()
    {
        //Debug.Log("Looking at Torch");
    }

    public override void OnInteract()
    {
        if (wallTorchScript.GetLight().enabled)
            playerTorchScript.Reignite();
        else
        {
            wallTorchScript.ActivateLight();
            playerTorchScript.Reignite();
        }
    }
}
