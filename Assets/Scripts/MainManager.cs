using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text BestScore;
    public Text ScoreText;
    public GameObject GameOverText;
    
    public int _bestScore { get; private set; }
    private string _name;
    private string _previousName;
    public string Name { get { return _name; } }
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    private void Awake()
    {
        _name = MenuUiHandler.Instance.getName;
        _bestScore = MenuUiHandler.Instance.score;

        UpdateBestScore();
        _name = MenuUiHandler.Instance.enteredName.text;
        
        _previousName = MenuUiHandler.Instance.getName;
        _name = MenuUiHandler.Instance.enteredName.text;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void UpdateBestScore()
    {
        BestScore.text = $"Best Score : {_name} : {_bestScore}";
    }
    public void GameOver()
    {
        if(_name != _previousName)
        {
            _bestScore = m_Points;
            UpdateBestScore();
        }
        else if(m_Points > _bestScore && _name == _previousName)
        {
            _name = MenuUiHandler.Instance.enteredName.text;
            _bestScore = m_Points;
            UpdateBestScore();
        }
        m_GameOver = true;
        GameOverText.SetActive(true);
        SaveScore();
        MenuUiHandler.Instance.LoadScore();
        MenuUiHandler.Instance.enteredName.text = MenuUiHandler.Instance.getName;
    }
    
    [System.Serializable]
    public class SaveData
    {
        public string name;
        public int bestScore;
    }

    public void SaveScore()
    {
        SaveData data = new SaveData();
        data.name = _name;
        data.bestScore = _bestScore;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }
}
