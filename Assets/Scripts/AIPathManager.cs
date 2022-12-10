using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPathManager : MonoSingleton<AIPathManager>
{
    [SerializeField] GameObject _hidePointsContainer;
    private Transform[] _hidePointsChildren;
    
    [Header("Points the AI may hide at - Generated automatically")]
    public List<Transform> hidePoints;
    
    [Header("The path the AI follows.")]
    public List<Transform> runTheCourse;

    private void OnEnable()
    {
        GenerateHidePointsList();
    }

    private void GenerateHidePointsList()
    {
        _hidePointsChildren = _hidePointsContainer.GetComponentsInChildren<Transform>();

        foreach (var child in _hidePointsChildren)
        { 
            hidePoints.Add(child);
        }
    }
    
}
