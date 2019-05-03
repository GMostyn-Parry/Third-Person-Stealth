using UnityEngine;

//Condition to check if the agent can see the player.
[CreateAssetMenu(menuName = "StateMachine/Conditions/CanSeePlayer")]
public class CanSeePlayerCondition : Condition
{
    public override bool Evaluate(StateController controller)
    {
        Vector3 difference = controller.target.transform.position - controller.transform.position;

        //We can see the player if the angle is less than the max angle of the vision cone.
        if(Vector3.Angle(difference, controller.transform.forward) <= controller.stats.maxVisionAngle)
        {
            Physics.Raycast(controller.transform.position, difference, out RaycastHit hit);

            //Return true if we hit something, and it was the target.
            return hit.collider != null && hit.collider.gameObject == controller.target;
        }

        return false;
    }
}