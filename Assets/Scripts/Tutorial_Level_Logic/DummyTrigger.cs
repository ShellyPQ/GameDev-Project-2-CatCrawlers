using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTrigger : MonoBehaviour
{
    #region Variables
    [Header("Properties")]
    [Tooltip("Trigger Checks")]
    public bool canMelee = false;
    public bool canRange = false;
    #endregion

    #region Trigger Check
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.tag == "Melee Trigger" && collision.gameObject.tag == "Player")
        {
            //let the player use melee combat
            canMelee = true;
            //Turn this collider off
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            //Test this trigger is working
            Debug.Log("Melee can now be used");
        }
        else if (gameObject.tag == "Range Trigger" && collision.gameObject.tag == "Player")
        {
            //let the player use range combat
            canRange = true;
            //Turn this collider off
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            //Test this trigger is working
            Debug.Log("Range can now be used");
        }
    }
    #endregion
}
