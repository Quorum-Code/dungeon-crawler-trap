using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public void Activate() 
    {
        Debug.Log("trigger activated!!!");
    }

    public void Release() 
    {
        Debug.Log("trigger released!!!");
    }
}
