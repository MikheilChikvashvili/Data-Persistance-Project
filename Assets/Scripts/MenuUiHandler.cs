using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static MainManager;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuUiHandler : MonoBehaviour
{
    public static MenuUiHandler Instance;
    public Text nameScore;
    public Text enteredName;
    public int score = 0;
    private string _name = "Name";
    public string getName { get { return _name; } }
    private CanvasGroup _canvasGroup;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        LoadScore();
        WriteNameScore();
        _canvasGroup = GetComponent<CanvasGroup>();
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartNew()
    {
        SceneManager.LoadScene(1);
        this._canvasGroup.alpha = 0;
    }
    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif
    }

    public void WriteNameScore()
    {
        nameScore.text = $"Best Score : {_name} : {score}";
    }
    public void LoadScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            MainManager.SaveData data = JsonUtility.FromJson<SaveData>(json);

            _name = data.name;
            score = data.bestScore;
        }
    }
}
