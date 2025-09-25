using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    #region Variables
    [Header("References")]
    [Tooltip("The platform this button will trigger")]
    [SerializeField] GameObject _assignedPlatform;
    [SerializeField] private Animator _buttonAni;
    #endregion

    #region Awake
    private void Awake()
    {
        //assign animator on awake
        _buttonAni = GetComponent<Animator>();
    }
    #endregion

    #region Method/Functions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //change the tag on the assigned platform to Untagged
        if (collision.gameObject.tag == "Player")
        {
            //The platform will move once it is no longer tagged as a "TriggerPlatform"
            _assignedPlatform.gameObject.tag = "Untagged";
            // trigger animation
            _buttonAni.SetBool("ButtonPressed", true);
        }
        //Unlock SFX?
    }
    #endregion
}
