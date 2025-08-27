using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MovingPlatform : MonoBehaviour
{
    #region Variables
    [Header("Platform Properties")]
    [Tooltip("How fast the platform moves")]
    public float moveSpeed;
    [Tooltip("How close the platform needs to be before switching targets")]
    [SerializeField] private float _targetDist = 0.05f;
    [Header("Waypoint References")]
    public Transform posA, posB;

    //Store the vector3 of the target pos of the platform
    private Vector3 _targetPos;
    #endregion

    #region Start
    private void Start()
    {
        //Starting direction we want the platform to go to
        _targetPos = posA.position;
    }
    #endregion

    #region Update
    private void Update()
    {
        if (gameObject.tag != "TriggerPlatform")
        {
            MovePlatform();
        }        
    }
    #endregion

    #region Method/Functions
    public void MovePlatform()
    {
        if (Vector2.Distance(transform.position, posA.position) < _targetDist)
        {
            _targetPos = posB.position;
        }

        if (Vector2.Distance(transform.position, posB.position) < _targetDist) 
        {
            _targetPos = posA.position;
        }
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, moveSpeed * Time.deltaTime);
    }

    //ensure the player stays on the platform when on the platform by making the player a child of the platform
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.parent = this.transform;
        }
    }

    //unparent the player from the platform when the player leaves the platform
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.parent = null;
        }
    }
    #endregion

}
