using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderSFXOnRelease : MonoBehaviour, IPointerUpHandler
{
    #region Variables

    //variable that will hold the slider that will have the sfx applied to
    [SerializeField] private Slider _slider;

    #endregion

    #region Method/Functions

    //This will play when the lmb is release when pressing the slider
    public void OnPointerUp(PointerEventData eventData)
    {
        if (_slider != null) 
        {
            AudioClip clip = SFXManager.instance.GetClip("sfxSettingsButton");

            if (clip != null)
            {
                GameObject sfxObject = Instantiate(SFXManager.instance.soundObject);
                AudioSource source = sfxObject.GetComponent<AudioSource>();
                source.volume = _slider.value;
                source.clip = clip;
                source.Play();
            }
        }
    }
    #endregion
}
