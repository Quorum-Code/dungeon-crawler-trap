using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private List<Trigger> triggers = new List<Trigger>();
    public GameMap gameMap;
    public Point point;
    [SerializeField] private Animator animator;

    public void Activate() 
    {
        //Debug.Log("trap activated!");

        if (gameMap != null) 
        {
            Pawn p = gameMap.GetPawnAtPoint(point);
            if (p != null)
            {
                //Debug.Log("damaged done to a pawn");
                p.Damage(1);
            }
        }

        if (animator != null) 
        {
            animator.Play("SpikeActivation");
        }

        AudioSource audioSource = gameObject.GetComponentInChildren<AudioSource>();
        if (audioSource != null) 
        {
            //Debug.Log("played trap audio");
            audioSource.Play();
        }
    }

    public void Deactivate() 
    {

    }

    public void AddTriggers(List<Trigger> newTriggers) 
    {
        triggers.AddRange(newTriggers);
    }

    public void AddTrigger(Trigger trigger) 
    {
        triggers.Add(trigger);
    }
}
