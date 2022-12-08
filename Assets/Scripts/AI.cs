using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    private NavMeshAgent _agent;
    private List<Transform> _walkPath;
    private Transform _target;
    private int _currentTarget;

    private void OnEnable()
    {
        _walkPath = AIPathManager.instance.runTheCourse;
        _agent = GetComponent<NavMeshAgent>();
        TravelToDestination();
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
        SpawnManager.instance.enemiesKilled++;
        gameObject.SetActive(false);
    }
}
