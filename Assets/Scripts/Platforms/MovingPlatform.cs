using System.Collections;
using UnityEngine;


public class MovingPlatform : MonoBehaviour
{
    #region Variables
    [Header("Platform Properties")]
    [Tooltip("Speed when going to A")]
    public float moveSpeed;
    [Tooltip("Speed when going to B")]
    public float moveSpeedToB;
    [Tooltip("How close the platform needs to be before switching targets")]
    [SerializeField] private float _targetDist = 0.05f;

    public float pauseTime = 0.5f;

    [Header("Waypoint References")]
    public Transform posA, posB;

    //Store the vector3 of the target pos of the platform
    private Vector3 _targetPos;
    private bool _isMoving = true;
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
        if (gameObject.tag != "TriggerPlatform" && _isMoving)
        {
            MovePlatform();
        }
    }
    #endregion

    #region Method/Functions
    public void MovePlatform()
    {
        float currentSpeed = (_targetPos == posB.position) ? moveSpeedToB : moveSpeed;

        if (Vector2.Distance(transform.position, posA.position) < _targetDist)
        {
            _targetPos = posB.position;
            StartCoroutine(PausePlatform());
        }

        if (Vector2.Distance(transform.position, posB.position) < _targetDist)
        {
            _targetPos = posA.position;
            StartCoroutine(PausePlatform());
        }

        transform.position = Vector3.MoveTowards(transform.position, _targetPos, currentSpeed * Time.deltaTime);
    }

    private IEnumerator PausePlatform()
    {
        _isMoving = false;
        yield return new WaitForSeconds(pauseTime);
        _isMoving = true;
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
