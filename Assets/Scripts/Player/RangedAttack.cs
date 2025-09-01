using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    #region Variables
    [Header("Yarn Properties")]
    [SerializeField] private GameObject _yarnPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _yarnSpeed = 10f;
    [SerializeField] private float _maxRange = 5f;
    [SerializeField] private int _maxAmmo = 3;

    [Header("Tutorial Settings")]
    [SerializeField] private bool _isTutorial = true;
    [SerializeField] private float _tutorialPickupRespawnTime = 5f;

    private int _currentAmmo;
    #endregion

    #region Start
    private void Start()
    {
        _currentAmmo = _isTutorial ? 1 : _maxAmmo;
    }
    #endregion

    #region Update
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Shoot();
        }
    }
    #endregion

    #region Methods
    private void Shoot()
    {
        //allow shooting even if ammo is 0 for puzzle use (set flag in projectile)
        GameObject yarn = Instantiate(_yarnPrefab, _firePoint.position, Quaternion.identity);
        YarnProjectile projectile = yarn.GetComponent<YarnProjectile>();

        if (projectile != null)
        {
            float dir = transform.localScale.x > 0 ? 1f : -1f;
            projectile.Initialize(Vector3.right * dir, _yarnSpeed, _maxRange, 2f, this);
        }
    }

    //called by YarnProjectile when hitting an enemy
    public void ConsumeAmmo()
    {
        if (_currentAmmo > 0)
        {
            _currentAmmo--;
        }
    }

    public bool AddAmmo(int amount)
    {
        if (_currentAmmo >= _maxAmmo)
            return false;

        _currentAmmo = Mathf.Min(_currentAmmo + amount, _maxAmmo);
        return true;
    }

    public int GetAmmo()
    {
        return _currentAmmo;
    }

    public void UnlockMaxAmmo()
    {
        _maxAmmo = 3;
        _currentAmmo = Mathf.Min(_currentAmmo, _maxAmmo);
        _isTutorial = false;
        Debug.Log("Full yarn capacity unlocked");
    }
    #endregion
}
