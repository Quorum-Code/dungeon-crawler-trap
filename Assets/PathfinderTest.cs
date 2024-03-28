using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector2 a = new Vector2(0, 0);
        Vector2 b = new Vector2(0, -1);

        float angleRadians = (Mathf.Atan2(a.y - b.y, a.x - b.x) + Mathf.PI * 2) % (Mathf.PI * 2);
        Debug.Log(angleRadians);
        Debug.Log(Mathf.Rad2Deg * angleRadians);

        if (angleRadians < (7 * Mathf.PI / 4) && angleRadians >= (5 * Mathf.PI / 4))
        {
            Debug.Log("Down");
        }
        else if (angleRadians >= (3 * Mathf.PI / 4) && angleRadians < (5 * Mathf.PI / 4))
        {
            Debug.Log("Left");
        }
        else if (angleRadians < (3 * Mathf.PI / 4) && angleRadians >= (Mathf.PI / 4))
        {
            Debug.Log("Up");
        }
        else 
        {
            Debug.Log("Right");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
