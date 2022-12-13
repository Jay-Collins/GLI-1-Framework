using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using Random = System.Random;

public class AI : MonoBehaviour
{
    public static event Action<Vector3> deathCry;

    private enum aiStates {Run = 0, Hide = 1, Death = 2}
    private aiStates _currentAIState;
    
    private NavMeshAgent _agent;
    private List<Transform> _walkPath;
    private List<Transform> _hidePoints;
    private Transform _hideSpot;
    private Transform _target;
    private int _currentTarget;
    private float _closestHidingSpot;
    private bool _onDeath;
    private bool _dead;
    

    [Header("Astronaut Settings")] 
    [SerializeField] private float _speed;
    [SerializeField] private float _detectDeadDistance;
    [SerializeField] private int _scoreAwarded;

    [Header("Hide Timer")] 
    [SerializeField] private int _minHideTime;
    [SerializeField] private int _maxHideTime;

    [Header("Astronaut Object And Component References")] 
    [SerializeField] private SkinnedMeshRenderer _astronautObject;
    [SerializeField] private SkinnedMeshRenderer _astronautBackpackObject;
    [SerializeField] private Animator _animator;

    private void OnEnable()
    {
        Shoot.aiGotShot += GotShot;
        deathCry += Hide;
        
        _currentAIState = aiStates.Run;
        _walkPath = AIPathManager.instance.runTheCourse;
        _hidePoints = AIPathManager.instance.hidePoints;
        _agent = GetComponent<NavMeshAgent>();
        _currentAIState = aiStates.Run;
        ColorSelector();
    }

    private void Update()
    {
        if (_dead) return;
        
        DecisionMaking();
        
        _agent.speed = _speed;
        _animator.SetFloat("Speed", _speed);
    }

    private void DecisionMaking()
    {
        switch (_currentAIState)
        {
            case aiStates.Run:
                TravelToDestination();
                break;
            
            case aiStates.Hide:
                InHideState();
                break;
            
            case aiStates.Death:
                if (_onDeath) 
                    OnDeath();
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

    private void GotShot(Object hitInfo)
    {
        if (hitInfo == gameObject.transform)
        {
            _onDeath = true;
            _currentAIState = aiStates.Death;
        }
    }
    
    private void OnDeath()
    {
        deathCry -= Hide;
        
        _onDeath = false;
        _agent.enabled = false;
        _animator.SetBool("Death", true);
        UIManager.score += _scoreAwarded;
        SpawnManager.instance.enemiesKilled++;
        _dead = true;
        deathCry?.Invoke(transform.position);
    }

    private void Hide(Vector3 deadAI)
    {
        if (!gameObject.activeInHierarchy) return;
            
        var distance = Vector3.Distance(transform.position, deadAI);

        if (distance <= _detectDeadDistance)
        {
            _currentAIState = aiStates.Hide;
            _closestHidingSpot = Mathf.Infinity;
            StartCoroutine(HideTimer());
        }
    }

    private void InHideState()
    {
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
        
        if (transform.position == _agent.destination)
            _animator.SetBool("Hiding", true);
    }

    private IEnumerator HideTimer()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(_minHideTime, _maxHideTime));
        _animator.SetBool("Hiding", false);
        _currentAIState = aiStates.Run;
    }

    private void ColorSelector()
    {
        var selector = UnityEngine.Random.Range(0, 8);
        var astronaut = _astronautObject.material;
        var backpack = _astronautBackpackObject.material;
        
        switch (selector)
        {
            case 0:
                astronaut.color = Color.blue;
                backpack.color = Color.blue;
                break;
            case 1:
                astronaut.color = Color.cyan;
                backpack.color = Color.cyan;
                break;
            case 2:
                astronaut.color = Color.gray;
                backpack.color = Color.gray;
                break;
            case 3:
                astronaut.color = Color.green;
                backpack.color = Color.green;
                break;
            case 4:
                astronaut.color = Color.magenta;
                backpack.color = Color.magenta;
                break;
            case 5:
                astronaut.color = Color.red;
                backpack.color = Color.red;
                break;
            case 6:
                astronaut.color = Color.white;
                backpack.color = Color.white;
                break;
            case 7:
                astronaut.color = Color.yellow;
                backpack.color = Color.yellow;
                break;
        }
    }
}
