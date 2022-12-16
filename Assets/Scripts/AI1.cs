using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class AI1 : MonoBehaviour
{
    private enum aiStates {Run = 0, Hide = 1, Death = 2}
    private aiStates _currentAIState;
    
    private NavMeshAgent _agent;
    private List<Transform> _wayPoints;
    private int _index;
    private bool _dead;
    private bool _hideTimerStarted;
    private bool _aiWins;

    [Header("Astronaut Settings")] 
    [SerializeField] private float _speed;
    [SerializeField] private int _scoreAwarded;
    
    [Header("Total value must be less than Stage Transition Wait Time")]
    [SerializeField] private float _despawnTime;
    [SerializeField] private float _fadeOutMultiplier = 1;
    
    [Header("Astronaut Sound Settings")]
    [SerializeField] private AudioClip[] _deathSounds;
    [SerializeField] private AudioClip _pathCompleteSound;

    [Header("Astronaut Hide Timer")] 
    [SerializeField] private int _minHideTime;
    [SerializeField] private int _maxHideTime;

    [Header("Astronaut Object And Component References")] 
    [SerializeField] private SkinnedMeshRenderer _astronautObject;
    [SerializeField] private SkinnedMeshRenderer _astronautBackpackObject;
    [SerializeField] private Animator _animator;
    [SerializeField] private Collider _collider;

    private void OnEnable()
    {
        Shoot.aiGotShot += GotShot;
        _wayPoints = AIPathManager.instance.wayPoints;
        _agent = GetComponent<NavMeshAgent>();
        _currentAIState = aiStates.Run;
        ColorSelector();

        _agent?.SetDestination(_wayPoints[0].position);
        _animator?.SetFloat("Speed", _speed);
    }

    private void Update()
    {
        // Debug.Log(_index);
        if (_dead) return;
        
        DecisionMaking();

        if (_agent is not null) // null check
            _agent.speed = _speed;
    }

    private void DecisionMaking()
    {
        switch (_currentAIState)
        {
            case aiStates.Run:
                RunState();
                break;
            case aiStates.Hide:
                if (!_hideTimerStarted)
                    StartCoroutine(HideStateRoutine());
                break;
            case aiStates.Death:
                if (!_dead)
                    OnDeath();
                break;
        }
    }

    private void RunState()
    {
        if (!_agent.pathPending && !_agent.hasPath && _index == _wayPoints.Count - 1)
        {
            if (!_aiWins)
                OnWin();
        }
        else if (_agent.remainingDistance < 0.5f && _index == 0)
        {
            _index = Random.Range(_index + 1, _wayPoints.Count - 1);
            _agent?.SetDestination(_wayPoints[_index].position);
        }
        else if (!_agent.pathPending && !_agent.hasPath)
            _currentAIState = aiStates.Hide;
    }
    
    private IEnumerator HideStateRoutine()
    {
        // Debug.Log("Hiding");
        
        _hideTimerStarted = true;
        _animator?.SetBool("Hiding", true);
        yield return new WaitForSeconds(Random.Range(_minHideTime, _maxHideTime));
        _animator?.SetBool("Hiding", false);
        
        // Debug.Log("Running");

        if (_index > 21)
        {
            _index = _wayPoints.Count - 1;
            _agent?.SetDestination(_wayPoints[_wayPoints.Count - 1].position);
        }
        else
        {
            _index = UnityEngine.Random.Range(_index + 1, _wayPoints.Count - 1);
            _agent?.SetDestination(_wayPoints[_index].position);
        }

        _currentAIState = aiStates.Run;
        _hideTimerStarted = false;
    }
    
    private void GotShot(Transform hitInfo)
    {
        if (transform == hitInfo.transform)
            _currentAIState = aiStates.Death;
    }
    
    private void OnWin()
    {
        SpawnManager.instance.enemiesWon += 1;
        _aiWins = true;
        if (_pathCompleteSound is not null) // null check
            AudioSource.PlayClipAtPoint(_pathCompleteSound, transform.position);
        if (_collider is not null) // null check
            _collider.isTrigger = true;
        if (_agent is not null) // null check
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }
        _index = 0;
        gameObject.SetActive(false);
    }

    private void OnDeath()
    {
        if (_collider is not null) // null check
            _collider.isTrigger = true;
        if (_agent is not null) // null check
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }
        _index = 0;
        _animator?.SetBool("Death", true);
        _dead = true;
        if (_deathSounds is not null) // null check
            AudioSource.PlayClipAtPoint(_deathSounds[UnityEngine.Random.Range(0, _deathSounds.Length - 1)], transform.position);
        UIManager.score += _scoreAwarded;
        SpawnManager.instance.enemiesKilled++;

        StartCoroutine(FadeRoutine());
    }
    
    private IEnumerator FadeRoutine()
    {
        var colorMain = _astronautObject.material.color;
        var colorSub = _astronautBackpackObject.material.color;

        yield return new WaitForSeconds(_despawnTime);

        while (colorMain.a >= 0 && colorSub.a >= 0)
        {
            colorMain.a -= Time.deltaTime * _fadeOutMultiplier;
            colorSub.a -= Time.deltaTime * _fadeOutMultiplier;
            if (_astronautObject is not null) // null check
                _astronautObject.material.color = colorMain;
            if (_astronautBackpackObject is not null) // null check
                _astronautBackpackObject.material.color = colorSub;
            
            yield return new WaitForEndOfFrame();
        }
        
        // Debug.Log("Setting false!");
        gameObject.SetActive(false);
    }
    
    private void OnDisable()
    {
        _dead = false;
        _aiWins = false;
        _hideTimerStarted = false;
        if (_agent is not null) // null check
            _agent.isStopped = false;
        _currentAIState = aiStates.Run;
        Shoot.aiGotShot -= GotShot;
    }

    private void ColorSelector()
    {
        var selector = Random.Range(0, 7);
        var astronaut = _astronautObject.material;
        var backpack = _astronautBackpackObject.material;
        
        if (_astronautObject is null || _astronautBackpackObject is null) return; // null check
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, _wayPoints[_index].position);
    }
}
