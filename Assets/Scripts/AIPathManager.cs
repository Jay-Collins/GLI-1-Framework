using System.Collections.Generic;
using UnityEngine;

public class AIPathManager : MonoSingleton<AIPathManager>
{
    [SerializeField] GameObject _hidePointsContainer;
    
    private Collider[] _hidePointsChildren;
    
    [Header("Points the AI may hide at - Generated automatically")]
    public List<Transform> wayPoints;

    private void OnEnable()
    {
        if (_hidePointsContainer is not null) // null check
            GenerateHidePointsList();
    }

    private void GenerateHidePointsList()
    {
        _hidePointsChildren = _hidePointsContainer.GetComponentsInChildren<Collider>();

        foreach (var child in _hidePointsChildren)
        {
            wayPoints.Add(child.transform);
        }
    }
    
}
