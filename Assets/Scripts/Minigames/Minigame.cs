using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class Minigame : MonoBehaviour
{
    /* Minigame Code structure: */

    /* If external scripts need to perform actions when a minigame starts or ends, add them as listeners here. */
    [Space(10)]
    public UnityEvent OnGameStart = new();
    [Space(10)]
    public UnityEvent OnGameFinish = new();

    public string musicName;

    protected void Initialize()
    {
        /* Should be called by Awake() in child classes. */
        DepotController.MovementEnabled = false;
        OnGameStart.Invoke();
    }

    protected void Finish()
    {
        /* Call this to end a minigame. */
        UIManager.IsMiniGameActive = false;
        DepotController.MovementEnabled = true;
        OnGameFinish.Invoke();
        Destroy(gameObject);
    }
}
