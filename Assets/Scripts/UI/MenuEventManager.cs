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

    //Tween variables used to animate an object by scaling it up or down as needed
    protected Tween _scaleUpTween;
    protected Tween _scaleDownTween;    

    //Dictionary that will hold the selectables scale so we can change this when needed
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

    #region OnEnable
    public virtual void OnEnable()
    {
        //Subscribe to the on navigate method (This will allow us to use the keyboard to navigate the menu)
        _navigateReference.action.performed += OnNavigate;

        //ensure all selectable buttons are reset to their original size when this object is enabled
        //Loop through all the selectable buttons in our list and set their value to the store value in our dictionary
        for (int i = 0; i < SelectableButtons.Count; i++)
        {
            SelectableButtons[i].transform.localScale = _selectableButtonScales[SelectableButtons[i]];
        }

        StartCoroutine(SelectAfterDelay());
    }
    #endregion

    #region Coroutine 
    //Coroutine to allow atleast 1 frame allow the UI system to get set up before allowing this coroutine to be called
    protected virtual IEnumerator SelectAfterDelay()
    {
        yield return null;
        //Set the selected game object as the first selected game object
        EventSystem.current.SetSelectedGameObject(_firstSelected.gameObject);    
    }
    #endregion

    #region OnDisable
    public virtual void OnDisable()
    {
        //Unsubscribe to the on navigate method 
        _navigateReference.action.performed -= OnNavigate;

        //Disable/kill our tweens so it stops animating objects it can no longer access
        _scaleUpTween.Kill(true);
        _scaleDownTween.Kill(true);
    }
    #endregion 

    #region Method/Functions
    //Listeners that will be added to specified objects
    protected virtual void AddSelectionListeners(Selectable selectable)
    {
        //add listener if there is no listener
        EventTrigger trigger = selectable.gameObject.GetComponent<EventTrigger>();
        
        //If there there is no event trigger
        if (trigger == null)
        {
            trigger = selectable.gameObject.AddComponent<EventTrigger>();
        }

        //add select event if there is none
        EventTrigger.Entry SelectEntry = new EventTrigger.Entry
        {
            //Select the desired event ID from the event trigger options available (in this case we want select)
            eventID = EventTriggerType.Select
        };
        //When the specified event is trigger (OnSelect)
        SelectEntry.callback.AddListener(OnSelect);
        //Add this trigger to our trigger entry
        trigger.triggers.Add(SelectEntry);

        //add deslect event if there is none
        EventTrigger.Entry DeselectEntry = new EventTrigger.Entry
        {
            //Select the desired event ID from the event trigger options available (in this case we want deselect)
            eventID = EventTriggerType.Deselect
        };
        //When the specified event is trigger (OnDeselect)
        DeselectEntry.callback.AddListener(OnDeselect);
        //Add this trigger to our trigger entry
        trigger.triggers.Add(DeselectEntry);

        //add PointerEnter Event if there is none
        EventTrigger.Entry PointerEnter = new EventTrigger.Entry
        {
            //Select the desired event ID from the event trigger options available (in this case we want OnPointerEnter)
            eventID = EventTriggerType.PointerEnter
        };
        //When the pointer is over the button
        PointerEnter.callback.AddListener(OnPointerEnter);
        //Add this trigger to our trigger entry
        trigger.triggers.Add(PointerEnter);

        //add PointerExit Event if there is none
        EventTrigger.Entry PointerExit = new EventTrigger.Entry
        {
            //Select the desired event ID from the event trigger options available (in this case we want OnPointerExit)
            eventID = EventTriggerType.PointerExit
        };
        //When the pointer leaves the object it is over
        PointerExit.callback.AddListener(OnPointerExit);
        //Add this trigger to our trigger entry
        trigger.triggers.Add(PointerExit);
    }

    //This function using baseeventdata parameter will tell whatever wants to know that that something has trigger this function. 
    //In this case when a button has been pressed
    public void OnSelect(BaseEventData eventData)
    {       
        //If the selected object is in our animation exclusion list do not animate the object
        if (_animationExclusions.Contains(eventData.selectedObject))
        {
            return;
        }

        //When selecting an object store that this is the last object selected - this will update everytime we select an object
        _lastSelected = eventData.selectedObject.GetComponent<Selectable>();
        //What the new scale for the object being animated will be
        Vector3 newScale = eventData.selectedObject.transform.localScale * _selectedAnimationScale;
        //Tween the scale of the selected object upwards (changing the scale value smoothly overtime)
        _scaleUpTween = eventData.selectedObject.transform.DOScale(newScale, _scaleDuration);

        Debug.Log(eventData.selectedObject);
    }

    //This function using baseeventdata parameter will tell whatever wants to know that that something has trigger this function
    //In this case when a button has been deselected
    public void OnDeselect(BaseEventData eventData)
    {
        //If the selected object is in our animation exclusion list do not animate the object
        if (_animationExclusions.Contains(eventData.selectedObject))
        {
            return;
        }

        //What are we deselecting - store this data in the sel variable
        Selectable selectable = eventData.selectedObject.GetComponent<Selectable>();
        //Tween the object back to this objects stored scale (which was stored in our dictionary variable _selectableButtonScales)
        _scaleDownTween = eventData.selectedObject.transform.DOScale(_selectableButtonScales[selectable], _scaleDuration);
    }  

    //When this method is called - check to see if the pointer is over an object
    public void OnPointerEnter(BaseEventData eventData)
    {
        //Get the pointer event data from the event data (Pointer event data inherites from base event data)
        PointerEventData pointerEventData = eventData as PointerEventData;

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

    //When this function is called check to see if the pointer is no longer over an object
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

    //Detect when we press a button from our input action reference
    protected virtual void OnNavigate(InputAction.CallbackContext navigateContext)
    {
        //When we press a button (up, down, left right) fire an event.
        //This will allow us to navigate the menu with our keyboard even when the mouse pointer is not over a selectable object

        //If we have nothing selected and the last object selected variable is not null
        if (EventSystem.current.currentSelectedGameObject == null && _lastSelected != null)
        {
            //Set the selected object to the last selected object
            EventSystem.current.SetSelectedGameObject(_lastSelected.gameObject);
        }
    }
    #endregion
}
