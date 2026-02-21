using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static Transform MainCanvas => instance.mainCanvas;
    public static bool IsMiniGameActive = false;

    public Minigame[] Minigames;

    private static UIManager instance;

    [SerializeField] private Animator Pause;
    [SerializeField] private Animator Credits;

    private Dictionary<string, Minigame> miniGameMap = new();
    private Transform mainCanvas;

    private bool pauseActive;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        foreach (var game in Minigames)
            miniGameMap.Add(game.name, game);

        mainCanvas = transform;

        Pause.gameObject.SetActive(true);
        Credits.gameObject.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TogglePause(bool toggle) => Pause.Play(toggle ? "CreditsIn" : "CreditsOut");
    public void ToggleCredits(bool toggle) => Credits.Play(toggle ? "CreditsIn" : "CreditsOut");

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
