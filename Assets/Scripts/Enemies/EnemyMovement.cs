using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : PathFinder
{
    [Range(50, 1000)]
    [SerializeField] float movementTriggerRange;
    GameObject target;

    protected override void Start()
    {
        base.Start();
        this.target = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(CheckTargetInRange());
    }

    /// <summary>
    /// Checks every 0.5 seconds if target has entered or exited movement trigger range.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckTargetInRange()
    {
        // Check if target has entered acitive range
        while (true)
        {
            if (this.target == null)     // Break if target is destroyed or missing
            {
                yield break;
            }

            float distance = Vector3.Distance(target.transform.position, transform.position);

            // Break from first loop if player is detected
            if (distance <= movementTriggerRange)
            {
                StartCoroutine(IterateOverCurves());
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        DrawUtilities.DrawSphereWithLabel(transform.position, target.transform.position,
                                            movementTriggerRange, Color.cyan, 10, "Movement Trigger");
    }


}
