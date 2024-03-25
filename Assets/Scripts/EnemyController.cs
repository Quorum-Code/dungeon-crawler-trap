using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyPawn enemyPawn;
    IEnumerator animate;

    public void PawnMoved() 
    {
        MovePawn();
    }

    private void MovePawn()
    {
        if (animate != null)
            StopCoroutine(animate);
        animate = AnimateMove();
        StartCoroutine(animate);
    }

    private IEnumerator AnimateMove()
    {
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
    }
}
