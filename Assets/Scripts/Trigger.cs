using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public GameMap gameMap;
    private List<Trap> traps = new List<Trap>();
    public Point point;

    public void Activate() 
    {
        Debug.Log("trigger activated!!!");
        foreach (Trap trap in traps) 
        {
            trap.Activate();
        }
    }

    public void Release()
    {
        Debug.Log("trigger released!!!");
        foreach (Trap trap in traps)
        {
            trap.Deactivate();
        }
    }

    public void AddTraps(List<Trap> newTraps) 
    {
        traps.AddRange(newTraps);
    }
}
