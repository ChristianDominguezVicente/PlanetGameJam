using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoreTextScript : MonoBehaviour
{
    [SerializeField] float seconds;
    [SerializeField] GameObject ContinueText;
    private bool canClick;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canClick = false;
        StartCoroutine(EnableClick());
    }

    private void Update()
    {
        if (canClick && Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        }
    }

    private IEnumerator EnableClick()
    {
        yield return new WaitForSeconds(seconds);
        canClick = true;
        ContinueText.SetActive(true);
    }

}

   

