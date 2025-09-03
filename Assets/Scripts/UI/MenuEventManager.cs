using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class MenuEventManager : MonoBehaviour
{
    #region Variables
    [Header("References")]
    [Tooltip("List that will hold the buttons we want to interact with")]
    public List<Selectable> SelectableButtons = new List<Selectable>();
    [Tooltip("What selectable is selected first when we open a menu regardless of where the mouse pointer is at")]
    [SerializeField] protected Selectable _firstSelected;

    [Header("Controls")]
    [Tooltip("Reference to the input action we will be using to navigate the UI Menu")]
    [SerializeField] protected InputActionReference _navigateReference;

    [Header("Button Animations")]
    [Tooltip("Scale of the animation")]
    [SerializeField] protected float _selectedAnimationScale = 1.0f;
    [Tooltip("How long it will take the button to scale")]
    [SerializeField] protected float _scaleDuration = 0.2f;
    
    [Tooltip("List of selectables that we do not want to animate")]
    [SerializeField] protected List<GameObject> _animationExclusions = new List<GameObject>();        

    //dictionary to store each button's active tween
    protected Dictionary<Selectable, Tween> _activeTweens = new Dictionary<Selectable, Tween>();

    //dictionary that holds the original scale of each button
    protected Dictionary<Selectable, Vector3> _selectableButtonScales = new Dictionary<Selectable, Vector3>();

    //What selectable did we select last
    protected Selectable _lastSelected;
    
    #endregion

    #region Awake
    private void Awake()
    {
        //Loop through the selectable buttons we have stored in our selectable button list
        foreach (var selectableButtons in SelectableButtons)
        {
            //Add listeners to each selectable in our selectable button list
            AddSelectionListeners(selectableButtons);
            //Add the scales of each selectable button into our dictionary
            _selectableButtonScales.Add(selectableButtons,selectableButtons.transform.localScale);
        }
    }
    #endregion

    #region Start
    private void Start()
    {
        //Disable the navigation event in the unity built in event system (this is to prevent the event system from looping between on pointer enter and the built in event navigation system)
        EventSystem.current.sendNavigationEvents = false;
    }
    #endregion

    #region OnEnable
    public virtual void OnEnable()
    {
        //Subscribe to the on navigate method (This will allow us to use the keyboard to navigate the menu)
        _navigateReference.action.performed += OnNavigate;

        //reset all button scales
        foreach (var selectable in SelectableButtons)
        {
            selectable.transform.localScale = _selectableButtonScales[selectable];
        }

        StartCoroutine(SelectAfterDelay());
    }
    protected virtual IEnumerator SelectAfterDelay()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(_firstSelected.gameObject);        
    }
    #endregion                                         

    #region OnDisable
    public virtual void OnDisable()
    {
        //unsubscribe to navigation
        _navigateReference.action.performed -= OnNavigate;

        //kill all active tweens
        foreach (var tween in _activeTweens.Values)
        {
            tween.Kill();
        }
        _activeTweens.Clear();

        //reset scales safely
        foreach (var selectable in SelectableButtons)
        {
            if (selectable != null && _selectableButtonScales.ContainsKey(selectable))
            {
                selectable.transform.localScale = _selectableButtonScales[selectable];
            }
        }

        //clean up destroyed buttons
        SelectableButtons.RemoveAll(item => item == null);
    }
    #endregion

    #region OnDestroy
    private void OnDestroy()
    {
        foreach (var tween in _activeTweens.Values)
        {
            tween.Kill();
        }
        _activeTweens.Clear();
    }
    #endregion

    #region Method/Functions

    protected virtual void AddSelectionListeners(Selectable selectable)
    {
        //add listener if there is no listener
        EventTrigger trigger = selectable.gameObject.GetComponent<EventTrigger>();
        
        //If there there is no event trigger
        if (trigger == null)
        {
            trigger = selectable.gameObject.AddComponent<EventTrigger>();
        }

        // Select event
        EventTrigger.Entry selectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Select };
        selectEntry.callback.AddListener(OnSelect);
        trigger.triggers.Add(selectEntry);

        // Deselect event
        EventTrigger.Entry deselectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Deselect };
        deselectEntry.callback.AddListener(OnDeselect);
        trigger.triggers.Add(deselectEntry);

        // PointerEnter event
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener(OnPointerEnter);
        trigger.triggers.Add(pointerEnter);

        // PointerExit event
        EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener(OnPointerExit);
        trigger.triggers.Add(pointerExit);       
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        if (_animationExclusions.Contains(eventData.selectedObject))
        {
            return;
        }

        _lastSelected = eventData.selectedObject.GetComponent<Selectable>();
        Vector3 newScale = eventData.selectedObject.transform.localScale * _selectedAnimationScale;

        //kill any exisiting tween on this button
        if (_activeTweens.TryGetValue(_lastSelected, out Tween existingTween))
        {
            existingTween.Kill();
            _activeTweens.Remove(_lastSelected);
        }

        Tween tween = eventData.selectedObject.transform.DOScale(newScale, _scaleDuration);
        _activeTweens[_lastSelected] = tween;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (_animationExclusions.Contains(eventData.selectedObject))
        {
            return;
        }

        Selectable selectable = eventData.selectedObject.GetComponent<Selectable>();

        if (_activeTweens.TryGetValue(selectable, out Tween existingTween))
        {
            existingTween.Kill();
            _activeTweens.Remove(selectable);
        }

        Tween tween = eventData.selectedObject.transform.DOScale(_selectableButtonScales[selectable], _scaleDuration);
        _activeTweens[selectable] = tween;
    }  

    public void OnPointerEnter(BaseEventData eventData)
    {
        //Get the pointer event data from the event data (Pointer event data inherites from base event data)
        PointerEventData pointerEventData = eventData as PointerEventData;

        if (pointerEventData == null)
        {
            return ;
        }

        //Of the pointer event data is not null
        if (pointerEventData != null)
        {
            //Find a selectable object in the parent object
            Selectable selectable = pointerEventData.pointerEnter.GetComponentInParent<Selectable>();
            
            //If there is no selectable in the parent
            if (selectable == null)
            {
                //Try to find the selectable object in the children
                selectable = pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();
            }

            //Store the selectable object found in either the parent or the child object into the selected object variable
            pointerEventData.selectedObject = selectable.gameObject;
        }
    }
  
    public void OnPointerExit(BaseEventData eventData)
    {
        //Get the pointer event data from the event data (Pointer event data inherites from base event data)
        PointerEventData pointerEventData = eventData as PointerEventData;

        //if the pointer event data is not null
        if (pointerEventData != null)
        {
            //The pointer data is null (as it is no longer over an object)
            pointerEventData.selectedObject = null;
        }
    }

    protected virtual void OnNavigate(InputAction.CallbackContext navigateContext)
    {
        //If we have nothing selected and the last object selected variable is not null
        if (EventSystem.current.currentSelectedGameObject == null && _lastSelected != null)
        {
            //Set the selected object to the last selected object
            EventSystem.current.SetSelectedGameObject(_lastSelected.gameObject);
            //Enable the navigation event in the unity built in event system (this is to prevent the event system from looping between on pointer enter and the built in event navigation system)
            EventSystem.current.sendNavigationEvents = true;
        }
    }

    public void KillAllTweens()
    {
        //kill all active tweens
        foreach (var tween in _activeTweens.Values)
        {
            tween.Kill();
        }
        _activeTweens.Clear();

        //reset scales safely
        foreach (var selectable in SelectableButtons)
        {
            if (selectable != null && _selectableButtonScales.ContainsKey(selectable))
            {
                selectable.transform.localScale = _selectableButtonScales[selectable];
            }
        }

        //remove destroyed buttons from the list to keep things clean
        SelectableButtons.RemoveAll(item => item == null);
    }
    #endregion
}
