using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private TMP_Text _ammoText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _enemiesText;
    [SerializeField] private TMP_Text _topCenterText;
    [SerializeField] private TMP_Text _bottomCenterText;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private float _stageTimer;

    [NonSerialized] public static int score;
    [NonSerialized] public bool timerActive;
    [NonSerialized] public bool timeOver;

    private float _timeRemaining;
    private bool _displayBottomCenterText;
    
    private void Update()
    {
        EnemyCount();
        AmmoCount();
        ScoreCount();

        if (timerActive)
          CountDown();
    }

    private void AmmoCount() => _ammoText.text = "Ammo " + Shoot.ammo;
    private void EnemyCount() => _enemiesText.text = SpawnManager.enemies.ToString();
    private void ScoreCount() => _scoreText.text = score.ToString();
    private void Start() => _timeRemaining = _stageTimer;

    public void StageDisplay()
    {
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
                _bottomCenterText.text = "YOU GOT THE IMPOSTORS!";
                break;
        }
    }

    private IEnumerator DisplayBottomCenterTextRoutine()
    {
        _bottomCenterText.text = "SHOOT THE IMPOSTORS dont let more than half escape and you win!";
        yield return new WaitForSeconds(5);
        _bottomCenterText.text = "";
    }

    public void ResetTimer()
    {
        timeOver = false;
        _timeRemaining = _stageTimer;
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
        _timerText.text = $"{minutes:0}:{seconds:00}";
    }
}