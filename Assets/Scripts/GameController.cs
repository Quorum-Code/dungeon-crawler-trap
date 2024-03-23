using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject wallPrefab;

    GameMap gm;

    // Start is called before the first frame update
    void Start()
    {
        if (floorPrefab == null || wallPrefab == null) 
        {
            Debug.Log("no floorPrefab or wallPrefab");
            Destroy(gameObject);
        }

        gm = new GameMap(floorPrefab, wallPrefab);
    }
}
