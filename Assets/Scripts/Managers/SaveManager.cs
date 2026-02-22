using System;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveData ActiveSave => instance.activeSave;

    private SaveData activeSave = new();

    private static SaveManager instance;
    private const string saveKey = "Save";

    [Serializable]
    public class SaveData
    {
        public string Name = "MainSave";
        public bool SeenIntro;

        public bool DoVsync = true;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public static void Save() => instance.SaveInstance();

    private void SaveInstance()
    {
        string json = JsonUtility.ToJson(activeSave);
        if (json.Length < 256)
            PlayerPrefs.SetString(saveKey, json);
        else
            Debug.Log("JSON is too long");
    }

    public static void Load() => instance.LoadInstance();

    private void LoadInstance()
    {
        string dataStr = PlayerPrefs.GetString(saveKey);
        if (!string.IsNullOrEmpty(dataStr))
            activeSave = JsonUtility.FromJson<SaveData>(dataStr);
        else
            Debug.Log("No save data to load");
    }

    public static void DeleteSave() => PlayerPrefs.DeleteKey(saveKey);
}
