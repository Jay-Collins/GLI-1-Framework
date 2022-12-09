using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
 
public class Player : MonoBehaviour
{
    // Gitbash was giving me trouble so I added this script to the project as a reference to the last time
    // I used enums to communicate with the animator. 
    
    private enum AnimationState {WalkUp = 0, WalkDown = 1, WalkLeft = 2, WalkRight = 3}
 
    public bool playerMovementDisabled;
    private Vector2 _movementDirection;
 
    [SerializeField] float _moveSpeed = 2f;
    //[SerializeField] private Rigidbody2D _rb2D;
    [SerializeField] private Animator _animator;
 
    private AnimationState _animationState;
    private static readonly int _state = Animator.StringToHash("State");
    private static readonly int _speed = Animator.StringToHash("Speed");

    private void Update()
    {
        PlayerInput();
        AnimationStates();
    }
 
    private void FixedUpdate()
    {
        if (playerMovementDisabled) return;
 
        transform.Translate(_movementDirection.normalized * (_moveSpeed * Time.fixedDeltaTime));
    }
 
    private void PlayerInput()
    {
        if (playerMovementDisabled) return;
 
        _movementDirection.x = Input.GetAxisRaw("Horizontal");
        _movementDirection.y = Input.GetAxisRaw("Vertical");
    }
 
    private void AnimationStates()
    {
        if (playerMovementDisabled) return;
 
        _animationState = _movementDirection.x switch
        {
            1 => AnimationState.WalkRight, -1 => AnimationState.WalkLeft, _ => _animationState
        };
 
        _animationState = _movementDirection.y switch
        {
            1 => AnimationState.WalkUp, -1 => AnimationState.WalkDown, _ => _animationState
        };
 
        _animator.SetFloat(_speed, _movementDirection.sqrMagnitude);
        _animator.SetFloat(_state, ((int)_animationState));
    }
}