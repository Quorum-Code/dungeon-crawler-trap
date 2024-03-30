using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyPawn enemyPawn;
    IEnumerator chase;
    IEnumerator animateMove;
    [SerializeField] GameObject surpriseObject;
    [SerializeField] Animation deathAnimation;
    [SerializeField] Animator animator;

    public PlayerPawn playerPawn;
    public bool isMoving = false;

    private void Start()
    {

    }

    public void Ready(Point point, GameMap gameMap) 
    {
        enemyPawn = new EnemyPawn(point.x, point.z, gameObject, gameMap);
    }

    public void PawnMoved() 
    {
        MovePawn();
    }

    private void MovePawn()
    {
        if (animateMove != null)
            StopCoroutine(animateMove);
        animateMove = AnimateMove();
        StartCoroutine(animateMove);
    }

    private IEnumerator AnimateMove()
    {
        isMoving = true;

        Vector3 init = transform.position;
        Vector3 post = new Vector3(enemyPawn.point.x, 0.5f, enemyPawn.point.z);

        float t = 0f;
        float end = 0.3f;
        while (t < end) 
        {
            
            t += Time.deltaTime;
            gameObject.transform.position = Vector3.Lerp(init, post, t/end);
            yield return null;
        }
        transform.position = post;
        enemyPawn.FinishMove();

        if (enemyPawn.health <= 0) 
        {
            // play death anim
            animator.Play("SlimeDeath");
        }

        animateMove = null;
        isMoving = false;
    }

    public void StartChase(PlayerPawn playerPawn) 
    {
        if (chase != null)
            return;

        this.playerPawn = playerPawn;
        chase = Chase();
        StartCoroutine(chase);
    }

    private IEnumerator Chase() 
    {
        // Show '!'
        // enable object
        surpriseObject.SetActive(true);

        float total = 0f;
        float timer = 1f;

        // Wait for 1s
        while (total < timer) 
        {
            total += Time.deltaTime;
            float delta = 1 - total / timer;
            surpriseObject.transform.localScale = new Vector3(delta, delta, delta);
            yield return null;
        }
        total = 0f;
        surpriseObject.SetActive(false);

        // Start timer
        while (true) 
        {
            if (playerPawn == null)
                break;
                

            if (!isMoving)
                total += Time.deltaTime;
            // call animateMove
            if (total > timer) 
            {
                if (!enemyPawn.MoveTowardsPlayer(playerPawn))
                {
                    break;
                }

                total = 0f;
                animateMove = AnimateMove();
                StartCoroutine(animateMove);
            }

            yield return null;
        }
        Debug.Log("Chase ended");
    }

    public void Death() 
    {
        enemyPawn.Death();
        Destroy(gameObject);
    }
}
