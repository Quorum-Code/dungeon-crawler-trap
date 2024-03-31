using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMusic : MonoBehaviour
{
    public static DungeonMusic instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("destroyed!");
            Destroy(gameObject);
        }
        else 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
