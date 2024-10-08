using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Collection : MonoBehaviour
{
    [SerializeField] GameManager GM;
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        // passing in "this" as context additionally allows to 
        // click on the log to highlight the object where it came from
        Debug.Log("Currently hovering " + name, this);
    }

    public void ActivateLoadedNotes()
    {
        // Activates the notes in collectables in list
        // Loaded notes depend on what notes have been picked up
        foreach (int noteNum in GM.LoadFoundNotes())
        {
            transform.Find("Notes").GetChild(noteNum + 1).gameObject.SetActive(true);
        }
    }
}
