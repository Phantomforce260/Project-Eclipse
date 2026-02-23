using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static TextMeshProUGUI JKeyHint => instance.JKey;
    public static Transform MainCanvas => instance.mainCanvas;
    public static bool IsMiniGameActive = false;

    public Minigame[] Minigames;

    private static UIManager instance;

    public Animator Pause;
    public Animator Credits;
    public Animator Welcome;
    public Animator Summary;

    public TextMeshProUGUI JKey;
    public TextMeshProUGUI Notif;

    public Transform MinigameParent;

    public Sprite[] RankBadges;

    public TextMeshProUGUI PackageCount;
    public TextMeshProUGUI TimeCount;
    public Image Badge;

    private Dictionary<string, Minigame> miniGameMap = new();
    private Transform mainCanvas;

    private GameState currentState;
    private bool pauseActive;

    private string prevAudioTrack;

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

        if (Summary != null)
            Summary.gameObject.SetActive(true);

        if (Notif != null)
            Notif.gameObject.SetActive(false);

        if (Welcome != null)
            Welcome.gameObject.SetActive(true);

        if (Pause != null)
            Pause.gameObject.SetActive(true);
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
        if (JKeyHint != null)
        {
            foreach (var _ in Door.DoorRanges.Where(func => func()).Select(func => new { }))
            {
                JKeyHint.gameObject.SetActive(true);
                return;
            }
            JKeyHint.gameObject.SetActive(false);
        }
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

    public static void ToggleSummary(bool toggle)
    {
        if (instance.Summary != null)
            instance.Summary.Play(toggle ? "CreditsIn" : "CreditsOut");
    }

    public static void TogglePause(bool toggle)
    {
        if (instance.Pause != null)
        {
            instance.Pause.Play(toggle ? "CreditsIn" : "CreditsOut");
            instance.currentState = toggle ? GameState.Paused : GameState.Active;
        }
    }

    public static void ToggleCredits(bool toggle)
    {
        instance.Credits.Play(toggle ? "CreditsIn" : "CreditsOut");

        if (toggle)
        {
            instance.prevAudioTrack = AudioManager.CurrentPlayingSound.name;
            AudioManager.StopMusic();
            instance.Invoke(nameof(InvokeCredits), 1f);
        }
        else
        {
            AudioManager.StopMusic();
            instance.Invoke(nameof(InvokePrev), 1f);
        }
    }

    private void InvokeCredits() => AudioManager.PlayMusic("Credits");

    private void InvokePrev() => AudioManager.PlayMusic(instance.prevAudioTrack);

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
            return Instantiate(minigame.gameObject, instance.MinigameParent);
        }
        else
        {
            Debug.Log($"Could not find minigame of name: {name}");
            return null;
        }
    }

    public static List<string> GetAllMinigames()
    {
        List<string> output = new();
        output.AddRange(instance.Minigames.Select(minigame => minigame.name));
        return output;
    }

    public static void SetNotif(string message, float timer = 5f)
    {
        instance.CancelInvoke();
        instance.Notif.text = message;
        instance.Notif.gameObject.SetActive(true);
        instance.Invoke(nameof(HideNotif), timer);
    }

    public static void SetNotif(string message, Color textColor, float timer = 5f)
    {
        instance.CancelInvoke();
        instance.Notif.text = message;
        instance.Notif.color = textColor;
        instance.Notif.gameObject.SetActive(true);
        instance.Invoke(nameof(HideNotif), timer);
    }

    private void HideNotif() => Notif.gameObject.SetActive(false);

    public static void GenerateSummary()
    {
        float finalTime = GameManager.WorkTimer;
        instance.PackageCount.text = DepotController.PackagesDelivered.ToString();
        instance.TimeCount.text = FloatToTime(finalTime);

        DepotController.PackagesDelivered = 0;

        int badgeIndex;
        if (finalTime < 105)
            badgeIndex = 0;
        else if (finalTime < 120)
            badgeIndex = 1;
        else if (finalTime < 135)
            badgeIndex = 2;
        else if (finalTime < 150)
            badgeIndex = 3;
        else if (finalTime < 165)
            badgeIndex = 4;
        else
            badgeIndex = 4;

        instance.Badge.sprite = instance.RankBadges[badgeIndex];

        instance.StartCoroutine(instance.RemoveSummary());
    }

    public static string FloatToTime(float totalSeconds)
    {
        int minutes = (int)(totalSeconds / 60);
        int seconds = (int)(totalSeconds % 60);

        return $"{minutes}:{seconds:D2}";
    }

    IEnumerator RemoveSummary ()
    {
        yield return new WaitForSeconds(7.5f);

        SaveManager.ActiveSave.Day++;
        GameManager.instance.OnClickedIntro();
        DepotController.MovementEnabled = true;
        ToggleSummary(false);
    }
}