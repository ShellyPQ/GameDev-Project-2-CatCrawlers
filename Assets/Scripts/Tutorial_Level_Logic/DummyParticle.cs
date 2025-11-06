using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyParticle : MonoBehaviour
{
    #region Variables

    [Header("Particle References")]
    [SerializeField] private ParticleSystem _bigHayExplosion;
    [SerializeField] private ParticleSystem _smallHayExplosion;

    [Header("Fade out properties")]
    [SerializeField] private SpriteRenderer _dummySprite;
    [SerializeField] private float _fadeDuration = 1.5f;
    [SerializeField] private float _disableDelay = 2.0f;

    [Header("Collider Reference")]
    [SerializeField] private Collider2D _hitBox;

    private bool _hasTriggered = false;

    #endregion

    public void PlayParticleEffect()
    {
        if (_hasTriggered)
        {
            return;
        }

        _hasTriggered = true;
        // Play explosion SFX
        SFXManager.instance.playSFX("dummyExplosion");

        //trigger particle effects
        if (_bigHayExplosion != null)
        {
            _bigHayExplosion.Play();
        }

        if (_smallHayExplosion != null)
        {
            _smallHayExplosion.Play();
        }

        //disable hitbox so player can pass through
        if (_hitBox != null)
        {
            _hitBox.enabled = false;
        }

        //fade out dummy
        if (_dummySprite != null)
        {
            StartCoroutine(FadeOutAndDisable());
        }
        else
        {
            StartCoroutine(FadeOutAndDelay());
        }
    }

    private IEnumerator FadeOutAndDisable()
    {
        Color startColor = _dummySprite.color;
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            float t = elapsed / _fadeDuration;
            _dummySprite.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1f, 0f, t));
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        _dummySprite.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        yield return new WaitForSeconds(_disableDelay);

        Destroy(gameObject);
    }

    private IEnumerator FadeOutAndDelay()
    {
        yield return new WaitForSeconds(_disableDelay);
        Destroy(gameObject);
    }

}
