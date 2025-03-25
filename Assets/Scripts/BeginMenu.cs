using UnityEngine;
using UnityEngine.SceneManagement;

public class BeginMenu : MonoBehaviour
{
    [SerializeField] private GameObject beginMenu;
    [SerializeField] private GameObject settingsMenu;

    void Start()
    {
        
    }

    public void Play()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
