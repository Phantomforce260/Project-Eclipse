using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class Door : MonoBehaviour
{
    private float detectionRange = 1.5f;
    public static List<Func<bool>> DoorRanges = new();

    public string MinigameName;

    public static bool MinigameStarted;

    private bool inRange;
    private bool canEnterDoor;

    public GameObject DoorNotif;

    private void Awake()
    {
        DoorRanges.Add(() => inRange && canEnterDoor);
        DoorNotif.SetActive(false);
    }

    private void Update()
    {
        inRange = PlayerInRange();
        canEnterDoor = CanEnterDoor();
        if (inRange && canEnterDoor)
        {
            UIManager.JKeyHint.text = "J - Enter Door";
            if (!MinigameStarted && Keyboard.current.jKey.wasPressedThisFrame)
            {
                MinigameStarted = true;
                UIManager.CreateMinigame(MinigameName);
                AudioManager.SwitchTracks(MinigameName);
            }
        }
        SetDoorNotif();
    }

    private bool CanEnterDoor()
    {
        foreach (var _ in DepotController.PlayerInventory.Packages.Where(name => name.Equals(MinigameName)).Select(name => new { }))
            return true;

        return false;
    }

    private void SetDoorNotif()
    {
        if (canEnterDoor)
        {
            DoorNotif.SetActive(true);
            return;
        }
        DoorNotif.SetActive(false);
    }

    private bool PlayerInRange() => Vector3.Distance(transform.position, DepotController.Position) < detectionRange;
}
