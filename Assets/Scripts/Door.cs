using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    public static List<Func<bool>> DoorRanges = new();

    public string MinigameName;

    private bool minigameStarted;

    private bool inRange;

    private void Awake()
    {
        DoorRanges.Add(() => inRange);
    }

    private void Update()
    {
        inRange = PlayerInRange();
        if (inRange)
        {
            UIManager.JKeyHint.SetActive(true);
            if (!minigameStarted && Keyboard.current.jKey.wasPressedThisFrame)
            {
                Debug.Log("Starting Minigame: " + MinigameName);
                minigameStarted = true;
                UIManager.CreateMinigame(MinigameName);
            }
        }
    }

    private bool PlayerInRange() => Vector3.Distance(transform.position, DepotController.Position) < 1f;
}
