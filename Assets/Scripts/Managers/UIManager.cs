using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static Transform MainCanvas => instance.mainCanvas;
    public static bool IsMiniGameActive = false;

    public Minigame[] Minigames;

    private static UIManager instance;

    private Dictionary<string, Minigame> miniGameMap = new();
    private Transform mainCanvas;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        foreach (var game in Minigames)
            miniGameMap.Add(game.name, game);

        mainCanvas = transform;
    }

    void Start() { }

    void Update() { }

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
