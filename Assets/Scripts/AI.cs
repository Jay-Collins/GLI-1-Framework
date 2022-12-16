using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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
    private List<Transform> _wayPoints;
    private Transform _hideSpot;
    private int _index;
    private float _closestHidingSpot;
    private bool _onDeath;
    private bool _dead;
    private bool _initialWaypoint;

    [Header("Astronaut Settings")] 
    [SerializeField] private float _speed;
    [SerializeField] private float _detectDeadDistance;
    [SerializeField] private int _scoreAwarded;
    [SerializeField] private AudioClip[] _deathSounds;
    [SerializeField] private AudioClip _pathCompleteSound;

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
        _wayPoints = AIPathManager.instance.wayPoints;
        _agent = GetComponent<NavMeshAgent>();
        ColorSelector();
        // travel to first point
        _agent.SetDestination(_wayPoints[0].position);
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
        // check if close to current destination
        // randomly select next point
        // after arriving, activate hide
        // select next waypoint going forward
        // if within 4 waypoints of end, just go to end
        
        if (_agent.remainingDistance < 0.5 && _index != 0)
        {
            Debug.Log("TravelToDestination If 1");
            if (_animator.GetBool("Hiding") == false)
            {
                _animator.SetBool("Hiding", true);
                StartCoroutine(HideTimer());
            }
        }
        
        if (!_initialWaypoint)
        {
            Debug.Log("TravelToDestination If 2");
            _initialWaypoint = true;
            _index = UnityEngine.Random.Range(_index + 1, _wayPoints.Count - 1);
            _agent.SetDestination(_wayPoints[_index].position);
        }
    }
    
    private void PathCompleted()
    {
        Debug.Log("Made it!");
        AudioSource.PlayClipAtPoint(_pathCompleteSound, transform.position);
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
        AudioSource.PlayClipAtPoint(_deathSounds[UnityEngine.Random.Range(0,3)], transform.position);
        _onDeath = false;
        _agent.enabled = false;
        _animator.SetBool("Death", true);
        UIManager.score += _scoreAwarded;
        SpawnManager.instance.enemiesKilled++;
        _dead = true;
        deathCry?.Invoke(transform.position);
    }

    private void OnDisable()
    {
        Shoot.aiGotShot -= GotShot;
        deathCry -= Hide;
    }

    private void Hide(Vector3 deadAI)
    {
        Debug.Log("Hide");
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
        Debug.Log("InHideState");
        foreach (var hidingSpot in _wayPoints)
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
        Debug.Log("Made it to Hide Timer");
        yield return new WaitForSeconds(UnityEngine.Random.Range(_minHideTime, _maxHideTime));
        _animator.SetBool("Hiding", false);

        if (_index > 20)
        {
            Debug.Log("If _index > 15");
            _agent.SetDestination(_wayPoints[_wayPoints.Count - 1].position);
        }
        else
        {
            Debug.Log("if _index > 15 ELSE");
            _index = UnityEngine.Random.Range(_index + 1, _wayPoints.Count - 1);
            _agent.SetDestination(_wayPoints[_index].position);
        }
        
        _currentAIState = aiStates.Run;
    }

    private void ColorSelector()
    {
        var selector = UnityEngine.Random.Range(0, 7);
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
                astronaut.color = Color.green;
                backpack.color = Color.green;
                break;
            case 3:
                astronaut.color = Color.magenta;
                backpack.color = Color.magenta;
                break;
            case 4:
                astronaut.color = Color.red;
                backpack.color = Color.red;
                break;
            case 5:
                astronaut.color = Color.white;
                backpack.color = Color.white;
                break;
            case 6:
                astronaut.color = Color.yellow;
                backpack.color = Color.yellow;
                break;
        }
    }
}
