using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    public float stageTime;
    [SerializeField] private float _stageTransitionWaitTime;
    [Header("The minimum and maximum amount of time it takes to spawn an AI")]
    [SerializeField] private float _stageOneSpawnTimeMin;
    [SerializeField] private float _stageOneSpawnTimeMax;
    [SerializeField] private float _stageTwoSpawnTimeMin;
    [SerializeField] private float _stageTwoSpawnTimeMax;
    [SerializeField] private float _stageThreeSpawnTimeMin;
    [SerializeField] private float _stageThreeSpawnTimeMax;
    
    [NonSerialized] public int enemiesKilled;
    [NonSerialized] public int enemiesWon;
    [NonSerialized] public static int enemies;
    [NonSerialized] public bool spawnAI;
    [NonSerialized] public int stage;
    
    private int _aiToSpawn;
    private int _aiSpawned;
    private bool _stageStarted;
    private float _spawnTimer;
    
    private void Start()
    {
        _spawnTimer = Random.Range(_stageOneSpawnTimeMin, _stageOneSpawnTimeMax);
    }
    
    private void Update()
    {
        enemies = _aiSpawned - enemiesKilled - enemiesWon;
        
        Stages();

        if (spawnAI)
            AISpawner();
    }

    private void Stages()
    {
        switch (stage)
        {
            case 0:
                if (!_stageStarted)
                    StartCoroutine(StageZero());
                break;
            case 1:
                StageEnd();
                break;
            case 2:
                StageEnd();
                break;
            case 3:
                StageEnd();
                break;
            case 4:
                UIManager.instance.youWin = true;
                break;
        }
    }

    private void StageEnd()
    {
        if (!_stageStarted) 
            StartCoroutine(StartStage());
        if (UIManager.instance.timeOver && enemies == 0)
        {
            UIManager.instance.timerActive = false;
            UIManager.instance.ResetTimer();

            if (enemiesKilled - enemiesWon < _aiSpawned / 2)
            {
                UIManager.instance.youLose = true;
            }
            else
            {
                _stageStarted = false;
                stage++;
            }
        }
    }

    private void AISpawner()
    {
        if (_spawnTimer > 0)
            _spawnTimer -= Time.deltaTime;
        else
        {
            SpawnAI();
            _aiSpawned++;
            _spawnTimer = stage switch
            {
                1 => Random.Range(_stageOneSpawnTimeMin, _stageOneSpawnTimeMax),
                2 => Random.Range(_stageTwoSpawnTimeMin, _stageTwoSpawnTimeMax),
                3 => Random.Range(_stageThreeSpawnTimeMin, _stageThreeSpawnTimeMax),
                _ => Random.Range(_stageOneSpawnTimeMin, _stageOneSpawnTimeMax)
            };
        }
    }
    
    private void SpawnAI()
    {
        var ai = ObjectPool.instance.RequestAI();
        var pos = new Vector3(40, 2.1f, 1.45f);

        ai.transform.Rotate(new Vector3(0,-90f));
        ai.transform.position = pos;
        ai.SetActive(true);
    }

    private IEnumerator StageZero()
    {
        UIManager.instance.StageDisplay();
        _stageStarted = true;
        yield return new WaitForSeconds(_stageTransitionWaitTime);
        stage++;
        _stageStarted = false;
    }

    private IEnumerator StartStage()
    {
        _aiSpawned = 0;
        enemiesKilled = 0;
        Debug.Log("stage: " + stage);
        UIManager.instance.StageDisplay();
        _stageStarted = true;
        yield return new WaitForSeconds(_stageTransitionWaitTime);
        UIManager.instance.timerActive = true;
        spawnAI = true;
    }
}
