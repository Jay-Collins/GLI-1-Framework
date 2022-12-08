using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoSingleton<ObjectPool>
{
    [Header("AI Settings")]
    [SerializeField] private GameObject _aiPrefab;
    [SerializeField] private int _aiToPool;
    [SerializeField] private List<GameObject> _aiPool;
    [SerializeField] private GameObject _aiContainer;

    public override void Init()
    {
        //_aiPool = GenerateAI(_aiToPool);
    }

    public void Start() => _aiPool = GenerateAI(_aiToPool);
    

    List<GameObject> GenerateAI(int amountOfAI)
    {
        for (var i = 0; i < amountOfAI; i++)
        {
            var ai = Instantiate(_aiPrefab, _aiContainer.transform, true);
            ai.SetActive(false);
            _aiPool.Add(ai);
        }

        return _aiPool;
    }

    public GameObject RequestAI()
    {
        // Loop through ai list
        foreach (var ai in _aiPool)
        {
            if (ai.activeInHierarchy == false)
            {
                // ai is available
                ai.SetActive(true);
                return ai;
            }
        }
        
        // if we made it to this point we need to generate more ai
        GameObject newAI = Instantiate(_aiPrefab, _aiContainer.transform, true);
        newAI.SetActive(false);
        _aiPool.Add(newAI);
        
        return newAI;
    }
}