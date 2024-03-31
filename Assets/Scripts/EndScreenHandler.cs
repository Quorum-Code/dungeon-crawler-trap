using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenHandler : MonoBehaviour
{
    [SerializeField] Image blackout;
    [SerializeField] GameObject endParent;

    private void Start()
    {
        StartCoroutine(EndScene());
    }

    private IEnumerator EndScene() 
    {
        // Fade out blackout
        
        float total = 0f;
        float timer = 0.5f;
        while (total < timer) 
        {
            total += Time.deltaTime;
            blackout.color = new Color(0, 0, 0, 1-(total/timer));
            yield return null;
        }
        blackout.color = new Color(0, 0, 0, 0);

        // Wait
        total = 0f;
        timer = 1f;
        while (total < timer)
        {
            total += Time.deltaTime;
            yield return null;
        }

        // enable button stuffs
        endParent.SetActive(true);
    }

    public void BackToMainMenu() 
    {
        SceneManager.LoadScene(0);
    }
}
