using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    private bool gamePause = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (gamePause)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        gamePause = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        gamePause = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("BeginMenu");
    }
}
