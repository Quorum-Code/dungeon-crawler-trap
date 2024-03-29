using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] MapConfig mapConfig;

    [SerializeField] PlayerController playerController;
    [SerializeField] GameUIController guic;
    GameMap gm;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 100;

        gm = new GameMap(mapConfig);
        gm.endFound = FoundEnd;
        StartCoroutine(gm.LoadDungeon());

        if (playerController == null) 
        {
            Debug.LogError("no playerController");
            Destroy(gameObject);
        }

        playerController.Ready();
    }

    public GameMap GetGameMap() 
    {
        return gm;
    }

    private void FoundEnd() 
    {
        Debug.Log("start loading next level!");

        playerController.guic.BlackoutScreen();
    }
}
