using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Shoot : MonoBehaviour
{
    public static Action<Transform> aiGotShot;
    public static int ammo;
    
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _reloadTime;
    [SerializeField] private int _ammoCount;
    private bool _cantShoot;
    private bool _reloading;

    private void Start()
    {
        ammo = _ammoCount;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && ammo > 0)
            ShootGun();
    }
    
    private void ShootGun()
    {
        ammo--;
        
        if (Physics.Raycast(_mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out var hitInfo, Mathf.Infinity, 1 << 6 | 1 << 8 | 1 << 9))
        {
            switch (hitInfo.collider.gameObject.layer)
            {
                case 6:
                    aiGotShot(hitInfo.transform);
                    Debug.Log("Kill confirmed!");
                    break;
                case 8:
                    Debug.Log("Barrier was shot!");
                    break;
                case 9:
                    Debug.Log("You cant shoot through that!");
                    break;
            }
        }
        else
            Debug.Log("Miss!");

        if (ammo <= 0)
        {
            StartCoroutine(AmmoReload());
        }
    }
    
    private IEnumerator AmmoReload()
    {
        yield return new WaitForSeconds(_reloadTime);
        ammo = _ammoCount;
    }

}
