using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] MapConfig mapConfig;

    [SerializeField] PlayerController playerController;
    [SerializeField] GameUIController guic;
    GameMap gm;

    bool waiting = true;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 100;

        gm = new GameMap(mapConfig);
        gm.endFound = FoundEnd;
        gm.isLoading = true;
        StartCoroutine(gm.LoadDungeon(mapConfig.Dungeon1()));
        StartCoroutine(WaitForLoading());
    }

    private IEnumerator WaitForLoading() 
    {
        while (gm.isLoading) 
        {
            yield return null;
        }
        playerController.Ready();
        guic.FadeScreenOut();
    }

    public GameMap GetGameMap() 
    {
        return gm;
    }

    private void FoundEnd() 
    {
        playerController.guic.BlackoutScreen();

        gm.isLoading = true;
        StartCoroutine(gm.LoadDungeon(mapConfig.Dungeon1()));
        StartCoroutine(WaitForLoading());
    }
}
