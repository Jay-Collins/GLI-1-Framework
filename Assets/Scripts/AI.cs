using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class AI : MonoBehaviour
{
    public static event Action<Vector3> deathCry;

    private enum aiStates {Run, Hide, Death}

    private aiStates _currentAIState;
    
    private NavMeshAgent _agent;
    private List<Transform> _walkPath;
    private List<Transform> _hidePoints;
    private Transform _target;
    private int _currentTarget;
    private RaycastHit _raycastHit;
    private float _closestHidingSpot;
    private Transform _hideSpot;

    [Header("Hide Timer")] 
    [SerializeField] private int _minHideTime;
    [SerializeField] private int _maxHideTime;
    
    private void OnEnable()
    {
        AI.deathCry += Hide;

        _currentAIState = aiStates.Run;
        _walkPath = AIPathManager.instance.runTheCourse;
        _hidePoints = AIPathManager.instance.hideSpots;
        _agent = GetComponent<NavMeshAgent>();
        _currentAIState = aiStates.Run;
    }

    private void Update()
    {
        DecisionMaking();
        if (Input.GetKeyDown(KeyCode.H))
        {
            _currentAIState = aiStates.Hide;
            _closestHidingSpot = Mathf.Infinity;
            StartCoroutine(HideTimer());
        }
    }

    private void DecisionMaking()
    {
        switch (_currentAIState)
        {
            case aiStates.Run:
                TravelToDestination();
                break;
            
            case aiStates.Hide:
                foreach (var hidingSpot in _hidePoints)
                {
                    var distance = Vector3.Distance(transform.position, hidingSpot.position);
                    if (distance < _closestHidingSpot)
                    {
                        _closestHidingSpot = distance;
                        _hideSpot = hidingSpot;
                    }
                }
                _agent.SetDestination(_hideSpot.position);
                break;
            
            case aiStates.Death:
                break;
        }
    }

    private void TravelToDestination()
    {
        if (_agent.remainingDistance < 0.5f)
        {
            if (_currentTarget < _walkPath.Count - 1)
                _currentTarget++;
        }

        _agent.SetDestination(_walkPath[_currentTarget].position);
    }

    private void OnDeath()
    {
        deathCry?.Invoke(transform.position);
        // add 50 points
        _currentAIState = aiStates.Death;
        SpawnManager.instance.enemiesKilled++;
        gameObject.SetActive(false);
    }

    private void Hide(Vector3 deadAI)
    {
        var distance = Vector3.Distance(transform.position, deadAI);

        if (distance <= 10)
        {
            _currentAIState = aiStates.Hide;
            _closestHidingSpot = Mathf.Infinity;
            StartCoroutine(HideTimer());
        }
    }

    private IEnumerator HideTimer()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(_minHideTime, _maxHideTime));
        _currentAIState = aiStates.Run;
    }
}
