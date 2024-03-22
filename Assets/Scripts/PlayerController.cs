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

    private void Start()
    {
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

        dungeonControls.FindAction("Forward").performed += ForwardEvent;

        dungeonControls.FindAction("TurnLeft").performed += TurnLeftEvent;
        dungeonControls.FindAction("TurnRight").performed += TurnRightEvent;
    }

    private void ForwardEvent(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Started)
        {
            TryMove(new Vector3(1, 0, 0));
        }
        else 
        {
            Debug.Log(context.phase);
        }
    }

    private void TurnLeftEvent(InputAction.CallbackContext context)
    {
        Debug.Log("Trying left turn");
        TryTurn(new Vector3(0, -90f, 0));
    }

    private void TurnRightEvent(InputAction.CallbackContext context)
    {   
        TryTurn(new Vector3(0, 90f, 0));
    }

    private void TryMove(Vector3 delta) 
    {
        if (animate == null) 
        {
            animate = AnimateMove(delta);
            StartCoroutine(animate);
        }
    }

    private void TryTurn(Vector3 delta)
    {
        if (animate == null)
        {
            Debug.Log("animate == null");
            animate = AnimateTurn(delta);
            StartCoroutine(animate);
        }
        else 
        {
            Debug.Log("animate != null");
        }
    }

    private IEnumerator AnimateMove(Vector3 delta) 
    {
        Vector3 init = gameObject.transform.position;
        Vector3 post = init + delta;

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

    private IEnumerator AnimateTurn(Vector3 delta)
    {
        Vector3 initRot = gameObject.transform.rotation.eulerAngles;
        Vector3 postRot = initRot + delta;

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
