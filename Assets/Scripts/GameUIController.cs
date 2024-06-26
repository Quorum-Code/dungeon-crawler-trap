using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [SerializeField] GameObject settings;

    [SerializeField] GameObject deathScreen;
    [SerializeField] TMP_Text deathText;

    [SerializeField] GameObject skillScreen;
    [SerializeField] TMP_Text skillPointText;
    [SerializeField] TMP_Text vitText;
    [SerializeField] TMP_Text dexText;

    [SerializeField] GameObject heartParent;
    [SerializeField] Image xpImage;

    [SerializeField] GameObject heartPrefab;

    [SerializeField] GameObject staminaParent;
    [SerializeField] GameObject staminaPrefab;
    [SerializeField] GameObject staminaProgress;
    [SerializeField] Color filledColor;
    Image staminaImage;

    [SerializeField] Sprite fullHeart;
    [SerializeField] Sprite emptyHeart;
    [SerializeField] Sprite[] xpBar;

    [SerializeField] Image blackout;

    int maxHealth = 0;
    int curHealth = 0;
    List<GameObject> hearts = new List<GameObject>();
    List<GameObject> stams = new List<GameObject>();

    PlayerPawn playerPawn;

    private void Start()
    {
        blackout.gameObject.SetActive(true);
        blackout.enabled = true;
    }

    public void Init(PlayerPawn playerPawn) 
    {
        this.playerPawn = playerPawn;

        SetMaxHealth(playerPawn.maxHealth);
        SetHealth(playerPawn.health);
        SetXp(playerPawn.xp);

        staminaImage = staminaProgress.GetComponent<Image>();
        SetStamina(playerPawn);
    }

    public void OpenSettings() 
    {
        Cursor.lockState = CursorLockMode.None;
        settings.SetActive(true);
    }

    public void OpenSkillMenu() 
    {
        // Load stats into screen
        skillScreen.SetActive(true);

        Cursor.lockState = CursorLockMode.None;

        LoadSkillStats();
    }

    public void LoadSkillStats() 
    {
        skillPointText.text = "Points available: " + playerPawn.skillPoints;
        vitText.text = "VIT: " + playerPawn.maxHealth;
        dexText.text = "DEX: " + playerPawn.maxStamina;
    }

    public void AddVit() 
    {
        playerPawn.AddVit();
        SetMaxHealth(playerPawn.maxHealth);
        SetHealth(playerPawn.health);
        LoadSkillStats();
    }

    public void AddDex() 
    {
        playerPawn.AddDex();
        SetStamina(playerPawn);
        LoadSkillStats();
    }

    public void CloseSkillMenu() 
    {
        skillScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenDeathScreen() 
    {
        Cursor.lockState = CursorLockMode.None;
        deathScreen.SetActive(true);
        deathText.text = gameController.GetLevelHint();
    }

    public void RestartLevelButton() 
    {
        deathScreen.SetActive(false);
        BlackoutScreen();
        gameController.RestartLevel();
    }

    public void SetMaxHealth(int max) 
    {
        maxHealth = max;

        if (hearts.Count < max)
        {
            GameObject g;
            for (int i = 0; hearts.Count < max; i++) 
            {
                g = Instantiate(heartPrefab);
                g.transform.SetParent(heartParent.transform);
                hearts.Add(g);
            }
        }
        else if (hearts.Count > max) 
        {
            GameObject g;
            while (hearts.Count > max) 
            {
                if (hearts.Count > 0) 
                {
                    g = hearts[hearts.Count - 1];
                    hearts.RemoveAt(hearts.Count - 1);
                    Destroy(g);
                }
            }
        }
    }

    public void SetHealth(int health) 
    {
        curHealth = health;
        for (int i = 0; i < maxHealth; i++) 
        {
            Image img = hearts[i].GetComponent<Image>();
            if (img == null)
                break;

            if (i < health)
            {
                img.sprite = fullHeart;
                hearts[i].transform.SetSiblingIndex(0);
            }
            else 
            {
                img.sprite = emptyHeart;
            }
        }
    }

    public void SetXp(int xp) 
    {
        xpImage.sprite = xpBar[xp % xpBar.Length];
    }

    public void UpdateStaminaProgress(PlayerPawn playerPawn) 
    {
        // Check if curStamina < maxStam
        if (playerPawn.curStamina < playerPawn.maxStamina)
        {
            // then check if need to enable image
            if (!staminaImage.enabled)
            {
                staminaImage.enabled = true;
            }

            // set progress
            staminaImage.fillAmount = playerPawn.toNextStamina / playerPawn.regenStaminaTime;

            if (playerPawn.curStamina < stams.Count)
            {
                staminaProgress.transform.position = stams[playerPawn.curStamina].transform.position;
            }
        }
        else 
        {
            if (staminaImage.enabled)
                staminaImage.enabled = false;
        }
    }

    public void SetStamina(PlayerPawn playerPawn) 
    {
        // add more stamina dots
        GameObject g;
        while (stams.Count < playerPawn.maxStamina) 
        {
            g = Instantiate(staminaPrefab);
            g.transform.SetParent(staminaParent.transform);
            stams.Add(g);
        }

        // color/arrange accordingly
        for (int i = 0; i < stams.Count; i++) 
        {
            Image img = stams[i].GetComponent<Image>();
            if (i < playerPawn.curStamina)
            {
                stams[i].transform.SetSiblingIndex(i);
                if (img != null)
                {
                    img.color = filledColor;
                }
            }
            else if(img != null) 
            {
                img.color = Color.white;
            }
        }
    }

    public void BlackoutScreen() 
    {
        // enable img
        blackout.enabled = true;

        // set opacity to max
        blackout.color = Color.black;
    }

    public void FadeScreenOut() 
    {
        StartCoroutine(FadeBlackout());
    }

    private IEnumerator FadeBlackout() 
    {
        blackout.gameObject.SetActive(true);
        blackout.enabled = true;

        float timer = 0;
        float total = 0.3f;
        while (timer < 0.3f) 
        {
            timer += Time.deltaTime;
            blackout.color = new Color(0, 0, 0, 1 - (timer/total));
            yield return null;
        }
        blackout.color = new Color(0, 0, 0, 0);
        blackout.enabled = false;
    }
}
