using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Image blackout;

    [SerializeField] GameObject Settings;
    [SerializeField] GameObject Credits;

    public void StartGame() 
    {
        StartCoroutine(FadeToStart());
    }

    private IEnumerator FadeToStart() 
    {
        float timer = 0f;
        float total = .3f;
        while (timer < total) 
        {
            timer += Time.deltaTime;
            blackout.color = new Color(0, 0, 0, timer/total);
            yield return null;
        }
        blackout.color = Color.black;
        SceneManager.LoadScene(1);
    }

    public void OpenSettings() 
    {
        Settings.SetActive(true);
    }

    public void CloseSettings() 
    {
        Settings.SetActive(false);
    }

    public void OpenCredits() 
    {
        Credits.SetActive(true);
    }

    public void CloseCredits() 
    {
        Credits.SetActive(false);
    }
}
