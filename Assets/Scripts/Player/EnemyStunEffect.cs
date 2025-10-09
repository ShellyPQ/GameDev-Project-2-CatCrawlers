using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunEffect : MonoBehaviour
{
    #region Variables
    [Header("Shake Settings")]
    public float duration = 0.5f;
    public float shakeStrength = 0.1f;

    private Vector3 _originalPosition;
    private bool _isStunned = false;
    #endregion

    #region Method/Functions
    public void Stun()
    {
        if (!_isStunned)
        {
            StartCoroutine(Shake());
        }            
    }

    private IEnumerator Shake()
    {
        _isStunned = true;
        _originalPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-1f, 1f) * shakeStrength;
            float y = Random.Range(-1f, 1f) * shakeStrength;
            transform.localPosition = _originalPosition + new Vector3(x, y, 0);
            yield return null;
        }

        transform.localPosition = _originalPosition;
        _isStunned = false;
    }
    #endregion
}
