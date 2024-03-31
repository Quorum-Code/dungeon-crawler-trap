using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public GameUIController guic;
    [SerializeField] private InputActionAsset inputAsset;
    [SerializeField] private AudioSource damageSource;
    [SerializeField] private AudioSource shoveSource;
    [SerializeField] private AudioSource bumpSource;
    [SerializeField] private AudioSource potionSource;
    private GameMap gameMap;

    IEnumerator animate;
    PlayerPawn playerPawn;

    [SerializeField] GameController gameController;

    delegate void QueuedEvent(InputAction.CallbackContext context);
    QueuedEvent qe;
    InputAction.CallbackContext qeContext;

    public bool canMove = false;

    public void Ready() 
    {
        if (gameController == null)
        {
            Debug.LogError("no gameController set");
            Destroy(gameObject);
        }
        gameMap = gameController.GetGameMap();

        if (playerPawn == null)
        {
            playerPawn = new PlayerPawn(gameMap.spawn.x, gameMap.spawn.z, gameObject, gameMap);

            // UI events
            playerPawn.updateHealth = HealthChange;
            playerPawn.updateXp = XpChange;
            playerPawn.updateStamina = StaminaChange;
            playerPawn.tookDamage = TookDamageEvent;
            playerPawn.shoveEvent = ShoveEvent;
            playerPawn.bumpEvent = BumpEvent;
            playerPawn.healEvent = HealEvent;
        }
        else 
        {
            playerPawn.Reset(gameMap);
            playerPawn.SetGameMap(gameMap);
            gameMap.MovePlayerToSpawn(playerPawn);
        }

        guic.Init(playerPawn);

        transform.position = new Vector3(playerPawn.point.x, 0, playerPawn.point.z);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        if (inputAsset == null)
        {
            Debug.LogError("no inputAsset on " + gameObject.name);
            Destroy(gameObject);
        }
        else
        {
            ConnectInputEvents();
        }

        canMove = true;
        animate = null;
    }

    private void FixedUpdate()
    {
        if (playerPawn != null && guic != null) 
        {
            playerPawn.IncStaminaTime(Time.deltaTime);
            guic.UpdateStaminaProgress(playerPawn);
        }
    }

    private void ShoveEvent() 
    {
        shoveSource.Play();
    }

    private void BumpEvent() 
    {
        bumpSource.Play();
    }

    private void TookDamageEvent() 
    {
        damageSource.Play();
    }

    private void HealEvent() 
    {
        potionSource.Play();
    }

    private void TestDebug() 
    {
        Debug.Log("Event called!");
    }

    private void StaminaChange() 
    {
        guic.SetStamina(playerPawn);
    }

    private void MaxHealthChange() 
    {
        guic.SetMaxHealth(playerPawn.health);
    }

    private void HealthChange() 
    {
        guic.SetHealth(playerPawn.health);
    }

    private void XpChange() 
    {
        guic.SetXp(playerPawn.xp);
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
        if (context.phase == InputActionPhase.Started || context.phase == InputActionPhase.Disabled)
        {
            if (animate == null)
            {
                TryMove(0, 1);
            }
            else
            {
                qe = ForwardEvent;
            }
        }
    }

    private void RightEvent(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Started || context.phase == InputActionPhase.Disabled)
        {
            if (animate == null)
                TryMove(1, 0);
            else
            {
                qe = RightEvent;
            }
        }
    }
    private void BackwardEvent(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started || context.phase == InputActionPhase.Disabled)
        {
            if(animate == null)
                TryMove(0, -1);
            else
            {
                qe = BackwardEvent;
            }
        }
    }

    private void LeftEvent(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started || context.phase == InputActionPhase.Disabled)
        {
            if(animate == null)
                TryMove(-1, 0);
            else
            {
                qe = LeftEvent;
            }
        }
    }

    private void TurnLeftEvent(InputAction.CallbackContext context)
    {
        if (animate == null)
            TryTurn(true);
        else
        {
            qe = TurnLeftEvent;
        }
    }

    private void TurnRightEvent(InputAction.CallbackContext context)
    {   
        if (animate == null)
            TryTurn(false);
        else
        {
            qe = TurnRightEvent;
        }
    }

    private void TryMove(int dx, int dz) 
    {
        if (gameMap == null || !canMove)
            return;

        if (playerPawn.curStamina > 0 && animate == null)
        {
            bool ok = playerPawn.Move(dx, dz);
            gameMap.CheckIsEnd(playerPawn);

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
            }
        }
    }

    private void TryTurn(bool isLeftTurn)
    {
        if (animate == null && canMove)
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

    private void EndAnimate() 
    {
        animate = null;

        gameMap.NotifyEnemies(playerPawn);

        if (qe != null)
        {
            qeContext = new InputAction.CallbackContext();
            qe(qeContext);
            qe = null;
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

        playerPawn.FinishMove();
        EndAnimate();
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
        
        EndAnimate();
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

        EndAnimate();
    }
}
