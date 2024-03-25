using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    Camera main;

    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
    }

    void Update()
    {
        transform.rotation = main.transform.rotation;
    }
}
