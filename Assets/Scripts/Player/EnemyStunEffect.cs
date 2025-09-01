using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunEffect : MonoBehaviour
{
    [Header("Squeeze Settings")]
    public float duration = 0.5f;
    public float scaleX = 1.2f;
    public float scaleY = 0.8f;
    public float twistAngle = 15f;

    private Vector3 _originalScale;
    private Quaternion _originalRotation;
    private bool _isStunned = false;

    public void Stun()
    {
        if (!_isStunned)
            StartCoroutine(SqueezeAndTwist());
    }

    private IEnumerator SqueezeAndTwist()
    {
        _isStunned = true;
        _originalScale = transform.localScale;
        _originalRotation = transform.rotation;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Sin(elapsed / duration * Mathf.PI);

            // Squeeze
            transform.localScale = new Vector3(
                Mathf.Lerp(_originalScale.x, _originalScale.x * scaleX, t),
                Mathf.Lerp(_originalScale.y, _originalScale.y * scaleY, t),
                _originalScale.z
            );

            // Twist
            float rotationZ = Mathf.Sin(elapsed / duration * Mathf.PI * 4) * twistAngle;
            transform.rotation = Quaternion.Euler(0, 0, rotationZ);

            yield return null;
        }

        transform.localScale = _originalScale;
        transform.rotation = _originalRotation;
        _isStunned = false;
    }
}
