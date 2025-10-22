using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int levelNumber = 1;
    private Button _levelButton;

    private void Awake()
    {
        _levelButton = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_levelButton != null && _levelButton.interactable)
        {
            ChallengeManager.instance.DisplayChallenge(levelNumber);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_levelButton != null && _levelButton.interactable)
        {
            ChallengeManager.instance?.HideChallenges();
        }
    }
}
