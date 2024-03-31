using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] MapConfig mapConfig;

    [SerializeField] PlayerController playerController;
    [SerializeField] GameUIController guic;
    GameMap gm;

    int level = 0;
    public string[] levelHints; 

    int lastMaxHp;
    int lastCurHp;
    int lastMaxStamina;
    int lastXp;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 100;

        gm = new GameMap(mapConfig);
        gm.endFound = FoundEnd;
        gm.isLoading = true;
        StartCoroutine(gm.LoadDungeon(mapConfig.DungeonByLevel(level)));
        StartCoroutine(WaitForLoading());
    }

    public string GetLevelHint() 
    {
        if (levelHints.Length > level) 
        {
            return levelHints[level];
        }
        return "Keep trying.";
    }

    public void RestartLevel() 
    {
        StartCoroutine(gm.LoadDungeon(mapConfig.DungeonByLevel(level)));
        StartCoroutine(WaitForLoading());
        LoadLastPlayerStats();
    }

    public void NextLevel() 
    {
        level++;
        gm.isLoading = true;
        MapConfig.DungeonLayout dl = mapConfig.DungeonByLevel(level);

        if (dl == null) 
        {
            SceneManager.LoadScene(2);
            return;
        }

        StartCoroutine(gm.LoadDungeon(dl));
        StartCoroutine(WaitForLoading());
    }

    private IEnumerator WaitForLoading() 
    {
        while (gm.isLoading) 
        {
            yield return null;
        }
        playerController.Ready();
        GetPlayerStats();
        guic.FadeScreenOut();
    }

    private void GetPlayerStats() 
    {
        if (playerController.playerPawn == null)
            return;
        else 
        {
            lastMaxHp = playerController.playerPawn.maxHealth;
            lastCurHp = playerController.playerPawn.health;
            lastMaxStamina = playerController.playerPawn.maxStamina;
            lastXp = playerController.playerPawn.xp;
        }
    }

    private void LoadLastPlayerStats() 
    {
        PlayerPawn p = playerController.playerPawn;

        p.SetMaxHealth(lastMaxHp);
        p.SetCurHealth(lastCurHp);
        p.SetMaxStamina(lastMaxStamina);
        p.SetXp(lastXp);
    }

    public GameMap GetGameMap() 
    {
        return gm;
    }

    private void FoundEnd() 
    {
        playerController.guic.BlackoutScreen();
        playerController.canMove = false;

        NextLevel();
    }
}
