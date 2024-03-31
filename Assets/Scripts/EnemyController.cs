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

    public int damage = 1;
    public float moveDelay = 1f;
    public int health = 1;

    public bool isBoss = false;

    public void Ready(Point point, GameMap gameMap) 
    {
        enemyPawn = new EnemyPawn(point.x, point.z, damage, health, gameObject, gameMap);
    }

    public void PawnMoved() 
    {
        MovePawn();
    }

    private void MovePawn()
    {
        if (animateMove != null)
            StopCoroutine(animateMove);

        if ((int)gameObject.transform.position.x  == enemyPawn.point.x && (int)gameObject.transform.position.z == enemyPawn.point.z)
        {
            animateMove = AnimateBump(enemyPawn.DirTowardsPlayer(playerPawn));
            StartCoroutine(animateMove);
        }
        else
        {
            animateMove = AnimateMove();
            StartCoroutine(animateMove);
        }
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
            if (playerPawn != null)
                playerPawn.AddXp(3);
            else
                Debug.LogError("playerController null?");

            // play death anim
            animator.Play("SlimeDeath");
        }

        animateMove = null;
        isMoving = false;
    }

    private IEnumerator AnimateBump((int, int) dir) 
    {
        isMoving = true;

        int dx = dir.Item1;
        int dz = dir.Item2;

        Vector3 init = gameObject.transform.position;
        Vector3 post = new Vector3(enemyPawn.point.x + dx * .2f, 0.5f, enemyPawn.point.z + dz * .2f);

        float t = 0f;
        float end = 0.075f;
        while (t < end)
        {
            t += Time.deltaTime;
            gameObject.transform.position = Vector3.Lerp(init, post, t / end);
            yield return null;
        }

        init = new Vector3(enemyPawn.point.x, 0.5f, enemyPawn.point.z);

        t = 0f;
        while (t < end)
        {
            t += Time.deltaTime;
            gameObject.transform.position = Vector3.Lerp(post, init, t / end);
            yield return null;
        }
        gameObject.transform.position = init;

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
        float timer = moveDelay;

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

            if (animateMove == null)
                total += Time.deltaTime;
            else
                total = 0f;
            // call animateMove
            if (total > timer)
            {
                if (!enemyPawn.MoveTowardsPlayer(playerPawn))
                    break;

                total = 0f;
            }

            yield return null;
        }
        chase = null;
    }

    public void Death() 
    {
        enemyPawn.Death();
        Destroy(gameObject);
    }
}
