using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputAsset;
    private PlayerInput playerInput;

    bool isMoving = false;
    Mutex moving;

    int x = 0;
    int z = 0;

    IEnumerator animate;

    PlayerPawn playerPawn;

    [SerializeField] GameController gameController;

    public void Ready() 
    {
        if (gameController == null)
        {
            Debug.LogError("no gameController set");
            Destroy(gameObject);
        }
        GameMap gm = gameController.GetGameMap();
        playerPawn = new PlayerPawn(gm.spawn.x, gm.spawn.z, gameObject, gm);
        transform.position = new Vector3(playerPawn.point.x, 0, playerPawn.point.z);


        if (inputAsset == null)
        {
            Debug.LogError("no inputAsset on " + gameObject.name);
            Destroy(gameObject);
        }
        else
        {
            ConnectInputEvents();
        }

        playerInput = GetComponent<PlayerInput>();
        animate = null;
    }

    private void ConnectInputEvents() 
    {
        InputActionMap dungeonControls = inputAsset.FindActionMap("DungeonCrawling");
        if (dungeonControls == null)
        {
            Debug.LogError("no InputActionMap found");
            return;
        }

        dungeonControls.FindAction("Forward").started += ForwardEvent;
        dungeonControls.FindAction("Right").started += RightEvent;
        dungeonControls.FindAction("Backward").started += BackwardEvent;
        dungeonControls.FindAction("Left").started += LeftEvent;

        dungeonControls.FindAction("TurnLeft").performed += TurnLeftEvent;
        dungeonControls.FindAction("TurnRight").performed += TurnRightEvent;
    }

    private void ForwardEvent(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Started)
        {
            TryMove(0, 1);
        }
    }

    private void RightEvent(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Started)
        {
            TryMove(1, 0);
        }
    }
    private void BackwardEvent(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            TryMove(0, -1);
        }
    }

    private void LeftEvent(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            TryMove(-1, 0);
        }
    }

    private void TurnLeftEvent(InputAction.CallbackContext context)
    {
        TryTurn(true);
    }

    private void TurnRightEvent(InputAction.CallbackContext context)
    {   
        TryTurn(false);
    }

    private void TryMove(int dx, int dz) 
    {
        if (animate == null) 
        {
            bool ok = playerPawn.Move(dx, dz);
            if (ok)
            {
                // move
                animate = AnimateMove();
                StartCoroutine(animate);
            }
            else 
            {
                // bump
                animate = AnimateBump(dx, dz);
                StartCoroutine(animate);
                Debug.Log("bumping");
            }
        }
    }

    private void TryTurn(bool isLeftTurn)
    {
        if (animate == null)
        {
            if (isLeftTurn)
            {
                playerPawn.TurnLeft();
            }
            else 
            {
                playerPawn.TurnRight();
            }
            animate = AnimateTurn(isLeftTurn);
            StartCoroutine(animate);
        }
    }

    private IEnumerator AnimateMove() 
    {
        Vector3 init = gameObject.transform.position;
        Vector3 post = new Vector3(playerPawn.point.x, 0, playerPawn.point.z);

        float t = 0f;
        float end = 0.3f;
        while (t < end)
        {
            t += Time.deltaTime;
            gameObject.transform.position = Vector3.Lerp(init, post, t / end);
            yield return null;
        }
        gameObject.transform.position = post;
        animate = null;
    }

    private IEnumerator AnimateBump(int dx, int dz) 
    {
        Point rel = playerPawn.InputToPoint(dx, dz);

        Vector3 init = gameObject.transform.position;
        Vector3 post = init + new Vector3(rel.x * .2f, 0, rel.z * .2f);

        float t = 0f;
        float end = 0.15f;
        while (t < end)
        {
            t += Time.deltaTime;
            gameObject.transform.position = Vector3.Lerp(init, post, t / end);
            yield return null;
        }

        t = 0f;
        while (t < end)
        {
            t += Time.deltaTime;
            gameObject.transform.position = Vector3.Lerp(post, init, t / end);
            yield return null;
        }
        gameObject.transform.position = init;
        animate = null;
    }

    private IEnumerator AnimateTurn(bool isLeftTurn)
    {
        Vector3 initRot = gameObject.transform.rotation.eulerAngles;
        Vector3 postRot = initRot + new Vector3(0, 90 * (isLeftTurn ? -1 : 1), 0);

        float t = 0f;
        float end = 0.3f;
        while (t < end) 
        {
            t += Time.deltaTime;
            gameObject.transform.rotation = Quaternion.Euler(Vector3.Lerp(initRot, postRot, t/end));
            yield return null;
        }
        gameObject.transform.rotation = Quaternion.Euler(postRot);
        animate = null;
    }
}
