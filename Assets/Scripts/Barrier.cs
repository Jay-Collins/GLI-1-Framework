using System.Collections;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [SerializeField] private int _barrierHealth;
    [SerializeField] private float _respawnTime;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private BoxCollider _collider;
    private bool _timerActive;

    private void OnEnable() => Shoot.barrierGotShot += BarrierGotShot;
    
    private void Update()
    {
        if (_barrierHealth == 0 && !_timerActive)
        {
            if (_collider is not null) // null check
                _collider.enabled = false;
            if (_collider is not null) // null check 
                _renderer.enabled = false;
            StartCoroutine(RespawnTimer());
        }
    }
    
    private void BarrierGotShot(Transform hitInfo)
    {
        if (transform == hitInfo.transform)
            _barrierHealth--;
    }

    private IEnumerator RespawnTimer()
    {
        _timerActive = true;
        yield return new WaitForSeconds(_respawnTime);
        
        _barrierHealth = 2;
        if (_renderer is not null) // null check
            _renderer.enabled = true;
        if (_collider is not null) // null check
            _collider.enabled = true;
        _timerActive = false;
    }

    private void OnDisable() => Shoot.barrierGotShot += BarrierGotShot;
    
}
