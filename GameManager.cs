using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    float restartDelay = 0f;
    bool playerHasDied;

    [SerializeField] GameObject deathScreen;

    private void Awake()
    {
        // When the player starts over the bool needs to be set to false again
        playerHasDied = false;
    }
    public void DeathScreen()
    {
        // When the player dies the deathscreen is shown
        Cursor.lockState = CursorLockMode.Confined;
        deathScreen.SetActive(true);
    }

    public void PlayerDeath()
    {
        // When the player presses Play Again in deathscreen
        // Respawn the player from the start. Could reload the scene
        if (!playerHasDied)
        {
            // Game Restarts after a certain delay
            playerHasDied = true;
            Invoke("Restart", restartDelay);
        }
    }
    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public List<int> LoadFoundNotes()
    {
        // Loads in the saved notes into the collection
        string filePath = Application.persistentDataPath + "/NotesData.txt";
        if (!System.IO.File.Exists(filePath))
            System.IO.File.WriteAllText(filePath, string.Empty);

        string data = System.IO.File.ReadAllText(filePath);
        List<int> notes = new List<int>();

        if (data.Split(",")[0] != "")
            foreach (string n in data.Split(","))
            {
                notes.Add(int.Parse(n));
            }

        return notes;
    }

    public void SaveNote(int num)
    {
        // Save the collectables in a list
        // then push the list into a file
        List<int> notes = LoadFoundNotes();
        if (!notes.Contains(num))
        {
            string filePath = Application.persistentDataPath + "/NotesData.txt";
            notes.Add(num);
            notes.Sort();
            string data = string.Join(",", notes);
            System.IO.File.WriteAllText(filePath, data);
        }
    }

}
