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
    public int maxAmmo = 3;

    [SerializeField] private int _currentAmmo;
    #endregion

    #region Start
    private void Start()
    {
        //starting ammo 
        _currentAmmo = 0;
    }
    #endregion

    #region Update
    private void Update()
    {
        if (InputManager.instance.attackRange)
        {
            Shoot();
        }
    }
    #endregion

    #region Methods/Functions
    private void Shoot()
    {
        if (_currentAmmo <= 0)
        {
            Debug.Log("No ammo!");
            return;
        }

        GameObject yarn = Instantiate(_yarnPrefab, _firePoint.position, Quaternion.identity);
        YarnProjectile projectile = yarn.GetComponent<YarnProjectile>();

        if (projectile != null)
        {
            float dir = PlayerController.instance._isFacingRight ? 1f : -1f;
            Vector3 projectileDir = Vector3.right * dir;
            projectile.Initialize(projectileDir, _yarnSpeed, _maxRange, this);
        }
    }

    //called by YarnProjectile when hitting an enemy or dummy
    public void ConsumeAmmo()
    {
        if (_currentAmmo > 0)
        {
            _currentAmmo--;

            //update HUD
            HUDManager.instance.UpdateAmmoText(_currentAmmo);
        }
    }

    public bool AddAmmo(int amount)
    {
        if (_currentAmmo >= maxAmmo)
        {
            return false;
        }            

        _currentAmmo = Mathf.Min(_currentAmmo + amount, maxAmmo);

        SFXManager.instance.playSFX("yarnBall");

        //update HUD
        HUDManager.instance.UpdateAmmoText(_currentAmmo);

        return true;
    }

    public int GetAmmo()
    {
        return _currentAmmo;
    }

    public void UnlockMaxAmmo()
    {
        _currentAmmo = maxAmmo;
    }
    #endregion
}
