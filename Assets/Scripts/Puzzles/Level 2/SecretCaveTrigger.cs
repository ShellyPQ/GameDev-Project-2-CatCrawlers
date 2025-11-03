using Cinemachine;
using UnityEngine;

public class SecretCaveTrigger : MonoBehaviour
{
    #region Variables
    [Header("References")]
    [SerializeField] private GameObject _wallParent;
    [SerializeField] private CinemachineConfiner2D _confiner;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    [Header("Camera Bounds")]
    [SerializeField] private Collider2D _caveBounds;
    [SerializeField] private Collider2D _worldBounds;

    #endregion

    #region Trigger Functions

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //disable wall
            _wallParent.SetActive(false);

            //change zoom when entering the cave
            _confiner.m_BoundingShape2D = _caveBounds;
            _confiner.InvalidateCache();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //enable wall
            _wallParent.SetActive(true);

            //change zoom when entering the cave
            _confiner.m_BoundingShape2D = _worldBounds;
            _confiner.InvalidateCache();
        }
    }
    #endregion
}
