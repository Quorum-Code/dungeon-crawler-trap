using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] MapConfig mapConfig;

    [SerializeField] PlayerController playerController;
    GameMap gm;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;

        gm = new GameMap(mapConfig);

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
}
