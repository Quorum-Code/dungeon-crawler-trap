using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] GameObject heartParent;
    [SerializeField] Image xpImage;

    [SerializeField] GameObject heartPrefab;

    [SerializeField] Sprite fullHeart;
    [SerializeField] Sprite emptyHeart;
    [SerializeField] Sprite[] xpBar;

    int maxHealth = 0;
    int curHealth = 0;
    List<GameObject> hearts = new List<GameObject>();

    public void Init(int maxHealth, int health, int xp) 
    {
        SetMaxHealth(maxHealth);
        SetHealth(health);
        SetXp(xp);
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
}
