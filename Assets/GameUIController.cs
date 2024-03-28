using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] GameObject heartParent;
    [SerializeField] Image xpImage;

    [SerializeField] GameObject heartPrefab;

    [SerializeField] GameObject staminaParent;
    [SerializeField] GameObject staminaPrefab;
    [SerializeField] GameObject staminaProgress;
    [SerializeField] Color filledColor;
    Image staminaImage;
    Transform nextStaminaTransform;

    [SerializeField] Sprite fullHeart;
    [SerializeField] Sprite emptyHeart;
    [SerializeField] Sprite[] xpBar;

    int maxHealth = 0;
    int curHealth = 0;
    List<GameObject> hearts = new List<GameObject>();
    List<GameObject> stams = new List<GameObject>();

    public void Init(PlayerPawn playerPawn) 
    {
        SetMaxHealth(playerPawn.maxHealth);
        SetHealth(playerPawn.health);
        SetXp(playerPawn.xp);

        staminaImage = staminaProgress.GetComponent<Image>();
        SetStamina(playerPawn);
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
        xpImage.sprite = xpBar[xp];
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
}
