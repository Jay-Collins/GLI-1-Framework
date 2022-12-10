using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class AI : MonoBehaviour
{
    public static event Action<Vector3> deathCry;

    private enum aiStates {Run = 0, Hide = 1, Death = 2}
    private aiStates _currentAIState;
    
    private NavMeshAgent _agent;
    private List<Transform> _walkPath;
    private List<Transform> _hidePoints;
    private Transform _target;
    private int _currentTarget;
    private RaycastHit _raycastHit;
    private float _closestHidingSpot;
    private Transform _hideSpot;
    private bool _currentlyHiding;

    [Header("Astronaut Max Speed")] 
    [SerializeField] private float _speed;

    [Header("Hide Timer")] 
    [SerializeField] private int _minHideTime;
    [SerializeField] private int _maxHideTime;

    [Header("Astronaut and Backpack Object")] 
    [SerializeField] private SkinnedMeshRenderer _astronautObject;
    [SerializeField] private SkinnedMeshRenderer _astronautBackpackObject;

    [Header("Astronaut Animator")] 
    [SerializeField] private Animator _animator;

    private static readonly int Speed = Animator.StringToHash("speed");

    private void OnEnable()
    {
        AI.deathCry += Hide;
        
        _currentAIState = aiStates.Run;
        _walkPath = AIPathManager.instance.runTheCourse;
        _hidePoints = AIPathManager.instance.hidePoints;
        _agent = GetComponent<NavMeshAgent>();
        _currentAIState = aiStates.Run;
        ColorSelector();
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

    private void OnDeath()
    {
        deathCry?.Invoke(transform.position);
        _animator.SetBool("Death", true);
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
