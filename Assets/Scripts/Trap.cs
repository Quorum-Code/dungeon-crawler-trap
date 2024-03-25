using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public List<Trigger> triggers = new List<Trigger>();
    public GameMap gameMap;
    public Point point;
    [SerializeField] private Animator animator;

    public void Activate() 
    {
        Debug.Log("trap activated!");

        if (gameMap != null) 
        {
            Pawn p = gameMap.GetPawnAtPoint(point);
            if (p != null)
            {
                Debug.Log("damaged done to a pawn");
                p.Damage(1);
            }
            else 
            {
                Debug.Log("no pawn at point");
            }
        }

        if (animator != null) 
        {
            animator.Play("SpikeActivation");
        }
    }

    public void Deactivate() 
    {
        Debug.Log("trap deactivated!");
    }
}
