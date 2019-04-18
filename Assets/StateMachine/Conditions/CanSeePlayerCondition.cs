using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Conditions/CanSeePlayer")]
public class CanSeePlayerCondition : Condition
{
    public override bool Evaluate(StateController controller)
    {
        Vector3 direction = controller.target.transform.position - controller.transform.position;

        if(Vector3.Angle(direction, controller.transform.forward) <= controller.stats.maxVisionAngle)
        {

            RaycastHit hit;
            Physics.Raycast(controller.transform.position, direction, out hit);

            return hit.collider == true && hit.collider.gameObject == controller.target;
        }

        return false;
    }
}