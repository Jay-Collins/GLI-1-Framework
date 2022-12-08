using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [Header("Enemies needed to progress to the next stage")]
    [SerializeField] private int _stageOneAIToKill;
    [SerializeField] private int _stageTwoAIToKill;
    [SerializeField] private int _stageThreeAIToKill;
    [Header("The minimum and maximum amount of time it takes to spawn an AI")]
    [SerializeField] private float _spawnTimeMin;
    [SerializeField] private float _spawnTimeMax;
    
    [NonSerialized] public int enemiesKilled;
    private int _stage;
    private int _aiToSpawn;
    private bool _spawnAI;
    private int _aiSpawned;
    private bool _stageStarted;
    private float _spawnTimer;
    
    private void Update()
    {
        Stages();

        if (_spawnAI)
            AISpawner();
    }

    private void Start() => _spawnTimer = Random.Range(_spawnTimeMin, _spawnTimeMax);
    
    private void Stages()
    {
        switch (_stage)
        {
            case 0:
                if (!_stageStarted)
                    StartCoroutine(StageZero());
                break;
            case 1:
                _aiToSpawn = _stageOneAIToKill;
                if (!_stageStarted) 
                    StartCoroutine(StartStage());
                if (enemiesKilled > _stageOneAIToKill)
                {
                    _stage++;
                    _stageStarted = false;
                }
                break;
            case 2:
                _aiToSpawn = _stageTwoAIToKill;
                if (!_stageStarted) 
                    StartCoroutine(StartStage());
                if (enemiesKilled > _stageTwoAIToKill)
                {
                    _stage++;
                    _stageStarted = false;
                }
                break;
            case 3:
                _aiToSpawn = _stageThreeAIToKill;
                if (!_stageStarted) 
                    StartCoroutine(StartStage());
                if (enemiesKilled > _stageThreeAIToKill)
                {
                    _stage++;
                    _stageStarted = false;
                }
                break;
            case 4:
                break;
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
            if (_aiSpawned >= _aiToSpawn)
                _spawnAI = false;
            _spawnTimer = Random.Range(_spawnTimeMin, _spawnTimeMax);
        }
    }
    
    private void SpawnAI()
    {
        var ai = ObjectPool.instance.RequestAI();
        var pos = new Vector3(42f, 1.7f, 1.45f);
        
        ai.transform.position = pos;
    }

    private IEnumerator StageZero()
    {
        _stageStarted = true;
        yield return new WaitForSeconds(5);
        _stage++;
        _stageStarted = false;
    }

    private IEnumerator StartStage()
    {
        _stageStarted = true;
        _spawnAI = false;
        _aiSpawned = 0;
        yield return new WaitForSeconds(5);
        _spawnAI = true;
    }
}
