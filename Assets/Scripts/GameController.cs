using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject wallPrefab;

    [SerializeField] PlayerController playerController;
    GameMap gm;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;

        if (floorPrefab == null || wallPrefab == null) 
        {
            Debug.Log("no floorPrefab or wallPrefab");
            Destroy(gameObject);
        }
        gm = new GameMap(floorPrefab, wallPrefab);

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
