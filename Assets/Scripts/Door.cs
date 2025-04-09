using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject doorText;
    [SerializeField] private int keys = 1;
    [SerializeField] private Image blackScreen;

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory player = other.GetComponent<PlayerInventory>();

        if (player != null)
        {
            if (player.Key >= keys)
            {
                StartCoroutine(DeathCinematicAndLoadNextLevel());
            }
            else
            {
                StartCoroutine(ShowTextTemporarily());
            }
        }
    }

    private IEnumerator ShowTextTemporarily()
    {
        doorText.SetActive(true);
        yield return new WaitForSeconds(3f);
        doorText.SetActive(false);
    }

    private IEnumerator DeathCinematicAndLoadNextLevel()
    {
        blackScreen.gameObject.SetActive(true);

        float fadeDuration = 2f;
        float fadeTime = 0f;
        Color startColor = blackScreen.color;
        blackScreen.color = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (fadeTime < fadeDuration)
        {
            fadeTime += Time.deltaTime;
            blackScreen.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Clamp01(fadeTime / fadeDuration));
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
