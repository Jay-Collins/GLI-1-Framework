using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Shoot : MonoBehaviour
{
    public static Action<Transform> aiGotShot;
    
    [SerializeField] private Camera _mainCamera;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            ShootGun();
    }

    private void ShootGun()
    {
        if (Physics.Raycast(_mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out var hitInfo, Mathf.Infinity, 1 << 6 | 1 << 8))
        {
            switch (hitInfo.collider.gameObject.layer)
            {
                case 6:
                    aiGotShot(hitInfo.transform);
                    break;
                case 8:
                    Debug.Log("Barrier was shot!");
                    break;
            }
        }
        else
            Debug.Log("Miss!");
    }
}
