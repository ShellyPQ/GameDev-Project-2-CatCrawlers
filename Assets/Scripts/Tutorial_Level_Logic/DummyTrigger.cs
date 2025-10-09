using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTrigger : MonoBehaviour
{
    #region Variables
    [Header("Properties")]
    [Tooltip("Trigger Type")]
    [SerializeField] private bool isMeleeTrigger = false;
    [SerializeField] private bool isRangeTrigger = false;

    [Header("References")]
    [SerializeField] private GameObject _ammoHUD;

    #endregion

    #region Trigger Check
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //check to see if its the player
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        //Enable melee attack
        if (isMeleeTrigger && collision.TryGetComponent(out MeleeAttack meleeScript))
        {
            meleeScript.enabled = true;
        }

        //Enable ranged attack
        if ((isRangeTrigger && collision.TryGetComponent(out RangedAttack rangeScript)))
        {
            rangeScript.enabled = true;
            _ammoHUD.SetActive(true);
        }

        //Disable this trigger so it only fires once
        GetComponent<Collider2D>().enabled = false;

    }
    #endregion
}
