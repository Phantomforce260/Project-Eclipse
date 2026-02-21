using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static GameObject JKeyHint => instance.JKey;
    public static Transform MainCanvas => instance.mainCanvas;
    public static bool IsMiniGameActive = false;

    public Minigame[] Minigames;

    private static UIManager instance;

    public Animator Pause;
    public Animator Credits;
    public Animator Welcome;

    public GameObject JKey;

    private Dictionary<string, Minigame> miniGameMap = new();
    private Transform mainCanvas;

    private GameState currentState;
    private bool pauseActive;

    public enum GameState
    { Paused, Active }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        foreach (var game in Minigames)
            miniGameMap.Add(game.name, game);

        mainCanvas = transform;

        Credits.gameObject.SetActive(true);

        if (Welcome != null)
            Welcome.gameObject.SetActive(true);

        if (Pause != null)
            Pause.gameObject.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            pauseActive = !pauseActive;
            TogglePause(pauseActive);
        }
        SetJkey();
    }

    public static void SetJkey()
    {
        foreach (var _ in Door.DoorRanges.Where(func => func()).Select(func => new { }))
        {
            JKeyHint.SetActive(true);
            return;
        }
        JKeyHint.SetActive(false);
    }

    // Note: I decide to reuse the Credits animation for all 3 Menus,
    // which is why they all have the same Animator names.
    public static void ToggleIntro(bool toggle)
    {
        if (instance.Welcome != null)
            instance.Welcome.Play(toggle ? "CreditsIn" : "CreditsOut");

        if (!toggle)
            SaveManager.ActiveSave.SeenIntro = true;
    }

    public static void TogglePause(bool toggle)
    {
        if (instance.Pause != null)
        {
            instance.Pause.Play(toggle ? "CreditsIn" : "CreditsOut");
            instance.currentState = toggle ? GameState.Paused : GameState.Active;
        }
    }

    public static void ToggleCredits(bool toggle) => instance.Credits.Play(toggle ? "CreditsIn" : "CreditsOut");

    public static GameObject CreateMinigame(string name)
    {
        if (IsMiniGameActive)
        {
            Debug.LogWarning("There is already an active minigame. Aborting.");
            return null;
        }

        if (instance.miniGameMap.TryGetValue(name, out var minigame))
        {
            IsMiniGameActive = true;
            return Instantiate(minigame.gameObject, MainCanvas);
        }
        else
        {
            Debug.Log($"Could not find minigame of name: {name}");
            return null;
        }
    }
}
