using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleTriggerHandler : MonoBehaviour
{
    #region Variables

    [Header("Particle Properties")]
    [SerializeField] private float _fadeOutTime = 0.5f;

    //reference to our collectable manager
    private CollectibleManager _collectableManager;
    private MeshRenderer _meshRenderer;
    private Collider2D _collider;
    private ParticleSystem _particle;
    private ParticleSystem.MainModule _particleMain;

    #endregion

    #region Awake
    private void Awake()
    {
        _collectableManager = GetComponent<CollectibleManager>();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _collider = GetComponent<Collider2D>();
        _particle = GetComponentInChildren<ParticleSystem>();

        if (_particle != null)
        {
            _particleMain = _particle.main;
        }
    }
    #endregion

    #region Method/Functions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _collectableManager.Collect(collision.gameObject);

            SFXManager.instance.playSFX("pawToken");

            //disable visuals & collider
            _meshRenderer.enabled = false;
            _collider.enabled = false;

            //fade out particle
            if (_particle != null)
            {
                StartCoroutine(FadeOutParticle(_fadeOutTime));
            }
        }
    }

    private IEnumerator FadeOutParticle(float duration)
    {
        float time = 0f;
        Color startColor = _particleMain.startColor.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            Color newColor = startColor;
            newColor.a = alpha;
            _particleMain.startColor = newColor;
            yield return null;
        }

        _particle.Stop();
        _particle.Clear();
    }

    private IEnumerator FadeInParticle(float duration)
    {
        _particle.Play();
        float time = 0f;
        Color startColor = _particleMain.startColor.color;
        startColor.a = 0f;
        _particleMain.startColor = startColor;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1, time / duration);
            Color newColor = startColor;
            newColor.a = alpha;
            _particleMain.startColor = newColor;
            yield return null;
        }
    }
    #endregion
}
