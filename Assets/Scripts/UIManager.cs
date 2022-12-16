using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private TMP_Text _ammoText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _enemiesText;
    [SerializeField] private TMP_Text _topCenterText;
    [SerializeField] private TMP_Text _bottomCenterText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private TMP_Text _gameWinText;
    [SerializeField] private TMP_Text _restartText;

    [NonSerialized] public static int score;
    [NonSerialized] public bool timerActive;
    [NonSerialized] public bool timeOver;
    
    public bool youLose;
    public bool youWin;
    private float _timeRemaining;
    private bool _displayBottomCenterText;
    
    private void Update()
    {
        EnemyCount();
        AmmoCount();
        ScoreCount();
        if (timerActive)
          CountDown();
        if (youLose)
            GameOver();
        if (youWin)
            GameWin();
    }

    private void AmmoCount()
    {
        if (_ammoText is not null) // null check
            _ammoText.text = "Ammo " + Shoot.ammo;
    }

    private void EnemyCount()
    {
        if (_enemiesText is not null) // null check
            _enemiesText.text = SpawnManager.enemies.ToString();
    }

    private void ScoreCount()
    {
        if (_scoreText is not null) // null check
            _scoreText.text = score.ToString();
    }

    private void Start() => _timeRemaining = SpawnManager.instance.stageTime;
    
    public void StageDisplay()
    {
        if (_topCenterText is null) return; // null check
        switch (SpawnManager.instance.stage)
        {
            case 0:
                _topCenterText.text = "WARNING";
                break;
            case 1:
                _topCenterText.text = "STAGE 1";
                StartCoroutine(DisplayBottomCenterTextRoutine());
                break;
            case 2:
                _topCenterText.text = "STAGE 2";
                break;
            case 3:
                _topCenterText.text = "STAGE 3";
                break;
            case 4:
                _topCenterText.text = "YOU WIN!";
                if (_bottomCenterText is not null) // null check
                    _bottomCenterText.text = "YOU GOT THE IMPOSTORS!";
                break;
        }
    }

    private IEnumerator DisplayBottomCenterTextRoutine()
    {
        if (_bottomCenterText is null) yield break; // null check
        
        _bottomCenterText.text = "SHOOT THE IMPOSTORS dont let more than half escape and you win!";
        yield return new WaitForSeconds(5);
        _bottomCenterText.text = "";
    }

    public void ResetTimer()
    {
        timeOver = false;
        _timeRemaining = SpawnManager.instance.stageTime;
    }

    private void CountDown()
    {
        if (_timeRemaining > 0)
            _timeRemaining -= Time.deltaTime;
        else
        {
            SpawnManager.instance.spawnAI = false;
            _timeRemaining = 0;
            timeOver = true;
        }
        DisplayTime(_timeRemaining);
    }

    private void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        if (_timerText is not null) // null check
            _timerText.text = $"{minutes:0}:{seconds:00}";
    }

    private void GameOver()
    {
        if (_gameWinText is null || _restartText is null) return; // null check
        
        _gameOverText.enabled = true;
        _restartText.enabled = true;

        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(0);
    }

    private void GameWin()
    {
        if (_gameWinText is null || _restartText is null) return; // null check
        
        _gameWinText.enabled = true;
        _restartText.enabled = true;
        
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(0);
    }
}