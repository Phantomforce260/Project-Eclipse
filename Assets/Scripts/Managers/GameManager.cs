using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /* The GameManager is designed to handle scene transitions and overall game settings, like the framerate, and managing URLS. */

    public static string Version { get; private set; }

    public static string CurrentScene { get; private set; }

    public bool Vsync;
    public int VsyncCount = 0;

    public static GameManager instance;

    private static string previousScene;

    public static float WorkTimer;

    [Header("Scene Transitions")]
    [SerializeField] private Animator crossFade;

    [SerializeField] private float transitionTime = 1;

    /* Awake:
     * 
     * Initialize the singleton and get the name of the current scene. 
     * If a URP object exists, try to get the Analog an Digital Glitch Modules. */
    private void Awake()
    {
        Version = Application.version;

        if (instance == null)
            instance = this;

        CurrentScene = SceneManager.GetActiveScene().name;
    }

    /* Start:
     * 
     * Retrieve virtual currencies, UserPrefs, and save data. Save data location depends on the method of signing in.
     * The screen starts out black then fades out. Finally, set the target frame rate. This ultimately depends on machine performance. */
    private void Start()
    {
        SaveManager.Load();
        crossFade.SetTrigger("Start");

        switch (CurrentScene)
        {
            case "Depot" when SaveManager.ActiveSave.SeenIntro:
                OnClickedIntro();
                goto case "Depot";
            case "Depot" when !SaveManager.ActiveSave.SeenIntro:
                UIManager.ToggleIntro(true);
                goto case "Depot";
            case "Depot":
                AudioManager.CurrentPlayingSound = AudioManager.GetSound(
                    "Depot",
                    AudioManager.SoundType.Music
                );
                foreach (var name in UIManager.GetAllMinigames())
                {
                    AudioManager.GetSound(
                        name, 
                        AudioManager.SoundType.Music
                    ).source.volume = 0;
                    AudioManager.PlayMusic(name);
                }
                AudioManager.PlayMusic("Depot");
                break;
            case "MainMenu":
                AudioManager.PlayMusic("Intro");
                AudioManager.CurrentPlayingSound = AudioManager.GetSound("Intro", AudioManager.SoundType.Music);
                break;
        }

        SetvSync(SaveManager.ActiveSave.DoVsync);
    }

    public void OnClickedIntro()
    {
        StartCoroutine(StartTimer(() => DepotController.PackagesDelivered >= MaxPackages));
    }

    public static int MaxPackages => SaveManager.ActiveSave.Day switch
    {
        1 => 10,
        2 => 15,
        3 => 20,
        4 => 25,
        5 => 30,
        _ => 10
    };

    private void Update()
    {
        Debug.Log("Day: " + SaveManager.ActiveSave.Day);
        if (DepotController.PackagesDelivered >= MaxPackages)
        {
            DepotController.MovementEnabled = false;
            UIManager.ToggleSummary(true);
            UIManager.GenerateSummary();
        }
    }

    private IEnumerator StartTimer(Func<bool> endCondition)
    {
        WorkTimer = 0;
        while (!endCondition())
        {
            WorkTimer += Time.deltaTime;
            yield return null;
        }
    }

    /* SetvSync:
     *    dovSync: The input value from the checkbox.
     *    
     * Toggles Vsync based on input. */
    public void SetvSync(bool dovSync) => QualitySettings.vSyncCount = dovSync ? 1 : 0;


    /* Scene Loading */
    // -------------------------------------------------------------------------------------------------------------------------------------------------------

    /* LoadLevel --> LoadLevelInstance:
     *     - sceneName: The name of the scene to load.
     * 
     * Stop the music, save and reset time. Then begin loading the next scene by name. */
    public static void LoadLevel(string sceneName) => instance.LoadLevelInstance(sceneName);

    private void LoadLevelInstance(string sceneName)
    {
        AudioManager.StopMusic();
        SaveManager.Save();
        Time.timeScale = 1;
        StartCoroutine(StartSceneTransition(sceneName));
    }

    /* StartSceneTransition:
     *     - sceneName: The name of the scene to load.
     *     
     * Fade the screen to black and wait for that to finish (~TransitionTime). */
    private IEnumerator StartSceneTransition(string sceneName)
    {
        crossFade.SetTrigger("ChangeScene");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }

    /* ReloadScene:
     *
     * Load the current active scene (i.e. reloading) */
    public static void ReloadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /* EnterScene --> EnterSceneInstance:
     *     - sceneName: The name of the scene to load.
     *     
     * Store the current scene and enter a new one.
     * This allows us to go back to the scene we came from when we are done. */
    public static void EnterScene(string sceneName) => instance.EnterSceneInstance(sceneName);

    private void EnterSceneInstance(string sceneName)
    {
        previousScene = SceneManager.GetActiveScene().name;
        Time.timeScale = 1;
        LoadLevelInstance(sceneName);
    }

    /* ExitScene:
     * 
     * Used by UI buttons. Goes back to the previous scene. */
    public static void ExitScene() => instance.LoadLevelInstance(previousScene);

    // -------------------------------------------------------------------------------------------------------------------------------------------------------

    /* OpenURL:
     * 
     * Opens a URL in the browser, given the URL as a string. */
    public static void OpenURL(string link) => Application.OpenURL(link);

    /* QuitGame:
     * 
     * Closes the application. */
    public static void QuitGame() => Application.Quit();
}