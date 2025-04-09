using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class BeginMenu : MonoBehaviour
{
    [SerializeField] private GameObject beginMenu;
    [SerializeField] private GameObject settingsMenu;

    [Header("Audio")]
    [SerializeField] private EventReference buttonClickSound;
    [SerializeField] private EventReference openMenuSound;
    [SerializeField] private EventReference closeMenuSound;
    [SerializeField] private EventReference menuMusic;

    void Start()
    {
        AudioManager.instance.InitializeMusic(menuMusic);
    }

    public void Play()
    {
        AudioManager.instance.PlayOneShot(buttonClickSound, this.transform.position);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Settings() 
    {
        AudioManager.instance.PlayOneShot(openMenuSound, this.transform.position);
        beginMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void Quit()
    {
        AudioManager.instance.PlayOneShot(buttonClickSound, this.transform.position);
        Application.Quit();
    }

    public void ExitScreen() 
    {
        AudioManager.instance.PlayOneShot(closeMenuSound, this.transform.position);
        beginMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }
}
