using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu; // Reference to the main menu panel
    public GameObject optionsMenu; // Reference to the options menu panel
    public GameObject controlsMenu; // Reference to the controls menu panel
    public GameObject creditsMenu; // Reference to the credits menu panel

    private AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void StartGame()
    {
        audioManager.PlayButtonPress();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Load the next scene
    }

    public void OpenOptions()
    {
        audioManager.PlayMenuSwap();
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void QuitGame()
    {
        audioManager.PlayButtonPress();
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        audioManager.PlayMenuSwap();
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void OpenControls()
    {
        audioManager.PlayMenuSwap();
        mainMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void OpenCredits()
    {
        audioManager.PlayMenuSwap();
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }
}
