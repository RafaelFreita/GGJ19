using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelTrigger : MonoBehaviour
{
    public Animator stateTree;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ChangeLevelTrigger levelTrigger = collision.GetComponent<ChangeLevelTrigger>();
        if (levelTrigger)
        {
            stateTree.SetInteger("currentLevel", levelTrigger.level);
        }
    }

}
